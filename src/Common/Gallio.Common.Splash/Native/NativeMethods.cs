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
    unsafe internal static class NativeMethods
    {
        [DllImport("usp10.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int ScriptItemize(
            [In] char* pwcInChars,
            [In] int cInChars,
            [In] int cMaxItems,
            [In] SCRIPT_CONTROL* psControl,
            [In] SCRIPT_STATE* psState,
            [Out] SCRIPT_ITEM* pItems,
            [Out] out int pcItems);

        [DllImport("usp10.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int ScriptBreak(
            [In] char* pwcChars,
            [In] int cChars,
            [In] SCRIPT_ANALYSIS* psa,
            [Out] SCRIPT_LOGATTR* psla);

        [DllImport("usp10.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int ScriptLayout(
            [In] int cRuns,
            [In] byte* pbLevel,
            [Out] int* piVisualToLogical,
            [Out] int* piLogicalToVisual);

        [DllImport("usp10.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int ScriptShape(
            [In] IntPtr hdc,
            [In, Out] ref IntPtr psc,
            [In] char* pwcChars,
            [In] int cChars,
            [In] int cMaxGlyphs,
            [Out] SCRIPT_ANALYSIS* psa,
            [Out] ushort* pwOutGlyphs,
            [Out] ushort* pwLogClust,
            [Out] SCRIPT_VISATTR* psva,
            [Out] out int pcGlyphs);

        [DllImport("usp10.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int ScriptPlace(
            [In] IntPtr hdc,
            [In, Out] ref IntPtr psc,
            [In] ushort* pwGlyphs,
            [In] int cGlyphs,
            [In] SCRIPT_VISATTR* psva,
            [In, Out] SCRIPT_ANALYSIS* psa,
            [Out] int* piAdvance,
            [Out] GOFFSET* pGoffset,
            [Out] out ABC pABC);

        [DllImport("usp10.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int ScriptCPtoX(
            [In] int iCP,
            [In] bool fTrailing,
            [In] int cChars,
            [In] int cGlyphs,
            [In] ushort* pwLogClust,
            [In] SCRIPT_VISATTR* psva,
            [In] int* piAdvance,
            [In] SCRIPT_ANALYSIS* psa,
            [Out] out int piX);

        [DllImport("usp10.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int ScriptXtoCP(
            [In] int iX,
            [In] int cChars,
            [In] int cGlyphs,
            [In] ushort* pwLogClust,
            [In] SCRIPT_VISATTR* psva,
            [In] int* piAdvance,
            [In] SCRIPT_ANALYSIS* psa,
            [Out] out int piCP,
            [Out] out int piTrailing);

        [DllImport("usp10.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int ScriptTextOut(
            [In] IntPtr hdc,
            [In, Out] ref IntPtr psc,
            [In] int x,
            [In] int y,
            [In] ExtTextOutOptions fuOptions,
            [In] RECT* lprc,
            [In] SCRIPT_ANALYSIS* psa,
            [In] char* pwcReserved,
            [In] int iReserved,
            [In] ushort* pwGlyphs,
            [In] int cGlyphs,
            [In] int* piAdvance,
            [In] int* piJustify,
            [In] GOFFSET* pGoffset);

        [DllImport("usp10.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int ScriptCacheGetHeight(
            [In] IntPtr hdc,
            [In, Out] ref IntPtr psc,
            [Out] out int tmHeight);

        [DllImport("usp10.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        public static extern int ScriptFreeCache(
            [In, Out] ref IntPtr psc);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr GetDC([In] IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int ReleaseDC([In] IntPtr hWnd, [In] IntPtr hDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr SelectObject([In] IntPtr hDC, [In] IntPtr hObject);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern void SetBkMode([In] IntPtr hDC, [In] int iBkMode);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetTextMetrics([In] IntPtr hdc, [Out] out TEXTMETRIC textMetric);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int SetDCPenColor([In] IntPtr hdc, [In] int crColor);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int SetDCBrushColor([In] IntPtr hdc, [In] int crColor);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr GetStockObject([In] int fnObject);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int SetTextColor([In] IntPtr hdc, [In] int crColor);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr CreateRectRgn([In] int nLeftRect, [In] int nTopRect, [In] int nRightRect, [In] int nBottomRect);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetClipRgn([In] IntPtr hdc, [In] IntPtr hrgn);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int SelectClipRgn([In] IntPtr hdc, [In] IntPtr hrgn);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int ExcludeClipRect([In] IntPtr hdc, [In] int nLeftRect, [In] int nTopRect, [In] int nRightRect, [In] int nBottomRect);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int ExtSelectClipRgn([In] IntPtr hdc, [In] IntPtr hrgn, [In] int fnMode);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int FillRect([In] IntPtr hdc, [In] ref RECT rect, [In] IntPtr hBrush);
    }
}
