﻿// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Text;

namespace Gallio.Common.Xml.Paths
{
    internal class XmlPathRendererWithIndentation : XmlPathRenderer
    {
        public XmlPathRendererWithIndentation(IXmlPathStrict path, NodeFragment fragment)
            : base(path, fragment)
        {
        }

        protected override string Consolidate(List<Line> lines)
        {
            var output = new StringBuilder();
            output.AppendLine();
            int levelMax = GetLevelMax(lines);

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                var tabs = new string(' ', (levelMax - lines[i].Level) * 2);
                output.AppendLine(tabs + lines[i].Text);
            }

            return output.ToString();
        }

        private static int GetLevelMax(List<Line> lines)
        {
            int max = 0;

            foreach (Line line in lines)
            {
                if (line.Level > max)
                    max = line.Level;
            }

            return max;
        }
    }
}