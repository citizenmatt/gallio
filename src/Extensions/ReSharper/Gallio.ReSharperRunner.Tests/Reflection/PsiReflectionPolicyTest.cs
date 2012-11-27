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

using Gallio.Common.Reflection;
using Gallio.Model.Tree;
using Gallio.ReSharperRunner.Reflection;
using Gallio.Tests.Common.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using MbUnit.Framework;
using System.Reflection;
using MbUnit.TestResources;
using Test=Gallio.Model.Tree.Test;

namespace Gallio.ReSharperRunner.Tests.Reflection
{
    [TestFixture]
    [TestsOn(typeof(PsiReflectionPolicy))]
#if !RESHARPER_31
    [RunWithGuardedReadLock]
#endif
    [Category("Integration")]
    public class PsiReflectionPolicyTest : BaseReflectionPolicyTest
    {
        private PsiReflectionPolicy reflectionPolicy;

        [FixtureSetUp]
        public void TestFixtureSetUp()
        {
            ReSharperTestHarness.LoadTestSolutionIfNeeded();
        }

        public override void SetUp()
        {
            base.SetUp();
            WrapperAssert.SupportsSpecialFeatures = false;
            WrapperAssert.SupportsSpecialName = false;
            WrapperAssert.SupportsCallingConventions = false;
            WrapperAssert.SupportsReturnAttributes = false;
            WrapperAssert.SupportsEventFields = false;
            WrapperAssert.SupportsGenericParameterAttributes = false;
            WrapperAssert.SupportsFinalizers = false;
            WrapperAssert.SupportsStaticConstructors = false; // compiler generated static constructors introduced by static readonly fields are not supported

            PsiManager manager = PsiManager.GetInstance(SolutionManager.Instance.CurrentSolution);

            reflectionPolicy = new PsiReflectionPolicy(manager);
        }

        protected override IReflectionPolicy ReflectionPolicy
        {
            get { return reflectionPolicy; }
        }

        [Test]
        public void WrapNullReturnsNull()
        {
            Assert.IsNull(reflectionPolicy.Wrap((IDeclaredElement)null));
            Assert.IsNull(reflectionPolicy.Wrap((IEvent)null));
            Assert.IsNull(reflectionPolicy.Wrap((IField)null));
            Assert.IsNull(reflectionPolicy.Wrap((IFunction)null));
            Assert.IsNull(reflectionPolicy.Wrap((IConstructor)null));
            Assert.IsNull(reflectionPolicy.Wrap((IMethod)null));
            Assert.IsNull(reflectionPolicy.Wrap((IOperator)null));
            Assert.IsNull(reflectionPolicy.Wrap((IParameter)null));
            Assert.IsNull(reflectionPolicy.Wrap((IProperty)null));
            Assert.IsNull(reflectionPolicy.Wrap((ITypeElement)null));
            Assert.IsNull(reflectionPolicy.Wrap((ITypeParameter)null));
        }

        [Test, Description("Other tests exercise Psi project modules, this one checks Psi assembly modules.")]
        public void AssemblyWrapperForPsiAssemblyModules()
        {
            Assembly target = typeof(SimpleTest).Assembly;
            IAssemblyInfo info = GetAssembly(target);

            WrapperAssert.AreEquivalent(target, info, false);
        }

        [Test, Description("Other tests exercise Psi project modules, this one checks Psi assembly modules.")]
        public void AssemblyWrapperForPsiAssemblyModules_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<Assembly, IAssemblyInfo>(
                typeof(SimpleTest).Assembly,
                typeof(Test).Assembly,
                GetAssembly);
        }
    }
}