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
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Gallio.Common.Collections;
using Gallio.Framework.Data;
using Gallio.Runtime.Conversions;
using Gallio.Runtime.Formatting;
using Gallio.Model;
using Gallio.Common.Diagnostics;
using Gallio.Common.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Represents the run-time state of a single instance of a <see cref="PatternTest" />
    /// that is to be executed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Typical lifecycle of <see cref="PatternTestInstanceState" />:
    /// <list type="bullet">
    /// <item>The <see cref="PatternTestController" /> creates a <see cref="PatternTestInstanceState" /> for the
    /// instance of the <see cref="PatternTest" /> to be executed using particular data bindings.</item>
    /// <item>The controller populates the instance state with slot values for each slot with
    /// an associated <see cref="IDataAccessor" /> in the <see cref="PatternTestState" />.</item>
    /// <item>The controller calls <see cref="PatternTestInstanceActions.BeforeTestInstanceChain" /> to give test extensions
    /// the opportunity to modify the instance state.</item>
    /// <item>The controller initializes, sets up, executes, tears down and disposes the test instance.</item>
    /// <item>The controller calls <see cref="PatternTestInstanceActions.AfterTestInstanceChain" /> to give test extensions
    /// the opportunity to clean up the instance state.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public class PatternTestInstanceState
    {
        private static readonly Key<PatternTestInstanceState> ContextKey = new Key<PatternTestInstanceState>("Gallio.PatternTestInstanceState");

        private readonly PatternTestStep testStep;
        private readonly PatternTestInstanceActions testInstanceActions;
        private readonly PatternTestState testState;
        private readonly IDataItem bindingItem;
        private readonly Dictionary<PatternTestParameter, object> testParameterValues;
        private readonly Dictionary<ISlotInfo, object> slotValues;
        private readonly UserDataCollection data;
        private readonly TestAction body;

        private Type fixtureType;
        private object fixtureInstance;
        private MethodInfo testMethod;
        private object[] testArguments;

        private string nameBase;
        private string nameSuffixes;

        /// <summary>
        /// Creates an initial test instance state object.
        /// </summary>
        /// <param name="testStep">The test step used to execute the test instance.</param>
        /// <param name="testInstanceActions">The test instance actions.</param>
        /// <param name="testState">The test state.</param>
        /// <param name="bindingItem">The data item.</param>
        /// <param name="body">The body of the test instance.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testStep"/>,
        /// <paramref name="testInstanceActions"/> or <paramref name="testState"/> or <paramref name="bindingItem"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="testState"/> belongs to a
        /// different test from the <paramref name="testStep"/>.</exception>
        internal PatternTestInstanceState(PatternTestStep testStep, 
            PatternTestInstanceActions testInstanceActions,
            PatternTestState testState, IDataItem bindingItem, TestAction body)
        {
            if (testStep == null)
                throw new ArgumentNullException("testStep");
            if (testInstanceActions == null)
                throw new ArgumentNullException("testInstanceActions");
            if (testState == null)
                throw new ArgumentNullException("testState");
            if (testStep.Test != testState.Test)
                throw new ArgumentException("The test state belongs to a different test from the test step.", "testState");
            if (bindingItem == null)
                throw new ArgumentNullException("bindingItem");
            if (body == null)
                throw new ArgumentNullException("body");

            this.testStep = testStep;
            this.testInstanceActions = testInstanceActions;
            this.testState = testState;
            this.bindingItem = bindingItem;
            this.body = body;

            testParameterValues = new Dictionary<PatternTestParameter, object>();
            slotValues = new Dictionary<ISlotInfo, object>();
            data = new UserDataCollection();
            nameBase = testStep.Name;
            nameSuffixes = string.Empty;
        }

        /// <summary>
        /// Gets the pattern test instance state from the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The pattern test instance state, or null if none.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is null.</exception>
        public static PatternTestInstanceState FromContext(TestContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            while (!context.Data.HasValue(ContextKey) && context.Parent != null)
                context = context.Parent;

            return context.Data.GetValueOrDefault(ContextKey, null);
        }

        internal void SetInContext(TestContext context)
        {
            context.Data.SetValue(ContextKey, this);
        }

        /// <summary>
        /// Gets the converter for data binding.
        /// </summary>
        public IConverter Converter
        {
            get { return testState.Converter; }
        }

        /// <summary>
        /// Gets the formatter for data binding.
        /// </summary>
        public IFormatter Formatter
        {
            get { return testState.Formatter; }
        }

        /// <summary>
        /// Gets the test step used to execute the test instance.
        /// </summary>
        public PatternTestStep TestStep
        {
            get { return testStep; }
        }

        /// <summary>
        /// Gets the test instance actions.
        /// </summary>
        public PatternTestInstanceActions TestInstanceActions
        {
            get { return testInstanceActions; }
        }

        /// <summary>
        /// Gets the test associated with this test instance state.
        /// </summary>
        public PatternTest Test
        {
            get { return testState.Test; }
        }

        /// <summary>
        /// Gets the test state associated with this test instance state.
        /// </summary>
        public PatternTestState TestState
        {
            get { return testState; }
        }

        /// <summary>
        /// Gets the data item obtained from the test's <see cref="DataBindingContext" />
        /// to create this state.
        /// </summary>
        public IDataItem BindingItem
        {
            get { return bindingItem; }
        }

        /// <summary>
        /// Gets the user data collection associated with the test instance state.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It may be used
        /// to associate arbitrary key/value pairs with the execution of the test instance.
        /// </para>
        /// </remarks>
        public UserDataCollection Data
        {
            get { return data; }
        }

        /// <summary>
        /// Gets or sets the test fixture type or null if none.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
        /// contains unbound generic parameters, is a generic parameter, has an element type.</exception>
        public Type FixtureType
        {
            get { return fixtureType; }
            set
            {
                if (value != null)
                {
                    if (value.ContainsGenericParameters || value.IsGenericParameter || value.HasElementType)
                        throw new ArgumentException("The fixture type must not be an array, pointer, reference, generic parameter, or contain unbound generic parameters.", "value");
                }

                fixtureType = value;
            }
        }

        /// <summary>
        /// Gets or sets the test fixture instance or null if none.
        /// </summary>
        public object FixtureInstance
        {
            get { return fixtureInstance; }
            set { fixtureInstance = value; }
        }

        /// <summary>
        /// Gets or sets the test method or null if none.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
        /// is contains unbound generic parameters.</exception>
        public MethodInfo TestMethod
        {
            get { return testMethod; }
            set
            {
                if (value != null)
                {
                    if (value.ContainsGenericParameters)
                        throw new ArgumentException("The test method must not contain unbound generic parameters.", "value");
                }

                testMethod = value;
            }
        }

        /// <summary>
        /// Gets or sets the test method arguments or null if none.
        /// </summary>
        public object[] TestArguments
        {
            get { return testArguments; }
            set { testArguments = value; }
        }

        /// <summary>
        /// Gets a mutable dictionary of slots and their bound values.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The dictionary maps slots to the values that will be stored in them
        /// during test execution.
        /// </para>
        /// <para>
        /// The slots should be valid for the type of test in question.  For example,
        /// a test fixture supports constructor parameters, generic type parameters,
        /// fields and properies declared by the test fixture type.  Likewise a test method
        /// supports method parameters and generic method parameters declared by the test
        /// method.  Other novel kinds of tests might additional capabilities.
        /// </para>
        /// <para>
        /// A value should be of a type that is compatible with the slot's <see cref="ISlotInfo.ValueType" />.
        /// If any type conversion is required, it should already have taken place prior to
        /// adding the value to this dictionary.
        /// </para>
        /// </remarks>
        public IDictionary<ISlotInfo, object> SlotValues
        {
            get { return slotValues; }
        }

        /// <summary>
        /// Gets a mutable dictionary of values assigned to test parameters.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The contents of the dictionary are initialized by the framework as part of the
        /// test parameter binding phase for the test instance, just before the "before test instance"
        /// actions run.
        /// </para>
        /// </remarks>
        public IDictionary<PatternTestParameter, object> TestParameterValues
        {
            get { return testParameterValues; }
        }

        /// <summary>
        /// Returns true if the <see cref="TestStep" /> is the <see cref="PatternTestState.PrimaryTestStep" />
        /// that was created for the test.  
        /// </summary>
        /// <remarks>
        /// <para>
        /// False if a new <see cref="PatternTestStep"/> 
        /// was created as a child of the primary test step just for this test instance.
        /// </para>
        /// </remarks>
        public bool IsReusingPrimaryTestStep
        {
            get { return testStep.IsPrimary; }
        }

        /// <summary>
        /// Gets or sets the basic name of the test instance which usually consists of the method or
        /// class name and a description of its bound argument values.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The name may only be changed during <see cref="PatternTestInstanceActions.BeforeTestInstanceChain" />
        /// when <see cref="IsReusingPrimaryTestStep" /> is false.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public string NameBase
        {
            get { return nameBase; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                nameBase = value;
                UpdateTestStepName();
            }
        }

        /// <summary>
        /// Adds a suffix to the name base.
        /// </summary>
        /// <param name="suffix">The suffix to add.</param>
        /// <remarks>
        /// <para>
        /// The suffixes may only be added during <see cref="PatternTestInstanceActions.BeforeTestInstanceChain" />
        /// when <see cref="IsReusingPrimaryTestStep" /> is false.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="suffix"/> is null.</exception>
        public void AddNameSuffix(string suffix)
        {
            if (suffix == null)
                throw new ArgumentNullException("suffix");

            nameSuffixes += ", " + suffix;
            UpdateTestStepName();
        }

        private void UpdateTestStepName()
        {
            testStep.Name = nameSuffixes.Length == 0 ? nameBase : nameBase + nameSuffixes;
        }

        /// <summary>
        /// Gets a fixture object creation specification using the state's bound <see cref="SlotValues"/>.
        /// </summary>
        /// <param name="type">The fixture type or generic type definition.</param>
        /// <returns>The fixture instance.</returns>
        /// <remarks>
        /// The values of <see cref="FixtureType" /> and <see cref="FixtureInstance" /> are not used.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the slots or values in <see cref="SlotValues" />
        /// are not appropriate for instantiating <paramref name="type"/>.</exception>
        /// <seealso cref="ObjectCreationSpec"/>
        public ObjectCreationSpec GetFixtureObjectCreationSpec(ITypeInfo type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return new ObjectCreationSpec(type, SlotValues, Converter);
        }

        /// <summary>
        /// Gets a test method invocation specification using the state's bound <see cref="SlotValues"/>.
        /// </summary>
        /// <param name="method">The test method or generic method definition,
        /// possibly declared by a generic type or generic type defintion.</param>
        /// <returns>The method return value.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the slots or values in <see cref="SlotValues" />
        /// or <see cref="FixtureType" /> are not appropriate for invoking <paramref name="method"/></exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="FixtureType" /> is null.</exception>
        /// <seealso cref="MethodInvocationSpec"/>
        public MethodInvocationSpec GetTestMethodInvocationSpec(IMethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (fixtureType == null)
                throw new InvalidOperationException("This method cannot be used when FixtureType is null.");

            return new MethodInvocationSpec(fixtureType, method, SlotValues, Converter);
        }

        /// <summary>
        /// Invokes a fixture method using the specified <paramref name="slotValues"/>.
        /// </summary>
        /// <param name="method">The fixture method or generic method definition,
        /// possibly declared by a generic type or generic type defintion.</param>
        /// <param name="slotValues">The slot values to use for invoking the method.</param>
        /// <returns>The method return value.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> or <paramref name="slotValues"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the slots or values in <see cref="SlotValues" />
        /// or <see cref="FixtureType" /> or <see cref="FixtureInstance" /> are not appropriate for
        /// invoking <paramref name="method"/></exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="FixtureType" /> is null.</exception>
        /// <exception cref="Exception">Any exception thrown by the invoked method.</exception>
        /// <seealso cref="MethodInvocationSpec"/>
        [UserCodeEntryPoint, DebuggerNonUserCode]
        public object InvokeFixtureMethod(IMethodInfo method, IEnumerable<KeyValuePair<ISlotInfo, object>> slotValues)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (slotValues == null)
                throw new ArgumentNullException("slotValues");
            if (fixtureType == null)
                throw new InvalidOperationException("This method cannot be used when FixtureType is null.");

            var spec = new MethodInvocationSpec(fixtureType, method, slotValues, Converter);
            return spec.Invoke(fixtureInstance);
        }

        /// <summary>
        /// Invokes the test method specified by <see cref="TestMethod" />, <see cref="FixtureInstance" />
        /// and <see cref="TestArguments" />.  
        /// </summary>
        /// <remarks>
        /// <para>
        /// If there is no test method or no arguments, the method does nothing.
        /// </para>
        /// </remarks>
        /// <returns>The method return value, or null if there was none.</returns>
        /// <exception cref="Exception">Any exception thrown by the invoked method.</exception>
        [UserCodeEntryPoint, DebuggerNonUserCode]
        public object InvokeTestMethod()
        {
            if (testMethod != null && testArguments != null)
                return ExceptionUtils.InvokeMethodWithoutTargetInvocationException(testMethod, fixtureInstance, testArguments);

            return null;
        }

        /// <summary>
        /// Runs the body of the test.
        /// </summary>
        /// <returns>The test outcome.</returns>
        [UserCodeEntryPoint, DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
        internal static TestOutcome RunBody(PatternTestInstanceState state)
        {
            return state.body();
        }
    }
}