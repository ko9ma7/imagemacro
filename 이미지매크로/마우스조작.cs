using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace 이미지매크로
{
    public static class 마우스조작
    {
        // Inactive 방식 (PostMessage 사용)
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN_POST = 0x0201;
        private const int WM_LBUTTONUP_POST = 0x0202;

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private static int MAKELPARAM(int lo, int hi) => ((hi << 16) | (lo & 0xffff));

        public static void 비활성클릭_PostMessage(IntPtr hwnd, MouseButtons btn, int x, int y)
        {
            int p = MAKELPARAM(x, y);
            PostMessage(hwnd, WM_MOUSEMOVE, IntPtr.Zero, (IntPtr)p);
            Thread.Sleep(30);
            if (btn == MouseButtons.Left)
            {
                PostMessage(hwnd, WM_LBUTTONDOWN_POST, (IntPtr)1, (IntPtr)p);
                Thread.Sleep(50);
                PostMessage(hwnd, WM_LBUTTONUP_POST, IntPtr.Zero, (IntPtr)p);
            }
        }

        // Active 방식 (mouse_event 사용)
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, UIntPtr extra);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x20;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x40;
        private const uint MOUSEEVENTF_WHEEL = 0x0800;

        public static void 활성클릭(int sx, int sy, MouseButtons btn)
        {
            Cursor.Position = new Point(sx, sy);
            Thread.Sleep(50);
            switch (btn)
            {
                case MouseButtons.Left:
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                    Thread.Sleep(50);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
                    break;
                case MouseButtons.Right:
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
                    Thread.Sleep(50);
                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
                    break;
                case MouseButtons.Middle:
                    mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, UIntPtr.Zero);
                    Thread.Sleep(50);
                    mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, UIntPtr.Zero);
                    break;
            }
        }

        public static void 휠(int sx, int sy, int amount)
        {
            Cursor.Position = new Point(sx, sy);
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, (uint)amount, UIntPtr.Zero);
        }

        public static void 드래그(Point st, Point ed)
        {
            Cursor.Position = st;
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            int steps = 10;
            for (int i = 1; i <= steps; i++)
            {
                int nx = st.X + (ed.X - st.X) * i / steps;
                int ny = st.Y + (ed.Y - st.Y) * i / steps;
                Cursor.Position = new Point(nx, ny);
                Thread.Sleep(40);
            }
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
        }
    }
}
