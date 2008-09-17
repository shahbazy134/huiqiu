using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Configuration;
using System.Windows.Forms;

namespace WindowsLiveIDClientSample
{
    public partial class OptionsWindow : Form
    {
        public OptionsWindow()
        {
            InitializeComponent();
            if (MainWindow.currentUserName.Equals(MainWindow.defaultUserName))
            {
                checkBoxDefaultUser.Checked = true;
            }
            else
            {
                checkBoxDefaultUser.Checked = false;
            }
        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog1 = new ColorDialog();
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                MainWindow.tempBgColor = colorDialog1.Color.ToArgb().ToString();
            }
            else
            {
                MainWindow.tempBgColor = "";
            }
        }

        private void checkBoxDefaultUser_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDefaultUser.Checked)
            {
                MainWindow.tempDefaultUserName = MainWindow.currentUserName;
            }
            else
            {
                MainWindow.tempDefaultUserName = "";
            }
        }
    }
}