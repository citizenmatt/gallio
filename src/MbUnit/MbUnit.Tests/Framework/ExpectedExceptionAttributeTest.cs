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
using Gallio.Common.Markup;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(ExpectedExceptionAttribute))]
    [RunSample(typeof(ExpectedExceptionSample))]
    [RunSample(typeof(ExpectedInnerExceptionSample))]
    public class ExpectedExceptionAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row("ExactException", true, null)]
        [Row("SubclassException", true, null)]
        [Row("SubstringExceptionMessage", true, null)]
        [Row("WrongExceptionType", false, "Expected an exception of type 'System.InvalidOperationException' but a different exception was thrown.")]
        [Row("WrongExceptionMessage", false, "Expected an exception of type 'System.ArgumentNullException' with message substring 'message' but a different exception was thrown.")]
        [Row("NoExceptionThrown", false, "Expected an exception of type 'System.ArgumentNullException' but none was thrown.")]
        [Row("NoExceptionExpected", false, "Execute")]
        public void ExpectedExceptionOutcome(string testMethodName, bool success, string expectedLogOutput)
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(ExpectedExceptionSample).GetMethod(testMethodName)));

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(run);
                Assert.AreEqual(success ? TestOutcome.Passed : TestOutcome.Failed, run.Result.Outcome);

                if (expectedLogOutput != null)
                    AssertLogContains(run, expectedLogOutput, MarkupStreamNames.Failures);
            });
        }

        [Test]
        [Row("ExactInnerException", true, null)]
        [Row("SubclassInnerException", true, null)]
        [Row("WrongInnerExceptionType", false, "Expected an inner exception of type 'System.InvalidOperationException' but an exception with a different type of inner exception was thrown.")]
        [Row("NoInnerException", false, "Expected an inner exception of type 'System.ArgumentException' but an exception without an inner exception was thrown.")]
        [Row("NoInnerExceptionExpected", true, null)]
        public void ExpectedInnerExceptionOutcome(string testMethodName, bool success, string expectedLogOutput)
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(ExpectedInnerExceptionSample).GetMethod(testMethodName)));

            Assert.IsNotNull(run);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(success ? TestOutcome.Passed : TestOutcome.Failed, run.Result.Outcome);

                if (expectedLogOutput != null)
                    AssertLogContains(run, expectedLogOutput, MarkupStreamNames.Failures);
            });
        }

        [Explicit("Sample")]
        public class ExpectedExceptionSample
        {
            [Test]
            [ExpectedException(typeof(ArgumentNullException))]
            public void ExactException()
            {
                throw new ArgumentNullException();
            }

            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void SubclassException()
            {
                throw new ArgumentNullException();
            }

            [Test]
            [ExpectedException(typeof(ArgumentNullException), "message")]
            public void SubstringExceptionMessage()
            {
                throw new ArgumentNullException("the message");
            }

            [Test]
            [ExpectedException(typeof(InvalidOperationException))]
            public void WrongExceptionType()
            {
                throw new ArgumentNullException();
            }

            [Test]
            [ExpectedException(typeof(ArgumentNullException), "message")]
            public void WrongExceptionMessage()
            {
                throw new ArgumentNullException("different");
            }

            [Test]
            [ExpectedException(typeof(ArgumentNullException))]
            public void NoExceptionThrown()
            {
            }

            [Test]
            public void NoExceptionExpected()
            {
                throw new ArgumentNullException();
            }
        }

        [Explicit("Sample")]
        public class ExpectedInnerExceptionSample
        {
            [Test]
            [ExpectedException(typeof(ApplicationException), null, typeof(ArgumentException))]
            public void ExactInnerException()
            {
                throw new ApplicationException("not important", new ArgumentException());
            }

            [Test]
            [ExpectedException(typeof(ApplicationException), null, InnerExceptionType = typeof(ArgumentException))]
            public void SubclassInnerException()
            {
                throw new ApplicationException("not important", new ArgumentNullException());
            }

            [Test]
            [ExpectedException(typeof(ApplicationException), null, typeof(InvalidOperationException))]
            public void WrongInnerExceptionType()
            {
                throw new ApplicationException("not important", new ArgumentNullException());
            }

            [Test]
            [ExpectedException(typeof(ApplicationException), null, typeof(ArgumentException))]
            public void NoInnerException()
            {
                throw new ApplicationException();
            }

            [Test]
            [ExpectedException(typeof(ArgumentNullException))]
            public void NoInnerExceptionExpected()
            {
                throw new ArgumentNullException("", new InvalidOperationException());
            }
        }
    }
}
