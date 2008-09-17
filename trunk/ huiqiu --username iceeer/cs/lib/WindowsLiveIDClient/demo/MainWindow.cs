using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsLive.Id.Client;
using System.Configuration;

namespace WindowsLiveIDClientSample
{
    public partial class MainWindow : Form
    {
        IdentityManager oIDMgr;
        Identity oID;
        public static string currentUserName = "";
        public string currentBgColor = "";
        public static string defaultUserName = "";
        public static string tempBgColor = "";
        public static string tempDefaultUserName = "";
        System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public MainWindow()
        {
            InitializeComponent();
            //Try initializing the global instance of IdentityManager.
            try
            {
                oIDMgr = IdentityManager.CreateInstance("Tailspin Toys;user@tailspintoys.com;Tailspin Toys Application", "Windows Live ID Client Sample");
            }
            catch (WLLogOnException wlex)
            {
                //Check to see if FlowUrl is defined.
                if (wlex.FlowUrl != null)
                {
                    //If FlowUrl is defined, direct user to the web page to correct the error.
                    MessageBox.Show(wlex.ErrorString + "Please go to " + wlex.FlowUrl.AbsoluteUri + "to correct the condition that caused the error");
                }
                else
                {
                    //If FlowUrl is not defined, simply display the ErrorString.
                    MessageBox.Show(wlex.ErrorString);
                }
            }

            //Check the config file to see if a default user is defined.
            defaultUserName = ConfigurationManager.AppSettings["defaultUserName"];
            if (!String.IsNullOrEmpty(defaultUserName))
            {
                TrySilentSignIn();
            }
            else
            {
                //If no default user is defined, try instantiating the global Identity object instance from scratch.
                try
                {
                    oID = oIDMgr.CreateIdentity();
                }
                catch (WLLogOnException wlex)
                {
                    //Check to see if FlowUrl is defined.
                    if (wlex.FlowUrl != null)
                    {
                        //If FlowUrl is defined, direct user to the web page to correct the error.
                        MessageBox.Show(wlex.ErrorString + "Please go to " + wlex.FlowUrl.AbsoluteUri + "to correct the condition that caused the error");
                    }
                    else
                    {
                        //If FlowUrl is not defined, simply display the ErrorString.
                        MessageBox.Show(wlex.ErrorString);
                    }
                }
            }
            UpdateDisplay();
        }

