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

using Gallio.Framework.Data;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(BaseDataSet))]
    public class BaseDataSetTest : BaseTestWithMocks
    {
        [Test, ExpectedArgumentNullException]
        public void CanBindThrowsIfBindingIsNull()
        {
            BaseDataSet dataSet = Mocks.PartialMock<BaseDataSet>();
            dataSet.CanBind(null);
        }

        [Test, ExpectedArgumentNullException]
        public void GetItemsThrowsIfBindingListIsNull()
        {
            BaseDataSet dataSet = Mocks.PartialMock<BaseDataSet>();
            dataSet.GetItems(null, false);
        }
    }
}