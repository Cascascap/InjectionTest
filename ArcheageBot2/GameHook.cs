using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ArcheageBot2
{
    public class GameHook
    {
        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int VK_I = 0x49;

        static IntPtr FindWindowStartingWithTitle(string titleStart)
        {
            IntPtr hWnd = IntPtr.Zero;

            EnumWindows((hWndEnum, lParam) =>
            {
                StringBuilder windowText = new StringBuilder(256);
                GetWindowText(hWndEnum, windowText, 256);

                if (windowText.ToString().StartsWith(titleStart))
                {
                    hWnd = hWndEnum;
                    return false; // Stop enumeration
                }

                return true; // Continue enumeration
            }, IntPtr.Zero);

            return hWnd;
        }

        public static void Start()
        {
            // Replace with the correct starting part of the window name for the game
            string windowTitleStart = "- ArcheAge DX11";
            IntPtr hWnd = FindWindowStartingWithTitle(windowTitleStart);

            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine("Game window not found!");
                return;
            }

            // Simulate pressing the "I" key
            PostMessage(hWnd, WM_KEYDOWN, VK_I, 0);
            PostMessage(hWnd, WM_KEYUP, VK_I, 0);

            Console.WriteLine("Sent 'I' key press to game window!");
        }

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
    }
}
