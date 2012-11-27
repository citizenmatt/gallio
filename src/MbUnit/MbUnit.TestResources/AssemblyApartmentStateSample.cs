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
using System.Text;
using System.Threading;
using Gallio.Framework;
using MbUnit.Framework;

// Ordinarily the default is STA, we change it to MTA here for testing purposes.
[assembly: ApartmentState(System.Threading.ApartmentState.MTA)]

namespace MbUnit.TestResources
{
    public class AssemblyApartmentStateSample
    {
        // This is checked by ApartmentStateTest in MbUnit.Tests.
        [Test]
        public void WriteApartmentStateToLog()
        {
            TestLog.Write(Thread.CurrentThread.GetApartmentState());
        }
    }
}
