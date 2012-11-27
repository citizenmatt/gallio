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

using System.Drawing;
using Gallio.Model;
using JetBrains.CommonControls;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.UI.TreeView;

#if RESHARPER_31
using JetBrains.ReSharper.TreeModelBrowser;
using JetBrains.Util.DataStructures.TreeModel;
#elif RESHARPER_40 || RESHARPER_41
using JetBrains.TreeModels;
using JetBrains.ReSharper.CodeView.TreePsiBrowser;
#else
using JetBrains.TreeModels;
using JetBrains.ReSharper.Features.Common.TreePsiBrowser;
#endif
#if RESHARPER_50_OR_NEWER
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.UI;
#endif
#if RESHARPER_60_OR_NEWER
#endif

namespace Gallio.ReSharperRunner.Provider
{
    public class GallioTestPresenter : TreeModelBrowserPresenter
    {
        public GallioTestPresenter()
        {
            Present<GallioTestElement>(PresentTestElement);
        }

        private void PresentTestElement(GallioTestElement value, IPresentableItem item,
            TreeModelNode modelNode, PresentationState state)
        {
            item.Clear();

            item.RichText = value.TestName;

#if RESHARPER_60_OR_NEWER
            if (value.Explicit)
				item.RichText.SetForeColor(SystemColors.GrayText);

			var typeImage = UnitTestIconManager.GetStandardImage(UnitTestElementImage.Test);
			var stateImage = UnitTestIconManager.GetStateImage(state);
			if (stateImage != null)
			{
				item.Images.Add(stateImage);
			}
			else if (typeImage != null)
			{
				item.Images.Add(typeImage);
			}
#else
			if (value.IsExplicit)
				item.RichText.SetForeColor(SystemColors.GrayText);

            Image image = UnitTestManager.GetStateImage(state);

            if (image == null)
                image = UnitTestManager.GetStandardImage(value.IsTestCase ? UnitTestElementImage.Test : UnitTestElementImage.TestCategory);

            if (image != null)
                item.Images.Add(image);

            if (! value.IsTestCase)
                AppendOccurencesCount(item, modelNode, "test");
#endif
		}
    }
}