        private void TrySilentSignIn()
        {
            //Try instantiating the global Identity object instance with the username from the config file.
            try
            {
                oID = oIDMgr.CreateIdentity(defaultUserName);
            }
            catch (WLLogOnException wlex)
            {
                //Check to see if FlowUrl is defined.
                if (wlex.FlowUrl != null)
                {
                    //If FlowUrl is defined, direct user to the web page to correct the error.
                    MessageBox.Show(wlex.ErrorString + "Please go to " + wlex.FlowUrl.AbsoluteUri + "to correct the condition that caused the error");
                }
                else
                {
                    //If FlowUrl is not defined, simply display the ErrorString.
                    MessageBox.Show(wlex.ErrorString);
                }
            }

            //Check that the username is valid.
            if (oID != null)
            {
                //Check to make sure the user has stored their username and password.
                if (oID.SavedCredentials == CredentialType.UserNameAndPassword)
                {
                    try
                    {
                        //Try silent authentication.
                        if (oID.Authenticate(AuthenticationType.Silent))
                        {
                            currentUserName = defaultUserName;
                        }
                        else
                        {
                            MessageBox.Show("Default user's stored sign-in name and password are invalid.");
                        }
                    }
                    catch (WLLogOnException wlex)
                    {
                        //Check to see if FlowUrl is defined.
                        if (wlex.FlowUrl != null)
                        {
                            //If FlowUrl is defined, direct user to the web page to correct the error.
                            MessageBox.Show(wlex.ErrorString + "Please go to " + wlex.FlowUrl.AbsoluteUri + "to correct the condition that caused the error");
                        }
                        else
                        {
                            //If FlowUrl is not defined, simply display the ErrorString.
                            MessageBox.Show(wlex.ErrorString);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Default user hasn't stored sign-in name and password.");
                }
            }
            else
            {
                MessageBox.Show("defaultUserName in config file has an invalid value.");
                config.AppSettings.Settings.Remove("defaultUserName");
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }

        //Event handler for Sign-In/Sign-Out button clicks.
        private void buttonSignInOrOut_Click(object sender, EventArgs e)
        {
            //Check to see if the user is not currently authenticated.
            if (!oID.IsAuthenticated)
            {
                try
                {
                    //Try to authenticate the user by showing the Sign-In dialog window.
                    if (oID.Authenticate())
                    {
                        currentUserName = oID.UserName;
                    }
                    else
                    {
                        MessageBox.Show("Authentication failed");
                    }
                }
                catch (WLLogOnException wlex)
                {
                    //Check to see if FlowUrl is defined.
                    if (wlex.FlowUrl != null)
                    {
                        //If FlowUrl is defined, direct user to the web page to correct the error.
                        MessageBox.Show(wlex.ErrorString + "Please go to " + wlex.FlowUrl.AbsoluteUri + "to correct the condition that caused the error");
                    }
                    else
                    {
                        //If FlowUrl is not defined, simply display the ErrorString.
                        MessageBox.Show(wlex.ErrorString);
                    }
                }
            }
            else
            {
                //If user is authenticated, they intended to Sign-Out. Try signing the user out.
                try
                {
                    oID.CloseIdentityHandle();
                    currentUserName = "";
                }
                catch (WLLogOnException wlex)
                {
                    //Check to see if FlowUrl is defined.
                    if (wlex.FlowUrl != null)
                    {
                        //If FlowUrl is defined, direct user to the web page to correct the error.
                        MessageBox.Show(wlex.ErrorString + "Please go to " + wlex.FlowUrl.AbsoluteUri + "to correct the condition that caused the error");
                    }
                    else
                    {
                        //If FlowUrl is not defined, simply display the ErrorString.
                        MessageBox.Show(wlex.ErrorString);
                    }
                }
            }
            UpdateDisplay();
        }

        //Event handler for Options button clicks.
        private void buttonOptions_Click(object sender, EventArgs e)
        {
            OptionsWindow optsWindow = new OptionsWindow();
            if (optsWindow.ShowDialog() == DialogResult.OK)
            {
                UpdateConfig();
            }
            UpdateDisplay();
        }

        /// <summary>
        /// Update the config file.
        /// </summary>
        private void UpdateConfig()
        {
            if (!String.IsNullOrEmpty(tempBgColor))
            {
                config.AppSettings.Settings.Remove(oID.cId + "_bgColor");
                config.AppSettings.Settings.Add(oID.cId + "_bgColor", tempBgColor);
            }
            else
            {
                config.AppSettings.Settings.Remove(oID.cId + "_bgColor");
            }

            if (tempDefaultUserName != "")
            {
                defaultUserName = tempDefaultUserName;
                config.AppSettings.Settings.Remove("defaultUserName");
                config.AppSettings.Settings.Add("defaultUserName", tempDefaultUserName);
            }
            else if (oID.UserName.Equals(defaultUserName))
            {
                config.AppSettings.Settings.Remove("defaultUserName");
                defaultUserName = "";
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        //Method which updates the display of the application to reflect its current state.
        private void UpdateDisplay()
        {
            //Check to see if the user is authenticated and update the display appropriately.
            if (oID.IsAuthenticated)
            {
                toolStripStatusLabel1.Text = "Signed In";
                buttonSignInOrOut.Text = "Sign Out";
                buttonOptions.Enabled = true;
                buttonBlog.Enabled = true;
                buttonDetails.Enabled = true;
                currentBgColor = ConfigurationManager.AppSettings[oID.cId + "_bgColor"];
            }
            //If user is not authenticated, update the display appropriately.
            else
            {
                toolStripStatusLabel1.Text = "Signed Out";
                buttonSignInOrOut.Text = "Sign In";
                buttonOptions.Enabled = false;
                buttonBlog.Enabled = false;
                buttonDetails.Enabled = false;
                currentBgColor = "";
            }

            if (!String.IsNullOrEmpty(currentBgColor))
            {
                this.BackColor = Color.FromArgb(Int32.Parse(currentBgColor));
            }
            else
            {
                this.BackColor = Form.DefaultBackColor;
            }
        }

        private void buttonBlog_Click(object sender, EventArgs e)
        {
            BlogWindow blogWindow = new BlogWindow(oID);
            blogWindow.ShowDialog();
        }

        private void buttonDetails_Click(object sender, EventArgs e)
        {
            DetailsWindow detailsWindow = new DetailsWindow(oID);
            detailsWindow.ShowDialog();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://go.microsoft.com/fwlink/?LinkId=81967");
        }

        private void linkLabelSpaces_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.linkLabelSpaces.Text);
        }
    }
}