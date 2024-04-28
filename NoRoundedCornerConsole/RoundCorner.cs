using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ConsoleApp2
{
    internal class RoundCorner
    {

        #region API
        // The enum flag for DwmSetWindowAttribute's second parameter, which tells the function what attribute to set.
        // Copied from dwmapi.h
        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }

        // The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
        // what value of the enum to set.
        // Copied from dwmapi.h
        public enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        // Import dwmapi.dll and define DwmSetWindowAttribute in C# corresponding to the native function.
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void DwmSetWindowAttribute(IntPtr hwnd,
                                                         DWMWINDOWATTRIBUTE attribute,
                                                         ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
                                                         uint cbAttribute);

        #endregion

        private List<IntPtr> knownHandles { get; set; }
        private bool ShowDebugMessages { get; set; }


        internal RoundCorner(bool debug = false)
        {
            this.ShowDebugMessages = debug;
            this.knownHandles = new List<IntPtr>();

            do
            {
                LoopAllWindows();
                System.Threading.Thread.Sleep(300);

            } while (true);

        }

        private void SetRoundedCorner(bool on, IntPtr hwnd)
        {
            var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
            var preference = (on == true) ? DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND : DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND;
            DwmSetWindowAttribute(hwnd, attribute, ref preference, sizeof(uint));
        }

        private void LoopAllWindows()
        {
            //create new known process list
            List<IntPtr> newKnownHandles = new List<IntPtr>();

            foreach (KeyValuePair<IntPtr, string> window in OpenWindowGetter.GetOpenWindows())
            {

                if (!knownHandles.Contains(window.Key)) //unknown window
                {
                    try
                    {
                       if(ShowDebugMessages) Console.WriteLine($"Set corner for window: {window.Value} ({window.Key})");
                        SetRoundedCorner(false, window.Key);
                    }
                    catch (Exception ex)
                    {
                        if (ShowDebugMessages) Console.WriteLine($"Error while setting window for {window.Value}: {ex.Message}");
                    }

                }

                newKnownHandles.Add(window.Key);

            }

            knownHandles= newKnownHandles;




        }


        /// <summary>Contains functionality to get all the open windows.</summary>
        private static class OpenWindowGetter
        {
            /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
            /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
            public static IDictionary<IntPtr, string> GetOpenWindows()
            {
                IntPtr shellWindow = GetShellWindow();
                Dictionary<IntPtr, string> windows = new Dictionary<IntPtr, string>();

                EnumWindows(delegate (IntPtr hWnd, int lParam)
                {
                    if (hWnd == shellWindow) return true;
                    if (!IsWindowVisible(hWnd)) return true;

                    int length = GetWindowTextLength(hWnd);
                    if (length == 0) return true;

                    StringBuilder builder = new StringBuilder(length);
                    GetWindowText(hWnd, builder, length + 1);

                    windows[hWnd] = builder.ToString();
                    return true;

                }, 0);

                return windows;
            }

            private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

            [DllImport("USER32.DLL")]
            private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

            [DllImport("USER32.DLL")]
            private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

            [DllImport("USER32.DLL")]
            private static extern int GetWindowTextLength(IntPtr hWnd);

            [DllImport("USER32.DLL")]
            private static extern bool IsWindowVisible(IntPtr hWnd);

            [DllImport("USER32.DLL")]
            private static extern IntPtr GetShellWindow();
        }



    }
}
