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
namespace NeutronDictionary
{
    public partial class Hub : Form
    {
        public Hub()
        {
            InitializeComponent();
        }

        private void Hub_Load(object sender, EventArgs e)
        {
            // load file size để quy định kích thước chương trình là form to hay nhỏ
            switch (Properties.Settings.Default["Size"].ToString())
            {
                case "small":
                    miniSearchForm mini = new miniSearchForm();
                    mini.Show();
                    break;
                case "big":
                    mainForm MF = new mainForm();
                    MF.Show();
                    break;
                default:
                    mainForm DF = new mainForm();
                    DF.Show();
                    break;
            }
            
            
        }
    }
}
