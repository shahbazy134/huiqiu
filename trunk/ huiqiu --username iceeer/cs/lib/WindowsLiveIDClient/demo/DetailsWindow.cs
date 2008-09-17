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
    public partial class DetailsWindow : Form
    {
        public DetailsWindow(Identity oID)
        {
            InitializeComponent();
            //Display various properties of the Identity object representing the user.
            labelcId.Text = "cId: " + oID.cId;
            labelIsAuthed.Text = "IsAuthenticated: " + oID.IsAuthenticated.ToString();
            labelPersist.Text = "SavedCredentials: " + oID.SavedCredentials.ToString();
            labelUserName.Text = "UserName: " + oID.UserName;
            labelIsDefaultUser.Text = "Default User: " + oID.UserName.Equals(MainWindow.defaultUserName);
        }
    }
}