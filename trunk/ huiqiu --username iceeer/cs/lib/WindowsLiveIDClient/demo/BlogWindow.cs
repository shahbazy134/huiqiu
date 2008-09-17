using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsLive.Id.Client;

namespace WindowsLiveIDClientSample
{
    public partial class BlogWindow : Form
    {
        Identity oID;
        public BlogWindow(Identity oID)
        {
            InitializeComponent();
            buttonPostBlog.Enabled = false;
            buttonViewBlog.Enabled = false;
            buttonClear.Enabled = false;
            this.oID = oID;
        }

        //Event handler for Post Blog button clicks
        private void buttonPostBlog_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxBlogBody.Text) && !String.IsNullOrEmpty(textBoxBlogSubject.Text) && !String.IsNullOrEmpty(textBoxSpaceUrl.Text))
            {
                try
                {
                    SpacesAdapter.PostBlog(textBoxBlogSubject.Text, textBoxBlogBody.Text,
textBoxSpaceUrl.Text, oID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    MessageBox.Show("Blog request sent!");
                }
            }
            else
            {
                MessageBox.Show("Please enter some text before posting.");
            }
        }

        //Event handler for View Blog button clicks.
        private void buttonViewBlog_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxSpaceUrl.Text))
            {
                //Try opening a pre-authenticated browser window to view the user's blog.
                try
                {
                    oID.OpenAuthenticatedBrowser("http://" + textBoxSpaceUrl.Text + ".spaces.live.com/blog/", "lbi");
                }
                catch (WLLogOnException wlex)
                {
                    //Check to see if FlowUrl is defined.
                    if (wlex.FlowUrl != null)
                    {
                        //If FlowUrl is defined, direct user to the web page to correct the error.
                        MessageBox.Show(wlex.ErrorString + "Please go to " + wlex.FlowUrl.AbsoluteUri + "to correct the condition that caused the error");
                    }
                    else if(wlex.ErrorString.Length > 0)
                    {
                        //If FlowUrl is not defined, simply display the ErrorString.
                        MessageBox.Show(wlex.ErrorString);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter a SpaceUrl before browsing.");
            }
        }

        private void textBoxBlogBody_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxSpaceUrl.Text))
            {
                if (String.IsNullOrEmpty(textBoxBlogBody.Text) || String.IsNullOrEmpty(textBoxBlogSubject.Text))
                {
                    buttonViewBlog.Enabled = true;
                    buttonPostBlog.Enabled = false;
                    buttonClear.Enabled = true;
                }
                else
                {
                    buttonViewBlog.Enabled = true;
                    buttonPostBlog.Enabled = true;
                    buttonClear.Enabled = true;
                }
            }
            else
            {
                buttonViewBlog.Enabled = false;
                buttonPostBlog.Enabled = false;
                buttonClear.Enabled = false;

            }             
        }

        private void textBoxBlogSubject_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxSpaceUrl.Text))
            {
                if (String.IsNullOrEmpty(textBoxBlogBody.Text) || String.IsNullOrEmpty(textBoxBlogSubject.Text))
                {
                    buttonViewBlog.Enabled = true;
                    buttonPostBlog.Enabled = false;
                    buttonClear.Enabled = true;
                }
                else
                {
                    buttonViewBlog.Enabled = true;
                    buttonPostBlog.Enabled = true;
                    buttonClear.Enabled = true;
                }
            }
            else
            {
                buttonViewBlog.Enabled = false;
                buttonPostBlog.Enabled = false;
                buttonClear.Enabled = false;
            }             

        }

        private void textBoxSpaceUrl_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxSpaceUrl.Text))
            {
                labelSpaceURLExample.Text = "http://" + textBoxSpaceUrl.Text + ".spaces.live.com";
                if (String.IsNullOrEmpty(textBoxBlogBody.Text) || String.IsNullOrEmpty(textBoxBlogSubject.Text))
                {
                    buttonViewBlog.Enabled = true;
                    buttonPostBlog.Enabled = false;
                }
                else
                {
                    buttonViewBlog.Enabled = true;
                    buttonPostBlog.Enabled = true;
                }
            }
            else
            {
                buttonViewBlog.Enabled = false;
                buttonPostBlog.Enabled = false;
            }             

        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxBlogSubject.Text = "";
            textBoxBlogBody.Text = "";
        }
    }
}