using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;


namespace RATitle
{
    class Program
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);


        static void Main(string[] args)
        {
            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    if (process.MainWindowTitle.Contains("Windows Remote Assistance"))
                    {
                        Console.WriteLine("Process:  {0} ID: {1} Window title: {2}", process.ProcessName, process.Id, process.MainWindowTitle);
                        string wmiQuery = string.Format("select CommandLine from Win32_Process where ProcessId='{0}'", process.Id);
                        ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery);
                        ManagementObjectCollection retObjectCollection = searcher.Get();
                        foreach (ManagementObject retObject in retObjectCollection)
                        {
                            String pc = retObject["CommandLine"].ToString();
                            int pos = pc.LastIndexOf(" ") + 1;
                            pc = pc.Substring(pos, pc.Length - pos).ToUpper();

                            Console.WriteLine("[{0}]", pc);
                            if (!process.MainWindowTitle.ToString().EndsWith(pc))
                            {
                                SetWindowText(process.MainWindowHandle, String.Format(process.MainWindowTitle.ToString() + " - {0}", pc));
                            }



                        }

                    }

                }
            }

        }
    }
}
