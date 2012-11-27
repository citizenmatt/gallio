// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Gallio.Properties;
using ThreadState=System.Threading.ThreadState;

namespace Gallio.Common.Concurrency
{
    /// <summary>
    /// A <see cref="ThreadAbortScope" /> executes a block of code inside a special
    /// scope that is designed to issue and safely handle <see cref="Thread.Abort(object)" />
    /// on demand.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class may be used as a primitive for implementing higher-level protected
    /// scopes for the purpose of asynchronous cancelation and time-limited execution.
    /// </para>
    /// <para>
    /// The implementation distinguishes between <see cref="Thread.Abort(object)" />s that
    /// are initated using the <see cref="ThreadAbortScope.Abort" /> method and those
    /// that may be initiated by other system components.  Foreign aborts are not handled
    /// so they are allowed to continue to propagate.  This is to ensure that critical aborts
    /// such as those that are initiated by <see cref="AppDomain.Unload" /> can unwind the stack
    /// without interference.
    /// </para>
    /// </remarks>
    public class ThreadAbortScope
    {
        /// <summary>
        /// Maximum number of times to poll for a pending abort before giving up.
        /// </summary>
        private const int MaxThreadAbortPolls = 10;

        /// <summary>
        /// Indicates that the thread scope is not currently running abortable code.
        /// It could either not be entered, or it could be entered and running a
        /// protected block.
        /// </summary>
        private const int QuiescentState = 0;

        /// <summary>
        /// Indicates that the thread scope is running abortable code.
        /// </summary>
        private const int RunningState = 1;

        /// <summary>
        /// Indicates that <see cref="Thread.Abort(object)" /> is about to occur.
        /// </summary>
        private const int AbortPendingState = 2;

        /// <summary>
        /// Indicates that the scope has been aborted and a <see cref="Thread.Abort(object)" />
        /// has already been issued if needed.
        /// </summary>
        private const int AbortedState = 3;

        private object activeThread;
        private int state;

        /// <summary>
        /// Runs an action inside of the scope.
        /// </summary>
        /// <remarks>
        /// <para>
        /// At most one action may be in progress at any time.  This method is not
        /// reentrant and cannot be called concurrently from multiple threads.
        /// </para>
        /// </remarks>
        /// <param name="action">The action to run.</param>
        /// <returns>The <see cref="ThreadAbortException" /> that was caught if the action
        /// was aborted, or null if the action completed normally.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if an action is already running in this scope.</exception>
        /// <exception cref="Exception">Any other exception thrown by <paramref name="action"/> itself.</exception>
        [DebuggerHidden, DebuggerStepThrough]
        public ThreadAbortException Run(Action action)
        {
            // NOTE: This method has been optimized to minimize the total stack depth of the action
            //       by inlining blocks on the critical path that had previously been factored out.

            if (action == null)
                throw new ArgumentNullException("action");

            // First, ensure that this call is not re-entrant wrt. the current thread.
            if (Thread.VolatileRead(ref activeThread) == Thread.CurrentThread)
                throw new InvalidOperationException(Resources.ThreadAbortScope_ReentranceException);

            RuntimeHelpers.PrepareConstrainedRegions(); // MUST IMMEDIATELY PRECEDE TRY
            try
            {
                // Lock the scope for use by this thread only.
                if (Interlocked.CompareExchange(ref activeThread, Thread.CurrentThread, null) != null)
                    throw new InvalidOperationException(Resources.ThreadAbortScope_ReentranceException);

                // Run the action on the current thread (which is the "active" thread) and guarantee
                // that if an abort will occur, it must occur within this block and nowhere else.
                // This implies that we must wait for a pending abort to occur in case it has not
                // happened yet.
                try
                {
                    try
                    {
                        RuntimeHelpers.PrepareConstrainedRegions(); // MUST IMMEDIATELY PRECEDE TRY
                        try
                        {
                            // Preempt the action if it has been previously aborted.
                            int oldState = Interlocked.CompareExchange(ref state, RunningState, QuiescentState);
                            if (oldState == AbortedState)
                                Thread.CurrentThread.Abort(this);

                            // Run the action.
                            action();
                            return null;
                        }
                        finally // THIS IS A CONSTRAINED REGION
                        {
                            // Reset the state.
                            Interlocked.CompareExchange(ref state, QuiescentState, RunningState);
                        }
                    }
                    finally // NOT A CONSTRAINED REGION
                    {
                        // Cause the Abort to occur here if it is still pending.
                        WaitForThreadAbortIfAborting();
                    }
                }
                catch (ThreadAbortException ex) // NOT A CONSTRAINED REGION
                {
                    // Unwind the aborted state.
                    if (ex.ExceptionState == this)
                    {
                        Thread.ResetAbort();
                        Thread.VolatileWrite(ref state, AbortedState);
                        return ex;
                    }

                    throw;
                }
            }
            finally // THIS IS A CONSTRAINED REGION
            {
                // Release the scope from this thread's control.
                Interlocked.CompareExchange(ref activeThread, null, Thread.CurrentThread);
            }
        }

