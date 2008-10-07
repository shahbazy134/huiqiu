using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace UnhandledWPFException
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void m_btnGoBoom_Click(object sender, RoutedEventArgs e)
        {
            throw new ArgumentException("I disagree!");
        }

        private void m_btnThreadBoom_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(ThreadBoom);
            thread.Start();
        }

        private void ThreadBoom(object data)
        {
            throw new OverflowException("Click Click BOOM!");
        }
    }
}
