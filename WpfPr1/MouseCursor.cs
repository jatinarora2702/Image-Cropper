using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace WpfPr1
{
    class MouseCursor
    {
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        public static void MoveCursor(int x, int y)
        {
            SetCursorPos(x, y);
            
        }
    }
}
