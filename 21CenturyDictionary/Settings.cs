using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace NeutronDictionary
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

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

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }
        private void SaveSettings()
        {
            if (Personalization.Checked == true) Properties.Settings.Default["Personalization"] = true;
            else Properties.Settings.Default["Personalization"] = false;
            if (VoiceTyping.Checked == true) Properties.Settings.Default["VoiceTyping"] = true;
            else Properties.Settings.Default["VoiceTyping"] = false;
            if (SearchByEmoji.Checked == true) Properties.Settings.Default["SearchByEmoji"] = true;
            else Properties.Settings.Default["SearchByEmoji"] = false;
            if (PredictiveSearch.Checked == true) Properties.Settings.Default["PredictiveSearch"] = true;
            else Properties.Settings.Default["PredictiveSearch"] = false;
            switch (LanguageBox.Text)
            {
                case "English":
                    Properties.Settings.Default["Language"] = "English";
                    break;
                case "Tiếng Việt":
                    Properties.Settings.Default["Language"] = "Tiếng Việt";
                    break;
            }
            Properties.Settings.Default.Save();
        }
        private void LoadSettings()
        {
            Personalization.Checked = bool.Parse(Properties.Settings.Default["Personalization"].ToString());
            VoiceTyping.Checked = bool.Parse(Properties.Settings.Default["VoiceTyping"].ToString());
            SearchByEmoji.Checked = bool.Parse(Properties.Settings.Default["SearchByEmoji"].ToString());
            PredictiveSearch.Checked = bool.Parse(Properties.Settings.Default["PredictiveSearch"].ToString());
            LanguageBox.Text = Properties.Settings.Default["Language"].ToString();
        }
        private void Settings_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void Personalization_MouseDown(object sender, MouseEventArgs e)
        {
            Notice.Visible = true;
        }

        private void LanguageBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (LanguageBox.Text)
            {
                case "English":
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                    Properties.Settings.Default["Language"] = "English";
                    break;
                case "Tiếng Việt":
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("vi-VN");
                    Properties.Settings.Default["Language"] = "Tiếng Việt";
                    break;
            }
            this.Controls.Clear();
            InitializeComponent();
        }

        private void Slow_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["SoundSpeed"] = "slow";

        }

        private void Medium_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["SoundSpeed"] = "medium";
        }

        private void Fast_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["SoundSpeed"] = "fast";
        }
    }
}
