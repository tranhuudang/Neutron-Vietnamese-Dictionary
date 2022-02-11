using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Threading;
using System.Runtime.InteropServices;


#pragma warning disable IDE1006 //dùng để tắt các cảnh báo liên quan tới cú pháp
namespace NeutronDictionary
{
    public partial class mainForm : Form
    {

        public static string textSearchInDuty;
        private static AutoCompleteStringCollection list = new AutoCompleteStringCollection();
        public mainForm()
        {


            // Tùy biến giao diện dựa trên DPI của màn hình người dùng. Xử lí tình huống vỡ giao diện, nhòe chữ.
           



            InitializeComponent();

        }

        // Giảm kích thước font chữ
        public void SetAllControlsFont(Control.ControlCollection ctrls, int minusFontSize)
        {
            foreach (Control ctrl in ctrls)
            {
                if (ctrl.Controls != null)
                    SetAllControlsFont(ctrl.Controls, minusFontSize);

                ctrl.Font = new Font("MS Reference Sans Serif", ctrl.Font.Size - minusFontSize);

            }
        }



        // Xóa nhòe chữ cho scale trên 100
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        // #####################################################################################################
        // Hàm và thủ tục này dùng để kiểm tra mức độ thu phóng của hệ điều hành
        // Qua đó cho phép người lập trình xử lí giao diện tốt hơn
        [System.Runtime.InteropServices.DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117
        }

