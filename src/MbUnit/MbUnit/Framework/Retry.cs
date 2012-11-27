﻿// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Gallio.Common;
using Gallio.Framework.Assertions;
using Gallio.Framework.Pattern;
using Gallio.Common.Time;
using Gallio.Runtime;

namespace MbUnit.Framework
{
    /// <summary>
    /// Evaluates repeatedly the specified condition until it becomes fulfilled, or throws
    /// a <see cref="AssertionFailureException"/> if a timeout occured, or if the
    /// evaluation was done unsuccessfully too many times.
    /// </summary>
    /// <example>
    /// Here is an example that illustrates the usage of <see cref="Retry"/>.
    /// <code><![CDATA[
    /// [TestFixture]
    /// public class FooTest
    /// {
    ///     [Test]
    ///     public void MyRetryTest()
    ///     {
    ///         Retry.Repeat(10) // Retries maximum 10 times the evaluation of the condition.
    ///              .WithPolling(TimeSpan.FromSeconds(1)) // Waits approximatively for 1 second between each evaluation of the condition.
    ///              .WithTimeout(TimeSpan.FromSeconds(30)) // Sets a timeout of 30 seconds.
    ///              .DoBetween(() => { /* DoSomethingBetweenEachCall */ })
    ///              .Until(() => { return EvaluateSomeCondition(); });
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    public sealed class Retry
    {
        internal static readonly int DefaultRepeat = -1;
        internal static readonly TimeSpan DefaultPolling = TimeSpan.Zero;
        internal static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Specifies the maximum number of evaluation attempts, before the <see cref="Retry.Until(Func{bool})"/>, or 
        /// <see cref="Retry.Until(WaitHandle)"/> operation fails.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If not specified, the condition will be evaluated an infinite number of times, or until some
        /// other stop criterion is reached.
        /// </para>
        /// </remarks>
        /// <param name="repeat">The maximum number of evaluation cycles, or zero or a negative value to specify an unlimited number of attempts.</param>
        /// <returns>An instance to specify other options for the retry operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the options was called more than once.</exception>
        public static IRetryOptions Repeat(int repeat)
        {
            return GetDefaultOptions().Repeat(repeat);
        }

        /// <summary>
        /// Specifies a polling time expressed in milliseconds between each consecutive evaluation of the condition.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If not specified, the default polling time is zero; which causes the condition to be evaluated as fast as possible, with
        /// only a brief suspension of the current thread (<see cref="Thread.Sleep(int)"/>).
        /// </para>
        /// </remarks>
        /// <param name="milliseconds">The polling time expressed in milliseconds.</param>
        /// <returns>An instance to specify other options for the retry operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the option was called more than once.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="milliseconds"/> is negative.</exception>
        public static IRetryOptions WithPolling(int milliseconds)
        {
            return GetDefaultOptions().WithPolling(milliseconds);
        }

        /// <summary>
        /// Specifies a polling time between each consecutive evaluation of the condition.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If not specified, the default polling time is zero; which causes the condition to be evaluated as fast as possible, with
        /// only a brief suspension of the current thread (<see cref="Thread.Sleep(TimeSpan)"/>).
        /// </para>
        /// </remarks>
        /// <param name="polling">The polling time.</param>
        /// <returns>An instance to specify other options for the retry operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the option was called more than once.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="polling"/> represents a negative duration.</exception>
        public static IRetryOptions WithPolling(TimeSpan polling)
        {
            return GetDefaultOptions().WithPolling(polling);
        }

        /// <summary>
        /// Specifies a timeout value expressed in milliseconds.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The retry operation fails if the overall duration exceeds the specified timeout value.
        /// </para>
        /// <para>
        /// If not specified, the default timeout value is set to 10 seconds.
        /// </para>
        /// </remarks>
        /// <param name="milliseconds">The timeout value expressed in milliseconds, or a negative value to specify no timeout value.
        /// A value of zero, will let the condition to be evaluated only once.</param>
        /// <returns>An instance to specify other options for the retry operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the option was called more than once.</exception>
        public static IRetryOptions WithTimeout(int milliseconds)
        {
            return GetDefaultOptions().WithTimeout(milliseconds);
        }

        /// <summary>
        /// Specifies a timeout value.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The retry operation fails if the overall duration exceeds the specified timeout value.
        /// </para>
        /// <para>
        /// If not specified, the default timeout value is set to 10 seconds.
        /// </para>
        /// </remarks>
        /// <param name="timeout">The timeout value, or a negative value to specify no timeout value.
        /// A value of zero, will let the condition to be evaluated only once.</param>
        /// <returns>An instance to specify other options for the retry operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the option was called more than once.</exception>
        public static IRetryOptions WithTimeout(TimeSpan timeout)
        {
            return GetDefaultOptions().WithTimeout(timeout);
        }

        /// <summary>
        /// Specifies a custom action to be executed between each consecutive attempt, but not before the first one.
        /// </summary>
        /// <param name="action">A custom action to be executed.</param>
        /// <returns>An instance to specify other options for the retry operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the option was called more than once.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
        public static IRetryOptions DoBetween(Action action)
        {
            return GetDefaultOptions().DoBetween(action);
        }

        /// <summary>
        /// Specifies a custom formatted message to be added to the text of the assertion raised when
        /// the retry operation has failed.
        /// </summary>
        /// <param name="messageFormat">A user-supplied assertion failure message string, or null if none.</param>
        /// <param name="messageArgs">The format arguments, or null or empty if none.</param>
        /// <returns></returns>
        public static IRetryOptions WithFailureMessage(string messageFormat, params object[] messageArgs)
        {
            return GetDefaultOptions().WithFailureMessage(messageFormat, messageArgs);
        }

        internal static IRetryOptions WithClock(IClock clock)
        {
            return new RetryOptions(clock);
        }

        /// <summary>
        /// Specifies the condition to evaluate repeatedly, and starts the entire operation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The condition is considered fulfilled when it returns true.
        /// </para>
        /// </remarks>
        /// <param name="condition">The condition to evaluate.</param>
        /// <exception cref="AssertionFailureException">Thrown when the condition is still false, and a timeout occured, or the maximum
        /// number of evaluation attempts was reached.</exception>
        public static void Until(Func<bool> condition)
        {
            GetDefaultOptions().Until(condition);
        }

        /// <summary>
        /// Specifies a <see cref="WaitHandle"/> instance to wait for being signaled, and starts the entire operation.
        /// </summary>
        /// <param name="waitHandle">The wait handle to evaluate.</param>
        /// <exception cref="AssertionFailureException">Thrown when the wait handle is still unsignaled, and a timeout occured, or the maximum
        /// number of evaluation attempts was reached.</exception>
        /// <seealso cref="WaitHandle.WaitOne(int,bool)"/>
        public static void Until(WaitHandle waitHandle)
        {
            GetDefaultOptions().Until(waitHandle);
        }

        /// <summary>
        /// Specifies a <see cref="Thread"/> instance to wait for being terminated, and starts the entire operation.
        /// </summary>
        /// <param name="tread">The thread to evaluate.</param>
        /// <exception cref="AssertionFailureException">Thrown when the thread is still alive, and a timeout occured, or the maximum
        /// number of evaluation attempts was reached.</exception>
        /// <seealso cref="Thread.Join(int)"/>
        public static void Until(Thread tread)
        {
            GetDefaultOptions().Until(tread);
        }

        private static IRetryOptions GetDefaultOptions()
        {
            return new RetryOptions(new Clock());
        }

        private class RetryOptions : IRetryOptions
        {
            private int? repeat = null;
            private TimeSpan? polling = null;
            private TimeSpan? timeout = null;
            private Action action;
            private string messageFormat;
            private object[] messageArgs;
            private IClock clock;

            public RetryOptions(IClock clock)
            {
                this.clock = clock;
            }

            public IRetryOptions Repeat(int repeat)
            {
                if (this.repeat.HasValue)
                    throw new InvalidOperationException("Expected the maximum number of evaluation cycles to be specified only once.");

                this.repeat = repeat;
                return this;
            }

            public IRetryOptions WithPolling(int milliseconds)
            {
                return WithPolling(TimeSpan.FromMilliseconds(milliseconds));
            }

            public IRetryOptions WithPolling(TimeSpan polling)
            {
                if (this.polling.HasValue)
                    throw new InvalidOperationException("Expected the polling time to be specified only once.");

                if (polling < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException("polling", "The polling time must be greater than or equal to zero.");

                this.polling = polling;
                return this;
            }

            public IRetryOptions WithTimeout(int milliseconds)
            {
                return WithTimeout(TimeSpan.FromMilliseconds(milliseconds));
            }

            public IRetryOptions WithTimeout(TimeSpan timeout)
            {
                if (this.timeout.HasValue)
                    throw new InvalidOperationException("Expected the timeout value to be specified only once.");

                this.timeout = timeout;
                return this;
            }

            public IRetryOptions DoBetween(Action action)
            {
                if (this.action != null)
                    throw new InvalidOperationException("Expected the custom action to be specified only once.");

                if (action == null)
                    throw new ArgumentNullException("action");

                this.action = action;
                return this;
            }

            public IRetryOptions WithFailureMessage(string messageFormat, params object[] messageArgs)
            {
                this.messageFormat = messageFormat;
                this.messageArgs = messageArgs;
                return this;
            }

            public void Until(Func<bool> condition)
            {
                if (condition == null)
                    throw new ArgumentNullException("condition");

                Run(condition);
            }

            public void Until(WaitHandle waitHandle)
            {
                if (waitHandle == null)
                    throw new ArgumentNullException("waitHandle");

                Run(() => waitHandle.WaitOne(0, false));
            }

            public void Until(Thread thread)
            {
                if (thread == null)
                    throw new ArgumentNullException("thread");

                Run(() => thread.Join(0));
            }

            private void Run(Func<bool> condition)
            {
                var runner = new RetryRunner(
                    repeat ?? Retry.DefaultRepeat, 
                    polling ?? Retry.DefaultPolling, 
                    timeout ?? Retry.DefaultTimeout,
                    action, condition, messageFormat, messageArgs, clock);
                runner.Run();
            }
        }

        private class RetryRunner
        {
            private readonly int repeat;
            private readonly TimeSpan polling;
            private readonly TimeSpan timeout;
            private readonly Action action;
            private readonly Func<bool> condition;
            private readonly string messageFormat;
            private readonly object[] messageArgs;
            private IClock clock;
            private int count;

            public RetryRunner(int repeat, TimeSpan polling, TimeSpan timeout, Action action, 
                Func<bool> condition, string messageFormat, object[] messageArgs, IClock clock)
            {
                this.repeat = repeat;
                this.polling = polling;
                this.timeout = timeout;
                this.condition = condition;
                this.action = action;
                this.messageFormat = messageFormat;
                this.messageArgs = messageArgs;
                this.clock = clock;
            }

            private bool EvaluateCondition()
            {
                try
                {
                    return condition();
                }
                catch (Exception exception)
                {
                    AssertionHelper.Fail(new AssertionFailureBuilder("The 'Retry.Until' operation has failed " +
                        "because an exception occured during the evaluation of the condition.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddException(exception)
                        .ToAssertionFailure());
                    return false; // Make the compiler happy.
                }
            }

            private bool Continue()
            {
                if ((repeat > 0) && (count++ >= repeat))
                {
                    AssertionHelper.Fail(new AssertionFailureBuilder("The 'Retry.Until' operation has failed too many times.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddRawLabeledValue("Maximum Number Of Attempts", repeat)
                        .ToAssertionFailure());
                }

                if ((timeout >= TimeSpan.Zero) && (clock.Elapsed >= timeout))
                {
                    AssertionHelper.Fail(new AssertionFailureBuilder("The 'Retry.Until' operation has failed due to a timeout error.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddLabeledValue("Timeout Value", String.Format("{0} ms", timeout.TotalMilliseconds))
                        .ToAssertionFailure());
                }

                return true;
            }

            public void Run()
            {
                count = 0;
                clock.Start();

                do
                {
                    if (EvaluateCondition())
                        return;

                    if (action != null)
                        action();

                    clock.ThreadSleep(polling);
                } while (Continue());
            }
        }
    }
}
