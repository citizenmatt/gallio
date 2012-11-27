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
using System.Linq;
using System.Text;
using System.Threading;
using Gallio.Common.Concurrency;
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Concurrency
{
    [TestsOn(typeof(LockBox<>))]
    public class LockBoxTest
    {
        [Test]
        public void Read_WhenActionIsNull_Throws()
        {
            LockBox<int> box = new LockBox<int>(11);

            Assert.Throws<ArgumentNullException>(() => box.Read(null));
        }

        [Test]
        public void Read_WhenFuncIsNull_Throws()
        {
            LockBox<int> box = new LockBox<int>(11);

            Assert.Throws<ArgumentNullException>(() => box.Read<object>(null));
        }

        [Test]
        public void Write_WhenActionIsNull_Throws()
        {
            LockBox<int> box = new LockBox<int>(11);

            Assert.Throws<ArgumentNullException>(() => box.Write(null));
        }

        [Test]
        public void Write_WhenFuncIsNull_Throws()
        {
            LockBox<int> box = new LockBox<int>(11);

            Assert.Throws<ArgumentNullException>(() => box.Write<object>(null));
        }

        [Test]
        public void Read_WithAction_ProvidesContents()
        {
            LockBox<int> box = new LockBox<int>(11);

            int actualValue = 0;
            box.Read(value => actualValue = value);
            Assert.AreEqual(11, actualValue);
        }

        [Test]
        public void Write_WithAction_ProvidesContents()
        {
            LockBox<int> box = new LockBox<int>(11);

            int actualValue = 0;
            box.Write(value => actualValue = value);
            Assert.AreEqual(11, actualValue);
        }

        [Test]
        public void Read_WithFunc_ProvidesContents()
        {
            LockBox<int> box = new LockBox<int>(11);

            int actualValue = box.Read(value => value);
            Assert.AreEqual(11, actualValue);
        }

        [Test]
        public void Write_WithFunc_ProvidesContents()
        {
            LockBox<int> box = new LockBox<int>(11);

            int actualValue = box.Write(value => value);
            Assert.AreEqual(11, actualValue);
        }

        [Test]
        public void ReadersAndWritersDoNotInterfere()
        {
            LockBox<StringBuilder> box = new LockBox<StringBuilder>(new StringBuilder());

            int writeCount = 0;
            bool done = false;
            for (int i = 0; i < 10; i++)
            {
                if (i % 3 != 0)
                {
                    Tasks.StartThreadTask("Reader", () =>
                    {
                        while (! done)
                        {
                            box.Read(value => Assert.IsTrue(value.Length%2 == 0));
                            Thread.Sleep(0);
                        }
                    });
                }
                else
                {
                    Tasks.StartThreadTask("Writer", () =>
                    {
                        while (!done)
                        {
                            box.Write(value =>
                            {
                                Assert.IsTrue(value.Length%2 == 0);
                                value.Append('x');

                                Thread.Sleep(1);

                                Assert.IsTrue(value.Length%2 == 1);
                                value.Append('x');

                                writeCount += 1;
                            });
                        }
                    });
                }
            }

            Thread.Sleep(200);
            done = true;
            Tasks.JoinAndVerify(new TimeSpan(0, 0, 1));

            box.Read(value => Assert.AreEqual(writeCount * 2, value.Length));
        }
    }
}