        /// <summary>
        /// Enters a protected context wherein it cannot receive a thread abort from this
        /// <see cref="ThreadAbortScope"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method enables critical system code to be protected from aborts that
        /// may affect the scope.  This call cannot be nested.
        /// </para>
        /// </remarks>
        /// <returns>An object that when disposed ends the protected block, possibly null if there was no protection necessary.</returns>
        [DebuggerHidden, DebuggerStepThrough]
        public IDisposable Protect()
        {
            if (Thread.VolatileRead(ref activeThread) == Thread.CurrentThread)
            {
                if (Interlocked.CompareExchange(ref state, QuiescentState, RunningState) == RunningState)
                    return new ResetToRunningState(this);

                WaitForThreadAbortIfAborting();
            }

            return null;
        }

        private sealed class ResetToRunningState : IDisposable
        {
            private readonly ThreadAbortScope scope;

            public ResetToRunningState(ThreadAbortScope scope)
            {
                this.scope = scope;
            }

            [DebuggerHidden, DebuggerStepThrough]
            public void Dispose()
            {
                if (Interlocked.CompareExchange(ref scope.state, RunningState, QuiescentState) == AbortedState)
                    Thread.CurrentThread.Abort(scope);
            }
        }

        /// <summary>
        /// Aborts the currently running action and prevents any further actions from running
        /// inside of this scope.
        /// </summary>
        [DebuggerHidden, DebuggerStepThrough]
        public void Abort()
        {
            for (;;)
            {
                // Move from the quiescent state directly to aborted, if possible.
                // We have no work to do if we're not running.
                int oldState = Interlocked.CompareExchange(ref state, AbortedState, QuiescentState);
                if (oldState != RunningState)
                    return;

                RuntimeHelpers.PrepareConstrainedRegions(); // MUST IMMEDIATELY PRECEDE TRY
                try
                {
                    // If we're running, try to move to the abort pending state.
                    // When successful, then actually perform the abort.
                    if (Interlocked.CompareExchange(ref state, AbortPendingState, RunningState) == RunningState)
                    {
                        ((Thread) Thread.VolatileRead(ref activeThread)).Abort(this);
                        return;
                    }
                }
                finally // THIS IS A CONSTRAINED REGION
                {
                    // Record the fact that the abort is no longer pending.  This is used
                    // by the thread to detect whether it should continue waiting for a
                    // pending abort or if that abort was lost somehow (reset by other code).
                    Interlocked.CompareExchange(ref state, AbortedState, AbortPendingState);
                }
            }
        }

        /// <summary>
        /// Because a thread abort is delivered asynchronously, we need to synchronize with
        /// it to ensure that it gets delivered.  Otherwise the pending thread abort exception
        /// could occur outside of our special region.
        /// </summary>
        [DebuggerHidden, DebuggerStepThrough]
        private void WaitForThreadAbortIfAborting()
        {
            // The polling limit is designed to work around a problem where the Thread.Abort
            // method can hang without ever actually aborting the thread.  This way at least
            // we make progress after a few milliseconds.
            for (int i = 0; i < MaxThreadAbortPolls; i++)
            {
                int currentState = Thread.VolatileRead(ref state);
                if (currentState == AbortPendingState
                    || Thread.CurrentThread.ThreadState == ThreadState.AbortRequested)
                {
                    Thread.Sleep(1);
                }
                else if (currentState == AbortedState)
                {
                    break;
                }
                else
                {
                    return;
                }
            }

            Thread.CurrentThread.Abort(this);
        }
    }
}
