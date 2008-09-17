using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using lotus;
using System.Net;
using System.IO;

namespace NotesCOMDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Use HttpWebRequest to authentication domino login info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //NOTESSESSION s = new note
            string postData = "username=administrator&password=12345678";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://127.0.0.1/names.nsf?Login");
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;
            request.Method = "POST";
            request.AllowAutoRedirect = false;

            Stream requestStream = request.GetRequestStream();
            byte[] postBytes = Encoding.ASCII.GetBytes(postData);
            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            textBox1.Text = new StreamReader(response.GetResponseStream()).ReadToEnd() + "\n";
            textBox1.Text = textBox1.Text + "Headers:" + "\n";
            textBox1.Text = textBox1.Text + response.Headers.ToString() + "\n";
            //Console.WriteLine(new StreamReader(response.GetResponseStream()).ReadToEnd());
            //Console.WriteLine("Headers:");
            //Console.WriteLine(response.Headers.ToString());
            textBox1.Text = textBox1.Text + response.StatusCode + "\n";
        }
    }
}
