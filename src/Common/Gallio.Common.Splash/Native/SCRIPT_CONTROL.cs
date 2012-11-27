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
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Common.Splash.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SCRIPT_CONTROL
    {
        public const uint uDefaultLanguageMask    = 0x0000ffff;
        public const uint fContextDigitsMask      = 0x00010000;
        public const uint fInvertPreBoundDirMask  = 0x00020000;
        public const uint fInvertPostBoundDirMask = 0x00040000;
        public const uint fLinkStringBeforeMask   = 0x00080000;
        public const uint fLinkStringAfterMask    = 0x00100000;
        public const uint fNeutralOverrideMask    = 0x00200000;
        public const uint fNumericOverrideMask    = 0x00400000;
        public const uint fLegacyBidiClassMask    = 0x00800000;
        public const uint fMergeNeutralItemsMask  = 0x01000000;
        public const uint fReservedMask           = 0xfe000000;

        public uint BitFields;
    }
}
