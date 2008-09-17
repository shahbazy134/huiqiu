namespace TafxApplicationDemo
{
    partial class TestFixture1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestFixture1));
            this.CalcApplication = new TestAutomationFX.UI.UIApplication();
            this.计算器Window = new TestAutomationFX.UI.UIWindow();
            this.TitleBar = new TestAutomationFX.UI.UIMsaa();
            this.Button1 = new TestAutomationFX.UI.UIWindow();
            this.ButtonPlus = new TestAutomationFX.UI.UIWindow();
            this.ButtonEquals = new TestAutomationFX.UI.UIWindow();
            this.Button2 = new TestAutomationFX.UI.UIWindow();
            this.TextBox1 = new TestAutomationFX.UI.UIWindow();
            this.Button八进制 = new TestAutomationFX.UI.UIWindow();
            this.ButtonMultiply = new TestAutomationFX.UI.UIWindow();
            this.Button6 = new TestAutomationFX.UI.UIWindow();
            this.Button十进制 = new TestAutomationFX.UI.UIWindow();
            // 
            // CalcApplication
            // 
            this.CalcApplication.CommandLineArguments = null;
            this.CalcApplication.Comment = null;
            this.CalcApplication.ImagePath = "C:\\WINDOWS\\system32\\calc.exe";
            this.CalcApplication.Name = "CalcApplication";
            this.CalcApplication.ObjectImage = ((System.Drawing.Bitmap)(resources.GetObject("CalcApplication.ObjectImage")));
            this.CalcApplication.Parent = null;
            this.CalcApplication.ProcessName = "Calc";
            this.CalcApplication.TimeOut = 1000;
            this.CalcApplication.UIObjectType = TestAutomationFX.UI.UIObjectTypes.Application;
            this.CalcApplication.WorkingDirectory = "C:\\WINDOWS\\system32";
            // 
            // 计算器Window
            // 
            this.计算器Window.Comment = null;
            this.计算器Window.MatchedIndex = 0;
            this.计算器Window.MsaaRole = System.Windows.Forms.AccessibleRole.Dialog;
            this.计算器Window.Name = "计算器Window";
            this.计算器Window.ObjectImage = ((System.Drawing.Bitmap)(resources.GetObject("计算器Window.ObjectImage")));
            this.计算器Window.OwnedWindow = true;
            this.计算器Window.Parent = this.CalcApplication;
            this.计算器Window.UIObjectType = TestAutomationFX.UI.UIObjectTypes.Window;
            this.计算器Window.WindowClass = "SciCalc";
            // 
            // TitleBar
            // 
            this.TitleBar.Comment = null;
            this.TitleBar.Index = 1;
            this.TitleBar.MsaaName = null;
            this.TitleBar.Name = "TitleBar";
            this.TitleBar.ObjectImage = ((System.Drawing.Bitmap)(resources.GetObject("TitleBar.ObjectImage")));
            this.TitleBar.Parent = this.计算器Window;
            this.TitleBar.Role = System.Windows.Forms.AccessibleRole.TitleBar;
            this.TitleBar.UIObjectType = TestAutomationFX.UI.UIObjectTypes.Unknown;
            // 
            // Button1
            // 
            this.Button1.Comment = null;
            this.Button1.Index = 44;
            this.Button1.MatchedIndex = 0;
            this.Button1.MsaaRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Button1.Name = "Button1";
            this.Button1.ObjectImage = ((System.Drawing.Bitmap)(resources.GetObject("Button1.ObjectImage")));
            this.Button1.Parent = this.计算器Window;
            this.Button1.UIObjectType = TestAutomationFX.UI.UIObjectTypes.Button;
            this.Button1.WindowClass = "Button";
            this.Button1.WindowText = "1";
            // 
            // ButtonPlus
            // 
            this.ButtonPlus.Comment = null;
            this.ButtonPlus.Index = 60;
            this.ButtonPlus.MatchedIndex = 0;
            this.ButtonPlus.MsaaRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.ButtonPlus.Name = "ButtonPlus";
            this.ButtonPlus.ObjectImage = ((System.Drawing.Bitmap)(resources.GetObject("ButtonPlus.ObjectImage")));
            this.ButtonPlus.Parent = this.计算器Window;
            this.ButtonPlus.UIObjectType = TestAutomationFX.UI.UIObjectTypes.Button;
            this.ButtonPlus.WindowClass = "Button";
            this.ButtonPlus.WindowText = "+";
            // 
            // ButtonEquals
            // 
            this.ButtonEquals.Comment = null;
            this.ButtonEquals.Index = 65;
            this.ButtonEquals.MatchedIndex = 0;
            this.ButtonEquals.MsaaRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.ButtonEquals.Name = "ButtonEquals";
            this.ButtonEquals.ObjectImage = ((System.Drawing.Bitmap)(resources.GetObject("ButtonEquals.ObjectImage")));
            this.ButtonEquals.Parent = this.计算器Window;
            this.ButtonEquals.UIObjectType = TestAutomationFX.UI.UIObjectTypes.Button;
            this.ButtonEquals.WindowClass = "Button";
            this.ButtonEquals.WindowText = "=";
            // 
            // Button2
            // 
            this.Button2.Comment = null;
            this.Button2.Index = 49;
            this.Button2.MatchedIndex = 0;
            this.Button2.MsaaRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Button2.Name = "Button2";
            this.Button2.ObjectImage = ((System.Drawing.Bitmap)(resources.GetObject("Button2.ObjectImage")));
            this.Button2.Parent = this.计算器Window;
            this.Button2.UIObjectType = TestAutomationFX.UI.UIObjectTypes.Button;
            this.Button2.WindowClass = "Button";
            this.Button2.WindowText = "2";
            // 
            // TextBox1
            // 
            this.TextBox1.Comment = null;
            this.TextBox1.ControlId = 403;
            this.TextBox1.Index = 0;
            this.TextBox1.MatchedIndex = 0;
            this.TextBox1.MsaaRole = System.Windows.Forms.AccessibleRole.Text;
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.ObjectImage = ((System.Drawing.Bitmap)(resources.GetObject("TextBox1.ObjectImage")));
            this.TextBox1.Parent = this.计算器Window;
            this.TextBox1.UIObjectType = TestAutomationFX.UI.UIObjectTypes.TextBox;
            this.TextBox1.WindowClass = "Edit";
            // 
            // Button八进制
            // 
            this.Button八进制.Comment = null;
            this.Button八进制.Index = 7;
            this.Button八进制.MatchedIndex = 0;
            this.Button八进制.MsaaRole = System.Windows.Forms.AccessibleRole.RadioButton;
            this.Button八进制.Name = "Button八进制";
            this.Button八进制.ObjectImage = ((System.Drawing.Bitmap)(resources.GetObject("Button八进制.ObjectImage")));
            this.Button八进制.Parent = this.计算器Window;
            this.Button八进制.UIObjectType = TestAutomationFX.UI.UIObjectTypes.Button;
            this.Button八进制.WindowClass = "Button";
            this.Button八进制.WindowText = "八进制";
            // 
            // ButtonMultiply
            // 
            this.ButtonMultiply.Comment = null;
            this.ButtonMultiply.Index = 58;
            this.ButtonMultiply.MatchedIndex = 0;
            this.ButtonMultiply.MsaaRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.ButtonMultiply.Name = "ButtonMultiply";
            this.ButtonMultiply.ObjectImage = ((System.Drawing.Bitmap)(resources.GetObject("ButtonMultiply.ObjectImage")));
            this.ButtonMultiply.Parent = this.计算器Window;
            this.ButtonMultiply.UIObjectType = TestAutomationFX.UI.UIObjectTypes.Button;
            this.ButtonMultiply.WindowClass = "Button";
            this.ButtonMultiply.WindowText = "*";
            // 
            // Button6
            // 
            this.Button6.Comment = null;
            this.Button6.Index = 53;
            this.Button6.MatchedIndex = 0;
            this.Button6.MsaaRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.Button6.Name = "Button6";
            this.Button6.ObjectImage = ((System.Drawing.Bitmap)(resources.GetObject("Button6.ObjectImage")));
            this.Button6.Parent = this.计算器Window;
            this.Button6.UIObjectType = TestAutomationFX.UI.UIObjectTypes.Button;
            this.Button6.WindowClass = "Button";
            this.Button6.WindowText = "6";
            // 
            // Button十进制
            // 
            this.Button十进制.Comment = null;
            this.Button十进制.Index = 6;
            this.Button十进制.MatchedIndex = 0;
            this.Button十进制.MsaaRole = System.Windows.Forms.AccessibleRole.RadioButton;
            this.Button十进制.Name = "Button十进制";
            this.Button十进制.ObjectImage = ((System.Drawing.Bitmap)(resources.GetObject("Button十进制.ObjectImage")));
            this.Button十进制.Parent = this.计算器Window;
            this.Button十进制.UIObjectType = TestAutomationFX.UI.UIObjectTypes.Button;
            this.Button十进制.WindowClass = "Button";
            this.Button十进制.WindowText = "十进制";
            // 
            // TestFixture1
            // 
            this.UIMapObjectApplications.Add(this.CalcApplication);

        }

        #endregion

        private TestAutomationFX.UI.UIApplication CalcApplication;
        private TestAutomationFX.UI.UIWindow 计算器Window;
        private TestAutomationFX.UI.UIMsaa TitleBar;
        private TestAutomationFX.UI.UIWindow Button1;
        private TestAutomationFX.UI.UIWindow ButtonPlus;
        private TestAutomationFX.UI.UIWindow ButtonEquals;
        private TestAutomationFX.UI.UIWindow Button2;
        private TestAutomationFX.UI.UIWindow TextBox1;
        private TestAutomationFX.UI.UIWindow Button八进制;
        private TestAutomationFX.UI.UIWindow ButtonMultiply;
        private TestAutomationFX.UI.UIWindow Button6;
        private TestAutomationFX.UI.UIWindow Button十进制;
    }
}
