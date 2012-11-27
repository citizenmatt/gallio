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

using Gallio.Common.Markup;
using Gallio.Common.Markup.Tags;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Common.Markup.Tags
{
    public class MarkerTagTest : BaseTagTest<MarkerTag>
    {
        [VerifyContract]
        public readonly IContract EqualityTests = new EqualityContract<MarkerTag>()
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses = equivalenceClasses
        };
    
        public override EquivalenceClassCollection GetEquivalenceClasses()
        {
            return equivalenceClasses;
        }

        private static readonly EquivalenceClassCollection equivalenceClasses = new EquivalenceClassCollection
        {
            new MarkerTag(Marker.AssertionFailure),
            new MarkerTag(Marker.Highlight),
            new MarkerTag(Marker.AssertionFailure.WithAttribute("x", "y")),
            new MarkerTag(Marker.Highlight) { Contents = { new TextTag("text") }},
            new MarkerTag(Marker.Highlight) { Contents = { new TextTag("text"), new TextTag("more") }}
        };
    }
}
