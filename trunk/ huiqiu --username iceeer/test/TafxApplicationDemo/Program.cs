using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TestAutomationFX.UI.TestRunner;

namespace TafxApplicationDemo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            TestApplication.Run(new TestFixture1());
        }
    }
}
