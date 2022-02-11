using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeutronDictionary
{
    public partial class paragraphTranslate : Form
    {
        public paragraphTranslate()
        {
            InitializeComponent();
        }

        private void paragraphTranslate_Load(object sender, EventArgs e)
        {
            /// dùng đẻ chuyển sang giao diện ngôn ngữ tiếng việt
            if (File.Exists("Language.ini"))
            {
                this.Controls.Clear();
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("vi-VN");
                InitializeComponent();
            }
            WordFactory link = new WordFactory();
           // MessageBox.Show(mainForm.textSearchInDuty);
            link.PreParagraph = mainForm.textSearchInDuty;
            link.TranslateParagraphProcess();
            Uri linktrans = new Uri(link.OutParagraph);
            web.Url = linktrans;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonBack_MouseLeave(object sender, EventArgs e)
        {
            buttonBack.BackColor = Color.FromArgb(0, 122, 204);
        }

        private void buttonBack_MouseEnter(object sender, EventArgs e)
        {
            buttonBack.BackColor = Color.FromArgb(38, 157, 237);
        }
    }
}