        static double GetWindowsScreenScalingFactor(bool percentage = true)
        {
            //Create Graphics object from the current windows handle
            Graphics GraphicsObject = Graphics.FromHwnd(IntPtr.Zero);
            //Get Handle to the device context associated with this Graphics object
            IntPtr DeviceContextHandle = GraphicsObject.GetHdc();
            //Call GetDeviceCaps with the Handle to retrieve the Screen Height
            int LogicalScreenHeight = GetDeviceCaps(DeviceContextHandle, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(DeviceContextHandle, (int)DeviceCap.DESKTOPVERTRES);
            //Divide the Screen Heights to get the scaling factor and round it to two decimals
            double ScreenScalingFactor = Math.Round((double)PhysicalScreenHeight / (double)LogicalScreenHeight, 2);
            //If requested as percentage - convert it
            if (percentage)
            {
                ScreenScalingFactor *= 100.0;
            }
            //Release the Handle and Dispose of the GraphicsObject object
            GraphicsObject.ReleaseHdc(DeviceContextHandle);
            GraphicsObject.Dispose();
            //Return the Scaling Factor
            return ScreenScalingFactor;
        }
        // #####################################################################################################





        private void mainForm_Load(object sender, EventArgs e)
        {

            /// language setting
            switch (Properties.Settings.Default["Language"].ToString())
            {
                case "English":
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                    this.Controls.Clear();
                    InitializeComponent();
                    switch (GetWindowsScreenScalingFactor())
                    {
                        case 100: // 100% scaling

                            break;
                        case 125: // 125% scaling

                            SetProcessDPIAware();
                            SetAllControlsFont(this.Controls,4);
                            break;
                        case 150: // 150% scaling
                            SetProcessDPIAware();
                            SetAllControlsFont(this.Controls,5);
                            break;
                    }
                   
                    break;
                case "Tiếng Việt":
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("vi-VN");
                    this.Controls.Clear();
                    InitializeComponent();
                    switch (GetWindowsScreenScalingFactor())
                    {
                        case 100: // 100% scaling

                            break;
                        case 125: // 125% scaling

                            SetProcessDPIAware();
                            SetAllControlsFont(this.Controls,4);
                            break;
                        case 150: // 150% scaling
                            SetProcessDPIAware();
                            SetAllControlsFont(this.Controls,5);
                            break;
                    }
                    break;
            }

            int programHeight = 686;

            // change form's dimentions
            this.MaximumSize = new Size(970, 722);
            //--------------------------
            feature.Location = new Point(149, 313);
            feature.Width = 660;
            feature.Height = 122;
            //--------------------------
            panelRight.Location = new Point(panelRight.Width, 0);
            panelRight.Width = 327;
            saparateLineInPanelRight.Location = new Point(panelRight.Width - 2, 0);
            saparateLineInPanelRight.Size = new Size(2, programHeight);
            panelFavouriteAndHistory.Location = new Point(panelRight.Width + 15, 0);
            panelFavouriteAndHistory.Size = new Size(this.Width - (panelRight.Width + 15), programHeight);
            panelParagraph.Location = new Point(320, 220);
            //richMainParagraph.Location = new Point(85, 257);
            //richMainParagraph.Size = new Size(771, 382);

            /// load other settings

            if (bool.Parse(Properties.Settings.Default["VoiceTyping"].ToString()))
                SearchByVoice.Visible = true;
            else SearchByVoice.Visible = false;
            if (bool.Parse(Properties.Settings.Default["PredictiveSearch"].ToString()))
                textSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;
            else textSearch.AutoCompleteSource = AutoCompleteSource.None;
            switch (Properties.Settings.Default["Theme"].ToString())
            {
                case "light":
                    enableLightMode_Click(null, null);
                    break;
                case "dark":
                    enableDarkMode_Click(null, null);
                    break;
                case "special":
                    enableSpecialMode();
                    break;
            }

            // Load suggestion text
            foreach (string line in DataProcessing.wordsList)
            {
                list.Add(line);
            }
            textSearch.AutoCompleteCustomSource = list;


        }
        private void richWordMeanning_TextChanged(object sender, EventArgs e)
        {

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

        private void panelSearch_MouseHover(object sender, EventArgs e) { }
        private void enableDarkMode_Click(object sender, EventArgs e)
        {
            enableDarkMode.Visible = false;
            enableLightMode.Visible = true;
            Properties.Settings.Default["Theme"] = "dark";
            Properties.Settings.Default.Save();
            // meanning panel

            this.BackColor = U.gray1;
            this.ForeColor = U.white1;
            panelRight.BackColor = U.gray2;
            richWordMeanning.ForeColor = U.white1;
            richWordMeanning.BackColor = U.gray2;
            textSearch.ForeColor = U.white1; // box search
            textSearch.BackColor = U.gray5;
            BehindTextSearch.BackColor = U.gray5;

            saparateLineInPanelRight.BackColor = U.black1;
            panelFavouriteAndHistory.BackColor = U.gray1;
            listFavouriteHistory.BackColor = U.gray1;
            listFavouriteHistory.ForeColor = U.white1;
            //buttpic
            buttonHistory.Image = Properties.Resources.history_light;

            buttonFavourite.Image = Properties.Resources.favourite_light;
            //buttonDailyWord.Image = Properties.Resources.dailyword_light;
            buttonGoogleTranslate.Image = Properties.Resources.paragraph_light;
            heartForWord.Image = Properties.Resources.heart_32_light;
            pictureForWord.Image = Properties.Resources.picture_32_light;
            soundForWord.Image = Properties.Resources.volume_24_light;
            buttonSettings.Image = Properties.Resources.more_24_light;
            buttonDailyWord.Image = Properties.Resources.talk_64_white_gray;
            // paragraph panel
            RelatedMeaning.BackColor = U.gray1;
            RelatedMeaning.ForeColor = U.blue1;
            bunifuImageButton2.Image = Properties.Resources.smaller_32_light;// resize button 
        }

        private void enableLightMode_Click(object sender, EventArgs e)
        {
            enableDarkMode.Visible = true;
            enableLightMode.Visible = false;
            Properties.Settings.Default["Theme"] = "light";
            Properties.Settings.Default.Save();
            //change color of all item
            this.BackColor = U.white1;
            this.ForeColor = U.black1;
            panelRight.BackColor = U.white2; // panel meanning
            richWordMeanning.BackColor = U.white2;
            richWordMeanning.ForeColor = U.black1;
            textSearch.ForeColor = U.gray3; // box search
            textSearch.BackColor = U.white1;
            BehindTextSearch.BackColor = U.white1;

            saparateLineInPanelRight.BackColor = U.gray6;
            panelFavouriteAndHistory.BackColor = U.white1;
            listFavouriteHistory.BackColor = U.white1;
            listFavouriteHistory.ForeColor = U.black1;
            // buttonpic
            buttonHistory.Image = Properties.Resources.history;
            buttonFavourite.Image = Properties.Resources.favourite1;
            //buttonDailyWord.Image = Properties.Resources.dailyword;
            buttonGoogleTranslate.Image = Properties.Resources.paragraph;
            heartForWord.Image = Properties.Resources.heart_32;
            pictureForWord.Image = Properties.Resources.picture_32;
            buttonSettings.Image = Properties.Resources.more_24;
            buttonDailyWord.Image = Properties.Resources.talks_64_black_2;
            // paragraph panel
            RelatedMeaning.BackColor = U.white1;
            RelatedMeaning.ForeColor = U.blue1;
            bunifuImageButton2.Image = Properties.Resources.smaller_32_dark;// resize button 
        }
        private void enableSpecialMode()
        {

        }
        private void richWordMeanning_MouseEnter(object sender, EventArgs e)
        {
            richWordMeanning.ScrollBars = RichTextBoxScrollBars.Vertical;
        }

        private void richWordMeanning_MouseLeave(object sender, EventArgs e)
        {
            // richWordMeanning.ScrollBars = RichTextBoxScrollBars.None;

        }

        private void mainForm_MouseClick(object sender, MouseEventArgs e)
        {
            panelRight.Visible = false;
            panelParagraph.Visible = false;
        }

        private void panelRight_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonFavourite_Click(object sender, EventArgs e)
        {
            // đổi tên nút back
            buttonBack.Text = Favourite.Text;
            ShowUpFavouriteHistory(Favourite.Text);
        }
        private void buttonHistory_Click(object sender, EventArgs e)
        {
            // đổi tên nút back
            buttonBack.Text = History.Text;
            ShowUpFavouriteHistory(History.Text);

        }
        private void ShowUpFavouriteHistory(string HistoryOrFavourite)
        {

            // clear all list
            listFavouriteHistory.Items.Clear();
            // set location and visibility for panel
            panelFavouriteAndHistory.Visible = true;
            PanelList.Dock = DockStyle.Fill;
            // show up favourite list

            // hien thi panelRight
            panelRight.Visible = true;
            panelRight.Location = new Point(0, 0);
            //  Đọc dữ liệu từ file history
            if (HistoryOrFavourite == History.Text)
            {

                try
                {
                    if (File.Exists("History.txt"))
                    {
                        PanelNothing.Visible = false;
                        PanelList.Visible = true;
                        foreach (string line in FileFactory.TextInFileToStringArray("History.txt"))
                            //listFavouriteHistory.Items.Add(line);
                            if (line != "")
                                listFavouriteHistory.Items.Add(line);
                    }
                    else
                    {
                        PanelNothing.Visible = true;
                        PanelList.Visible = false;
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show("Error when try to read History list: " + error.ToString());
                }
            }
            else
            {
                try
                {
                    if (File.Exists("Favourite.txt"))
                    {
                        PanelNothing.Visible = false;
                        PanelList.Visible = true;
                        foreach (string line in FileFactory.TextInFileToStringArray("Favourite.txt"))
                            if (line != "")
                                listFavouriteHistory.Items.Add(line);


                    }
                    else
                    {
                        PanelNothing.Visible = true;
                        PanelList.Visible = false;
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show("Error when try to read Favourite list: " + error.ToString());
                }
            }
        }

        private void listFavourite_SelectedIndexChanged(object sender, EventArgs e)
        {

            RemoveItem.Visible = true;
            if (listFavouriteHistory.SelectedItem != null)
                temperTextSearch.Text = listFavouriteHistory.SelectedItem.ToString();
            searchMatchWord(temperTextSearch.Text, out bool founded);

        }

        private void buttonGoogleTranslate_Click(object sender, EventArgs e)
        {

            new paragraphTranslate().Show();

        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            contextMenuSettings.Show(Cursor.Position);

        }

        private void buttonDailyWord_Click(object sender, EventArgs e)
        {
            MessageBox.Show(errorList.stillDevelopingFunction);
        }

        private void buttonCollocation_Click(object sender, EventArgs e)
        {
            MessageBox.Show(errorList.stillDevelopingFunction);
        }

        private void buttonBack_MouseEnter(object sender, EventArgs e)
        {
            buttonBack.BackColor = Color.FromArgb(38, 157, 237);
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            panelFavouriteAndHistory.Visible = false;
            panelRight.Visible = false;
            listFavouriteHistory.DataSource = null;
        }

        private void buttonBack_MouseLeave(object sender, EventArgs e)
        {
            buttonBack.BackColor = Color.FromArgb(0, 122, 204);
        }

        private void panelParagraph_Paint(object sender, PaintEventArgs e)
        {

        }

        private void soundForWord_Click(object sender, EventArgs e)
        {
            Sound player = new Sound();
            WordFactory word = new WordFactory();
            word.PreWord = temperTextSearch.Text;
            word.SearchWordProcess();
            player.SoundUrl = word.OnlineUrlProcess("us");
            player.OnlinePlay();
            if (player.OnlinePlay() == false)
            {
                player.MachinePlay(word.OutWord);
            }


        }

        private void soundForWordUK_Click(object sender, EventArgs e)
        {
            Sound player = new Sound();
            WordFactory word = new WordFactory();
            word.PreWord = temperTextSearch.Text;
            word.SearchWordProcess();
            player.SoundUrl = word.OnlineUrlProcess("uk");
            player.OnlinePlay();
            if (player.OnlinePlay() == false)
            {
                player.MachinePlay(word.OutWord);
            }
        }

        private void heartForWord_Click(object sender, EventArgs e)
        {
            heartForWord.Image = Properties.Resources.heart_32_light_filled;

            FileFactory.AddItem(temperTextSearch.Text, "Favourite.txt");
        }
        private void settings_Click(object sender, EventArgs e)
        {
            Settings setup = new Settings();
            setup.Show();
        }
        private void about_Click(object sender, EventArgs e)
        {
            about info = new about();
            info.Show();
        }
        private void SearchByVoice_Click(object sender, EventArgs e)
        {
            textSearch.Text = "Listening...";
            WordFactory word = new WordFactory();
            word.PreWord = textSearch.Text;
            textSearch.Text = Sound.SpeechToText(word.OutWord);

        }

        private void NewTextSearch_KeyDown(object sender, KeyEventArgs e)
        {
            WordFactory relatedtext = new WordFactory();
            RelatedMeaning.DataSource = relatedtext.RelatedText(textSearch.Text, DataProcessing.AllRelatedText);
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

                        panelParagraph.Visible = true;
                        // gán từ đã tìm thấy vào lịch sử tìm kiếm 
                        // tempertext là nơi để từ đã được tìm kiếm và đã tìm thấy
                        FileFactory.AddItem(word, "History.txt");
                        // hiển thị panel dịch thuật và hiệu ứng thu vào mở ra
                        if (panelRight.Visible == false)
                        {
                            panelRight.Visible = false;
                            panelRight.Location = new Point(0, 0);
                            animatePanelRight.ShowSync(panelRight);
                        }
                    }


                }
            }
        }

        private void NewTextSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            // tắt âm thanh Ding sau khi bấm enter ở mục tìm kiếm
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
            }
        }

