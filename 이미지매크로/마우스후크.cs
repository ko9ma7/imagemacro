using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace 이미지매크로
{
    public sealed class 마우스후크 : IDisposable
    {
        private delegate IntPtr 저수준마우스콜백(int nCode, IntPtr wParam, IntPtr lParam);
        private IntPtr _후크 = IntPtr.Zero;
        private 저수준마우스콜백 _proc;

        public event MouseEventHandler? 마우스왼클릭;

        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;

        public 마우스후크()
        {
            _proc = HookFunc;
            _후크 = SetHook(_proc);
        }

        private IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
            {
                var st = Marshal.PtrToStructure<MSLLHOOK>(lParam);
                if (st is MSLLHOOK ms)
                {
                    마우스왼클릭?.Invoke(this,
                        new MouseEventArgs(MouseButtons.Left, 1, ms.pt.x, ms.pt.y, 0));
                }
            }
            return CallNextHookEx(_후크, nCode, wParam, lParam);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int x; public int y; }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOK
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private IntPtr SetHook(저수준마우스콜백 func)
        {
            using var ps = Process.GetCurrentProcess();
            using var mod = ps.MainModule!;
            return SetWindowsHookEx(WH_MOUSE_LL, func, GetModuleHandle(mod.ModuleName), 0);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, 저수준마우스콜백 lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public void Dispose()
        {
            if (_후크 != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_후크);
                _후크 = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }
}
