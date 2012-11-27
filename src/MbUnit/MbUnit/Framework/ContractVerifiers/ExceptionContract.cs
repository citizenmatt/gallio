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
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Gallio.Common.Collections;
using Gallio.Framework.Assertions;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Contract for verifying the implementation of a custom exception type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Built-in verifications:
    /// <list type="bullet">
    /// <item>
    /// <strong>HasSerializableAttribute</strong> : The exception type has the <see cref="SerializableAttribute" /> 
    /// attribute. Disable that test by settings the <see cref="ExceptionContract{TException}.ImplementsSerialization"/>
    /// property to <c>false</c>.
    /// </item>
    /// <item>
    /// <strong>HasSerializationConstructor</strong> : The exception type has a protected serialization 
    /// constructor similar to <see cref="Exception(SerializationInfo, StreamingContext)" />. Disable that test 
    /// by settings the <see cref="ExceptionContract{TException}.ImplementsSerialization"/>
    /// property to <c>false</c>.
    /// </item>
    /// <item>
    /// <strong>IsDefaultConstructorWellDefined</strong> : The exception type has a default parameter-less 
    /// constructor. When the <see cref="ExceptionContract{TException}.ImplementsSerialization"/> property
    /// is set to <c>true</c> as well, the method verifies that the properties of 
    /// the exception are preserved during a roundtrip serialization. Disable the test 
    /// by settings the <see cref="ExceptionContract{TException}.ImplementsStandardConstructors"/>
    /// property to <c>false</c>. 
    /// </item>
    /// <item>
    /// <strong>IsMessageConstructorWellDefined</strong> : The exception type has single argument constructor 
    /// for the message. When the <see cref="ExceptionContract{TException}.ImplementsSerialization"/> property
    /// is set to <c>true</c> as well, the method verifies that the properties of 
    /// the exception are preserved during a roundtrip serialization. Disable the test 
    /// by settings the <see cref="ExceptionContract{TException}.ImplementsStandardConstructors"/>
    /// property to <c>false</c>. 
    /// </item>
    /// <item>
    /// <strong>IsMessageAndInnerExceptionConstructorWellDefined</strong> : The exception type has two 
    /// parameters constructor for the message and an inner exception. 
    /// When the <see cref="ExceptionContract{TException}.ImplementsSerialization"/> property
    /// is set to <c>true</c> as well, the method verifies that the properties of 
    /// the exception are preserved during a roundtrip serialization. Disable the test 
    /// by settings the <see cref="ExceptionContract{TException}.ImplementsStandardConstructors"/>
    /// property to <c>false</c>. 
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example shows a simple custom exception class with some 
    /// basic serialization support, and a test fixture which uses the
    /// exception contract to test it.
    /// <code><![CDATA[
    /// [Serializable]
    /// public class SampleException : Exception, ISerializable
    /// {
    ///     public SampleException()
    ///     {
    ///     }
    /// 
    ///     public SampleException(string message)
    ///         : base(message)
    ///     {
    ///     }
    /// 
    ///     public SampleException(string message, Exception innerException)
    ///         : base(message, innerException)
    ///     {
    ///     }
    /// 
    ///     protected SampleException(SerializationInfo info, StreamingContext context)
    ///         : base(info, context)
    ///     {
    ///     }
    /// 
    ///     public override void GetObjectData(SerializationInfo info, StreamingContext context)
    ///     {
    ///         base.GetObjectData(info, context);
    ///     }
    /// }
    /// 
    /// public class SampleExceptionTest
    /// {
    ///     [VerifyContract]
    ///     public readonly IContract ExceptionTests = new ExceptionContract<SampleException>()
    ///     {
    ///         ImplementsSerialization = true, // Optional (default is true)
    ///         ImplementsStandardConstructors = true // Optional (default is true)
    ///     };
    /// }
    /// ]]></code>
    /// </example>
    /// <typeparam name="TException">The target custom exception type.</typeparam>
    /// <seealso cref="VerifyContractAttribute"/>
    public class ExceptionContract<TException> : AbstractContract
        where TException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExceptionContract()
        {
            this.ImplementsSerialization = true;
            this.ImplementsStandardConstructors = true;
        }

        /// <summary>
        /// Determines whether the verifier will check for the serialization support. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is <c>true</c>.
        /// </para>
        /// <para>
        /// Built-in verifications:
        /// <list type="bullet">
        /// <item>The exception implements the <see cref="ISerializable" /> interface.</item>
        /// <item>The exception has the <see cref="SerializableAttribute" /> attribute.</item>
        /// <item>The exception type has a protected serialization constructor similar to
        /// <see cref="Exception(SerializationInfo, StreamingContext)" />.</item>
        /// </list>
        /// </para>
        /// </remarks>
        public bool ImplementsSerialization
        {
            get;
            set;
        }

        /// <summary>
        /// Determines whether the verifier will check for the presence of
        /// the recommended standard constructors.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is <c>true</c>.
        /// </para>
        /// <para>
        /// Built-in verifications:
        /// <list type="bullet">
        /// <item>The exception has a default parameter-less constructor.</item>
        /// <item>The exception has a single parameter constructor for the message.</item>
        /// <item>The exception two parameters constructor for the message and an inner exception.</item>
        /// </list>
        /// </para>
        /// </remarks>
        public bool ImplementsStandardConstructors
        {
            get;
            set;
        }

        /// <inheritdoc />
        protected override IEnumerable<Test> GetContractVerificationTests()
        {
            if (ImplementsSerialization)
            {
                // Has Serializable attribute?
                yield return CreateSerializableAttributeTest("HasSerializableAttribute");

                // Has non-public serialization constructor?
                yield return CreateSerializationConstructorTest("HasSerializationConstructor");
            }

            if (ImplementsStandardConstructors)
            {
                // Is public default constructor well defined?
                yield return CreateStandardConstructorTest("DefaultConstructor",
                    "",
                    EmptyArray<Type>.Instance,
                    new object[][]
                    {
                        EmptyArray<object>.Instance
                    });

                // Is public single parameter constructor (message) well defined?
                yield return CreateStandardConstructorTest("MessageConstructor",
                    "string",
                    new Type[] { typeof(string) },
                    new object[][]
                    {
                        new object[] { null },
                        new object[] { "" },
                        new object[] { "A message" }
                    });

                // Is public two parameters constructor (message and inner exception) well defined?
                yield return CreateStandardConstructorTest("MessageAndInnerExceptionConstructor",
                    "string, Exception",
                    new Type[] { typeof(string), typeof(Exception) },
                    new object[][]
                    {
                        new object[] { null, null },
                        new object[] { "", null },
                        new object[] { "A message", null },
                        new object[] { null, new Exception() },
                        new object[] { "", new Exception() },
                        new object[] { "A message", new Exception() }
                    });
            }
        }

        private Test CreateSerializableAttributeTest(string name)
        {
            return new TestCase(name, () =>
            {
                AssertionHelper.Verify(() =>
                {
                    if (typeof(TException).IsDefined(typeof(SerializableAttribute), false))
                        return null;

                    return new AssertionFailureBuilder("Expected the exception type to have the [Serializable] attribute.")
                        .AddRawLabeledValue("Exception Type", typeof(TException))
                    .SetStackTrace(Context.GetStackTraceData())
                        .ToAssertionFailure();
                });
            });
        }

        private Test CreateSerializationConstructorTest(string name)
        {
            return new TestCase(name, () =>
            {
                var constructor = typeof(TException).GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                    new[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);

                AssertionHelper.Explain(() =>
                    Assert.IsNotNull(constructor), 
                    innerFailures => new AssertionFailureBuilder(
                        "Expected the exception type to have a serialization constructor with signature .ctor(SerializationInfo, StreamingContext).")
                        .AddRawLabeledValue("Exception Type", typeof(TException))
                        .SetStackTrace(Context.GetStackTraceData())
                        .AddInnerFailures(innerFailures)
                        .ToAssertionFailure());
            });
        }

        private Test CreateStandardConstructorTest(string name, string constructorSignature,
            Type[] constructorParameterTypes, object[][] constructorArgumentLists)
        {
            return new TestCase(name, () =>
            {
                var constructor = typeof(TException).GetConstructor(BindingFlags.Instance | BindingFlags.Public, 
                    null, constructorParameterTypes, null);

                AssertionHelper.Explain(() => 
                    Assert.IsNotNull(constructor), 
                    innerFailures => new AssertionFailureBuilder(String.Format(
                        "Expected the exception type to have a standard constructor with signature .ctor({0}).", constructorSignature))
                        .AddRawLabeledValue("Exception Type", typeof(TException))
                        .SetStackTrace(Context.GetStackTraceData())
                        .AddInnerFailures(innerFailures)
                        .ToAssertionFailure());

                foreach (var constructorArgumentList in constructorArgumentLists)
                {
                    TException instance = (TException) constructor.Invoke(constructorArgumentList);
                    string message = constructorArgumentList.Length > 0 ? (string) constructorArgumentList[0] : null;
                    Exception innerException = constructorArgumentList.Length > 1 ? (Exception) constructorArgumentList[1] : null;

                    Assert.Multiple(() =>
                    {
                        AssertionHelper.Explain(() => 
                            Assert.AreSame(innerException, instance.InnerException), 
                            innerFailures => new AssertionFailureBuilder(
                                "The inner exception should be referentially identical to the exception provided in the constructor.")
                                .AddRawLabeledValue("Exception Type", typeof (TException))
                                .AddRawLabeledValue("Actual Inner Exception", instance.InnerException)
                                .AddRawLabeledValue("Expected Inner Exception", innerException)
                                .SetStackTrace(Context.GetStackTraceData())
                                .AddInnerFailures(innerFailures)
                                .ToAssertionFailure());

                        if (message == null)
                        {
                            AssertionHelper.Explain(() =>
                                Assert.IsNotNull(instance.Message), 
                                innerFailures => new AssertionFailureBuilder(
                                    "The exception message should not be null.")
                                    .AddRawLabeledValue("Exception Type", typeof (TException))
                                    .SetStackTrace(Context.GetStackTraceData())
                                    .AddInnerFailures(innerFailures)
                                    .ToAssertionFailure());
                        }
                        else
                        {
                            AssertionHelper.Explain(() =>
                                Assert.AreEqual(message, instance.Message),
                                innerFailures => new AssertionFailureBuilder(
                                    "Expected the exception message to be equal to a specific text.")
                                    .AddRawLabeledValue("Exception Type", typeof (TException))
                                    .AddLabeledValue("Actual Message", instance.Message)
                                    .AddLabeledValue("Expected Message", message)
                                    .SetStackTrace(Context.GetStackTraceData())
                                    .AddInnerFailures(innerFailures)
                                    .ToAssertionFailure());
                        }

                        if (ImplementsSerialization)
                        {
                            AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(instance);
                        }
                    });
                }
            });
        }

        /// <summary>
        /// Verifies that the <see cref="Exception.Message" /> and 
        /// <see cref="Exception.InnerException" /> properties are preserved by round-trip serialization.
        /// </summary>
        /// <param name="instance">The exception instance.</param>
        private void AssertMessageAndInnerExceptionPreservedByRoundTripSerialization(Exception instance)
        {
            Exception result = RoundTripSerialize(instance);

            AssertionHelper.Explain(() =>
                Assert.AreEqual(result.Message, instance.Message),
                innerFailures => new AssertionFailureBuilder(
                    "Expected the exception message to be preserved by round-trip serialization.")
                    .AddRawLabeledValue("Exception Type", typeof(TException))
                    .AddLabeledValue("Expected Message", instance.Message)
                    .AddLabeledValue("Actual Message ", result.Message)
                    .SetStackTrace(Context.GetStackTraceData())
                    .AddInnerFailures(innerFailures)
                    .ToAssertionFailure());

            AssertionHelper.Verify(() =>
            {
                if (result.InnerException == null && instance.InnerException == null)
                    return null;
                if (result.InnerException != null && instance.InnerException != null
                    && result.InnerException.GetType() == instance.InnerException.GetType()
                    && result.InnerException.Message == instance.InnerException.Message)
                    return null;

                return new AssertionFailureBuilder("The inner exception should be preserved by round-trip serialization.")
                    .AddRawLabeledValue("Exception Type", typeof(TException))
                    .AddRawLabeledValue("Actual Inner Exception", instance.InnerException)
                    .AddRawLabeledValue("Expected Inner Exception", result.InnerException)
                    .SetStackTrace(Context.GetStackTraceData())
                    .ToAssertionFailure();
            });
        }

        /// <summary>
        /// Performs round-trip serialization of the exception and returns the result.
        /// </summary>
        /// <param name="instance">The exception instance.</param>
        /// <returns>The instance produced after serialization and deserialization.</returns>
        protected static Exception RoundTripSerialize(Exception instance)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, instance);
                stream.Position = 0;
                return (Exception)formatter.Deserialize(stream);
            }
        }
    }
}
