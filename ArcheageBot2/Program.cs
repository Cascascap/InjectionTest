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

namespace ArcheageBot2
{
    static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

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

            string windowTitleStart = "- ArcheAge DX11";
            //windowTitleStart = "*new 7 - Notepad++";
            windowTitleStart = "#bots |";
            /*
            Dictionary<IntPtr, string> handlesAndTitles = GetAllWindowHandlesAndTitles();
            foreach (var item in handlesAndTitles)
            {
                Console.WriteLine($"Handle: {item.Key}, Title: {item.Value}");
            }
            */
            IntPtr hWnd = FindWindowStartingWithTitle(windowTitleStart);

            Int32 targetPID = 0;
            string targetExe = null;

            // Will contain the name of the IPC server channel
            string channelName = null;

            // Create the IPC server using the FileMonitorIPC.ServiceInterface class as a singleton
            EasyHook.RemoteHooking.IpcCreateServer<ArcheageLibrary.ServerInterface>(ref channelName, System.Runtime.Remoting.WellKnownObjectMode.Singleton);


            uint processId;
            GetWindowThreadProcessId(hWnd, out processId);
            targetPID = (int)processId;
            


            // Get the full path to the assembly we want to inject into the target process
            string injectionLibrary = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "ArcheageLibrary.dll");

            try
            {
                // Injecting into existing process by Id
                if (processId > 0)
                {
                    Console.WriteLine("Attempting to inject into process {0}", targetPID);

                    // inject into existing process
                    EasyHook.RemoteHooking.Inject(
                        targetPID,          // ID of process to inject into
                        injectionLibrary,   // 32-bit library to inject (if target is 32-bit)
                        injectionLibrary,   // 64-bit library to inject (if target is 64-bit)
                        channelName         // the parameters to pass into injected library
                                            // ...
                    );
                }
                // Create a new process and then inject into it
                else if (!string.IsNullOrEmpty(targetExe))
                {
                    Console.WriteLine("Attempting to create and inject into {0}", targetExe);
                    // start and inject into a new process
                    EasyHook.RemoteHooking.CreateAndInject(
                        targetExe,          // executable to run
                        "",                 // command line arguments for target
                        0,                  // additional process creation flags to pass to CreateProcess
                        EasyHook.InjectionOptions.DoNotRequireStrongName, // allow injectionLibrary to be unsigned
                        injectionLibrary,   // 32-bit library to inject (if target is 32-bit)
                        injectionLibrary,   // 64-bit library to inject (if target is 64-bit)
                        out targetPID,      // retrieve the newly created process ID
                        channelName         // the parameters to pass into injected library
                                            // ...
                    );
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("There was an error while injecting into target:");
                Console.ResetColor();
                Console.WriteLine(e.ToString());
            }

           Application.Run(new Form1());
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
