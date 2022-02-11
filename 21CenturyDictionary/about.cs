using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace NeutronDictionary
{
    public partial class about : Form
    {
        public about()
        {
            InitializeComponent();
        }

        private void about_Load(object sender, EventArgs e)
        {
            /// dùng đẻ chuyển sang giao diện ngôn ngữ tiếng việt
            switch (Properties.Settings.Default["Language"].ToString())
            {
                case "English":
                    this.Controls.Clear();
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                    InitializeComponent();
                    break;
                case "Tiếng Việt":
                    this.Controls.Clear();
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("vi-VN");
                    InitializeComponent();
                    break;
            }
            Version.Text = Application.ProductVersion.ToString();
           
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonBack_MouseEnter(object sender, EventArgs e)
        {
            buttonBack.BackColor = Color.FromArgb(38, 157, 237);
        }

        private void buttonBack_MouseLeave(object sender, EventArgs e)
        {
            buttonBack.BackColor = Color.FromArgb(0, 122, 204);
        }

        private void ReleaseNotes_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.IO.File.WriteAllText("Release-Notes.txt", Properties.Resources.Release_Notes);
                Process.Start("Release-Notes.txt");
            }
            catch (Exception error)
            {
                MessageBox.Show("We're encountering a error here: " + error);
            }
        }
    }
}
