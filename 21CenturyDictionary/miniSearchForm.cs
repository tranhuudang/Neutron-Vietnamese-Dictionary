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
    public partial class miniSearchForm : Form
    {

        public static string textSearchInDuty;
        public miniSearchForm()
        {
            InitializeComponent();
        }

        private void miniSearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            // save form's size setting
            Properties.Settings.Default["Size"] = "big";
            Properties.Settings.Default.Save();

            // tìm cái form main, nếu nó có tồn tại thì mở
            var formToShow = Application.OpenForms.Cast<Form>().FirstOrDefault(c => c is mainForm);
            if (formToShow != null)
            {
                formToShow.Show();
            }
            else
            {
                mainForm MF = new mainForm();
                MF.Show();
            }
            this.Hide();

        }
        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            textSearchInDuty = textSearch.Text;
            if (textSearch.Text != "")
            {
                CleanMess.Visible = true;
            }
            else
            {
                CleanMess.Visible = false;
            }
        }

        private void CleanMess_Click(object sender, EventArgs e)
        {
            textSearch.Text = "";
        }

        private void SearchByVoice_Click(object sender, EventArgs e)
        {
            textSearch.Text = "Listening...";
            WordFactory word = new WordFactory();
            word.PreWord = textSearch.Text;
            textSearch.Text = Sound.SpeechToText(word.OutWord);
        }

        private void textSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (textSearch.Text != "")
            {
                if (textSearch.Text == "-reset") Application.Restart();
                if (e.KeyCode == Keys.Delete) textSearch.Text = "";
                if (e.KeyCode == Keys.Enter)
                {
                    string word = textSearch.Text;
                    bool founded;
                    searchMatchWord(word, out founded);

                    // nếu tìm thấy từ đó trong dữ liệu thì hiển thị 
                    if (founded)
                    {


                        // gán từ đã tìm thấy vào lịch sử tìm kiếm 
                        // tempertext là nơi để từ đã được tìm kiếm và đã tìm thấy
                        FileFactory.AddItem(word, "History.txt");
                        // hiển thị panel dịch thuật và hiệu ứng thu vào mở ra
                        if (panelRight.Visible == false)
                        {
                            panelRight.Visible = false;
                            panelRight.Location = new Point(0, 0);

                        }
                    }


                }
            }

        }
        public void searchMatchWord(string wordsToSearch, out bool availableText) // THIS IS FUNCTION TO SEARCH TEXT 
        {

            availableText = false;
            // xử lí từ ngữ được người dùng nhập vào------------------
            WordFactory word = new WordFactory();
            // Nạp từ vào đối tượng
            word.PreWord = wordsToSearch;
            // Xử lí từ và lấy kết quả
            wordsToSearch = word.SearchWordProcess();
            // tô màu trái tim đôi với từ nằm trong mục favourite 
            heartForWord.Image = Properties.Resources.heart_32_light;
            if (FileFactory.FileContain(word.OutWord, "Favourite.txt") != -1)
            {
                heartForWord.Image = Properties.Resources.heart_32_light_filled;
            }

            int indexOfArray = 0;
            // đọc từng dòng trong paragraph sau đó gán giá trị cho line 
            // ** using class to improve searching speed
            panelSound.Visible = false; // nếu không tìm thấy chữ này thì không hiển thị âm thanh
            foreach (string wordAndMeanning in DataProcessing.paragraph)
            {

                if (wordAndMeanning.Contains("• " + wordsToSearch + " "))
                {
                    temperTextSearch.Text = wordsToSearch;
                    availableText = true; // return true to available text if the word is founded
                    panelSound.Visible = true; // nếu tìm thấy chữ này mới hiển thị âm thanh
                    richWordMeanning.Text = DataProcessing.paragraph[indexOfArray];
                    break;

                }
                indexOfArray++;
            }


            // color particular text 
            Color wordTypeColor = Color.FromArgb(0, 122, 204);
            int wordPosition;
            // color all text that represent word type
            string[] listWordType = { "▫  danh từ", "▫  mạo từ", "▫  giới từ"
                    , "▫  tính từ","▫  ngoại động từ","▫  (viết tắt)","▫  phó từ"
            ,"▫  nội động từ"};

            for (int i = 0; i < listWordType.Length; i++)
            {
                string wordType = listWordType[i];
                // indentify the position of word type in paragraph
                wordPosition = richWordMeanning.Text.IndexOf(wordType);
                if (wordPosition >= 0) // check if there're at least a word fit the search
                {
                    // indentify index of line which have word type in it
                    int indexOfLine = richWordMeanning.GetLineFromCharIndex(wordPosition);
                    try
                    {

                        richWordMeanning.Select(wordPosition, wordType.Length);
                        richWordMeanning.SelectionColor = wordTypeColor;
                    }
                    catch (Exception errorReport)
                    {
                        MessageBox.Show(errorReport.ToString());
                    }


                }
            }
        }

        private void miniSearchForm_Load(object sender, EventArgs e)
        {
           
            /// load theme
            switch (Properties.Settings.Default["Theme"].ToString())
            {
                case "dark":
                    panelRight.BackColor = Color.FromArgb(30,30,30);
                    richWordMeanning.ForeColor = U.white1;
                    richWordMeanning.BackColor = Color.FromArgb(30, 30, 30);
                    textSearch.ForeColor = U.white1; // box search
                    textSearch.BackColor = U.gray5;
                    BehindTextSearch.BackColor = U.gray5;
                    
                    break;
            } 
           
            /// dùng đẻ chuyển sang giao diện ngôn ngữ tiếng việt
            if (Properties.Settings.Default["Language"].ToString()=="Tiếng Việt")
            {
                this.Controls.Clear();
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("vi-VN");
                InitializeComponent();
            }
            const int dist = 24;
            richWordMeanning.SetInnerMargins(dist, 0, dist, dist);
            
        }

        private void miniSearchForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void miniSearchForm_Resize(object sender, EventArgs e)
        {
            richWordMeanning.Height = this.Height - 180;
            line.Width = this.Width;
        }
    }
}
