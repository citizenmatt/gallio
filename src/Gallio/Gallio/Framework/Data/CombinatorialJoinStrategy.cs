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

using System.Collections.Generic;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// The combinatorial join strategy combines items by constructing the cartesian
    /// product of the items of each provider.
    /// </summary>
    /// <seealso cref="PairwiseJoinStrategy"/>
    public sealed class CombinatorialJoinStrategy : IJoinStrategy
    {
        /// <summary>
        /// Gets the singleton instance of the strategy.
        /// </summary>
        public static readonly CombinatorialJoinStrategy Instance = new CombinatorialJoinStrategy();

        private CombinatorialJoinStrategy()
        {
        }

        /// <inheritdoc />
        public IEnumerable<IList<IDataItem>> Join(IList<IDataProvider> providers, IList<ICollection<DataBinding>> bindingsPerProvider,
            bool includeDynamicItems)
        {
            int providerCount = providers.Count;
            if (providerCount == 0)
                yield break;

            IEnumerable<IDataItem>[] sequences = new IEnumerable<IDataItem>[providers.Count];
            for (int i = 0; i < providers.Count; i++)
                sequences[i] = providers[i].GetItems(bindingsPerProvider[i], includeDynamicItems);

            IEnumerator<IDataItem>[] enumerators = new IEnumerator<IDataItem>[providerCount];
            enumerators[0] = sequences[0].GetEnumerator();

            int enumeratorCount = 1;
            for (; ; )
            {
                IEnumerator<IDataItem> top = enumerators[enumeratorCount - 1];

                if (top.MoveNext())
                {
                    if (enumeratorCount < providerCount)
                    {
                        enumerators[enumeratorCount] = sequences[enumeratorCount].GetEnumerator();
                        enumeratorCount += 1;
                    }
                    else
                    {
                        IDataItem[] itemList = new IDataItem[providerCount];
                        for (int i = 0; i < providerCount; i++)
                            itemList[i] = enumerators[i].Current;

                        yield return itemList;
                    }
                }
                else
                {
                    enumeratorCount -= 1;
                    enumerators[enumeratorCount] = null;

                    if (enumeratorCount == 0)
                        break;
                }
            }
        }
    }
}
