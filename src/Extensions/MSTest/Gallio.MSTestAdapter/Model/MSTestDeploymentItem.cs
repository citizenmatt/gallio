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

namespace Gallio.MSTestAdapter.Model
{
    /// <summary>
    /// Describes an item to be deployed by MSTest.
    /// </summary>
    internal class MSTestDeploymentItem
    {
        private readonly string sourcePath;
        private readonly string destinationPath;

        public MSTestDeploymentItem(string sourcePath, string destinationPath)
        {
            if (sourcePath == null)
                throw new ArgumentNullException("sourcePath");
            if (destinationPath == null)
                throw new ArgumentNullException("destinationPath");

            this.sourcePath = sourcePath;
            this.destinationPath = destinationPath;
        }

        public string SourcePath
        {
            get { return sourcePath; }
        }

        public string DestinationPath
        {
            get { return destinationPath; }
        }
    }
}
