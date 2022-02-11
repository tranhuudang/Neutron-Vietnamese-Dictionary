using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Windows.Forms;

namespace NeutronDictionary
{
    /// <summary>
    ///  Class xử lí các vấn đề về âm thanh
    /// </summary>
    class Sound
    {
        private string soundUrl;
        private string fileAddress;

        public string SoundUrl { get => soundUrl; set => soundUrl = value; }
        public string FileAddress { get => fileAddress; set => fileAddress = value; }

        WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();

        public bool OnlinePlay()
        {
            try
            {

                if (player.isOnline == true) // Xác định xem máy tính có đang kết nối mạng không
                {
                    player.URL = soundUrl;
                    player.controls.play();
                }
                else
                {
                    return false;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return false;
            }
            return true;
        }
        public bool MachinePlay(string Word)
        {
            // Sử dụng máy đọc nếu như file âm thanh online  và offline không sẵn dùng
            try
            {
                SpeechSynthesizer machineSound = new SpeechSynthesizer(); // cần thêm thư viện System.Speech ở Project- Add Reference
                machineSound.Volume = 100;  // 0...100 âm lượng
                machineSound.Rate = -2;     // -10...10
                machineSound.SpeakAsync(Word); // phát âm từ
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return false;
            }
            return true;
        }
        public bool OfflinePlay()
        {
            // Phát âm thanh được tải về trong máy
            player.URL = fileAddress;
            player.controls.play();
            return false;
        }
        // Speech To Text 
        public static string SpeechToText(string waitingtime)
        {
            // waiting time format : "00:00:07"
            string Word="";
            SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();
            Grammar dictationGrammar = new DictationGrammar();
            recognizer.LoadGrammar(dictationGrammar);
            try
            {
                recognizer.SetInputToDefaultAudioDevice();
                System.TimeSpan time;
                System.TimeSpan.TryParse(waitingtime, out time);
                RecognitionResult result = recognizer.Recognize(time);

                if (result == null)
                {
                    Word = "";
                }
                else
                {
                    Word = result.Text;
                }

            }
            catch (InvalidOperationException error)
            {
                MessageBox.Show(error.ToString());
            }
            finally
            {
                recognizer.UnloadAllGrammars();
            }
            return Word;
        }


    }


}