        public void NewTextSearch_TextChanged(object sender, EventArgs e)
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
        private void CloseSuggestWords_Click(object sender, EventArgs e)
        {
            panelParagraph.Visible = false;
        }

        private void DelAllFavoutiteHistory_Click(object sender, EventArgs e)
        {
            // confirm delete favourite or history list
            DialogResult answer = MessageBox.Show("Do you want to delete it?", "Warning", MessageBoxButtons.YesNo);
            if (answer == DialogResult.Yes)
                if (buttonBack.Text == History.Text)
                {
                    listFavouriteHistory.Items.Clear();
                    FileFactory.DeleteFile("History.txt");
                }
                else
                {
                    listFavouriteHistory.Items.Clear();
                    FileFactory.DeleteFile("Favourite.txt");
                }

        }

        private void RemoveItem_Click(object sender, EventArgs e)
        {

            RemoveItem.Visible = false;
            listFavouriteHistory.Items.RemoveAt(listFavouriteHistory.SelectedIndex);
            if (buttonBack.Text == History.Text)
            {
                FileFactory.RemoveItem(temperTextSearch.Text, "History.txt");
            }
            else
            {
                FileFactory.RemoveItem(temperTextSearch.Text, "Favourite.txt");
            }
        }

        private void sortalpha_Click(object sender, EventArgs e)
        {
            ArrayList ListTemper = new ArrayList();

            for (int i = listFavouriteHistory.Items.Count - 1; i >= 0; i--)
            {
                if (listFavouriteHistory.Items[i].ToString() != "")
                {
                    ListTemper.Add(listFavouriteHistory.Items[i].ToString());
                }
            }

            listFavouriteHistory.DataSource = ListTemper;

        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            // save setting of form size
            Properties.Settings.Default["Size"] = "small";
            Properties.Settings.Default.Save();

            // tìm cái form miniform, nếu nó có tồn tại thì mở
            var formToShow = Application.OpenForms.Cast<Form>().FirstOrDefault(c => c is miniSearchForm);
            if (formToShow != null)
            {
                formToShow.Show();
            }
            else
            {
                miniSearchForm MF = new miniSearchForm();
                MF.Show();
            }
            this.Hide();
        }

        private void print_Click(object sender, EventArgs e)
        {

        }

        private void feature_Paint(object sender, PaintEventArgs e)
        {

        }

        private void mainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void SearchPanel_MouseClick(object sender, MouseEventArgs e)
        {
            panelRight.Visible = false;
            panelParagraph.Visible = false;
        }

        private void SmallSearchPanel_MouseClick(object sender, MouseEventArgs e)
        {
            panelRight.Visible = false;
            panelParagraph.Visible = false;
        }
    }
}

