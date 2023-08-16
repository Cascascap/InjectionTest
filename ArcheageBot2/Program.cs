using EasyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ArcheageBot2.GameHook;

namespace ArcheageBot2
{
    static class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, IntPtr szTitle);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            GameHook.Start();

            string windowTitleStart = "- ArcheAge DX11";
            /*
            Dictionary<IntPtr, string> handlesAndTitles = GetAllWindowHandlesAndTitles();
            foreach (var item in handlesAndTitles)
            {
                Console.WriteLine($"Handle: {item.Key}, Title: {item.Value}");
            }
            */

            IntPtr hWnd = FindWindowStartingWithTitle(windowTitleStart);

            IpcChannel channel = new IpcChannel(windowTitleStart);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(RemoteServer),
                windowTitleStart,
                WellKnownObjectMode.Singleton);


            uint processId;
            GetWindowThreadProcessId(hWnd, out processId);
            Process gameProcess = Process.GetProcessById((int)processId);

            // Inject the remote server into the game client
            string injectionLibraryPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ArcheageLibrary.dll");
            try
            {
                RemoteHooking.Inject(gameProcess.Id, injectionLibraryPath, null);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Remote server injected into game client!");
        }


        public static Dictionary<IntPtr, string> GetAllWindowHandlesAndTitles()
        {
            Dictionary<IntPtr, string> windowHandlesAndTitles = new Dictionary<IntPtr, string>();

            EnumWindows((hWnd, lParam) =>
            {
                StringBuilder windowTitle = new StringBuilder(256);
                GetWindowText(hWnd, windowTitle, windowTitle.Capacity);
                windowHandlesAndTitles.Add(hWnd, windowTitle.ToString());
                return true; // Continue enumeration
            }, IntPtr.Zero);

            return windowHandlesAndTitles;
        }

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
    }
}
