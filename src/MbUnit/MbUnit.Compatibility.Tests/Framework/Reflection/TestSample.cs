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

#pragma warning disable 0618

namespace MbUnit.Compatibility.Tests.Framework.Reflection
{
    public class TestSample : BaseSample
    {
        public string publicString = "MbUnit Rocks!!!";
        private DateTime privateDateTime = DateTime.Today;
        internal static int staticNum = 7;

        public string PublicProperty
        {
            get { return publicString; }
            set { publicString = value; }
        }

        internal DateTime InternalProperty
        {
            get { return privateDateTime; }
            set { privateDateTime = value; }
        }

        protected static int StaticProperty
        {
            get { return staticNum; }
            set { staticNum = value; }
        }

        public int Pow(int x)
        {
            return Multiply(x, x);
        }

        public string PraiseMe()
        {
            return publicString;
        }

        private int Multiply(int x, int y)
        {
            return x * y;
        }

        public static int Add(int x, int y)
        {
            return x + y;
        }
    }
}