using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WindowsLiveIDClientSample
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Application.Run(new MainWindow());
            }
            //System requirement detection.
            catch (System.IO.FileNotFoundException fnfex)
            {
                //Checking for the absence of the Windows Live Sign-In Assistant DLL.
                if (fnfex.Message.Contains("WindowsLive.ID.Client"))
                {
                    MessageBox.Show("Please install the Windows Live ID For Client Applications SDK.");
                }
                else
                {
                    MessageBox.Show(fnfex.Message);
                }
            }
            finally
            {
                Application.Exit();
            }
        }
    }
}