using System;
using System.ComponentModel;

using TestAutomationFX.Core;
using TestAutomationFX.UI;

namespace TafxApplicationDemo
{
    [UITestFixture]
    public partial class TestFixture1 : UIMap
    {
        public TestFixture1()
        {
            InitializeComponent();
        }

        public TestFixture1(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        [UITest()]
        public void Test1()
        {
            TitleBar.Click(198, 19);
            Button十进制.Click();
            Button1.Click();
            ButtonPlus.Click();
            Button1.Click();
            ButtonEquals.Click();
            Button2.Click();
            TextBox1.VerifyProperty("Text", "2. ");
            Button八进制.Click();
            TextBox1.VerifyProperty("Text", "2 ");
            ButtonMultiply.Click();
            Button6.Click();
            ButtonEquals.Click();
            TextBox1.VerifyProperty("Text", "14 ");
        }
    }
}
