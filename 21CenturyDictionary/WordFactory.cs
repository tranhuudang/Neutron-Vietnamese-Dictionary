using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
// Class xử lí các vấn đề liên quan tới chữ, link và địa chỉ file âm thanh
namespace NeutronDictionary
{

    class WordFactory
    {
        private string preWord = "none";
        private string onlineUrlOfUk;
        private string onlineUrlOfUs;
        //private string offlineAddress="none";
        private string outWord;
        private string preParagraph = " ";
        private string outParagraph = " ";

        public string PreWord { get => preWord; set => preWord = value; }
        public string OutWord { get => outWord; set => outWord = value; }
        public string PreParagraph { get => preParagraph; set => preParagraph = value; }
        public string OutParagraph { get => outParagraph; set => outParagraph = value; }

        // Xử lí Related Text
        public ArrayList RelatedText(string Word, string[] InputArray)
        {

            ArrayList RelatedTextSource = new ArrayList();
            string[] BunchRelated= null;
            int arrIndex = 0;
            int LimitOfList = 0; // limit of list is 28
            int max = 28;
            string SearchText = Word + "--";
            foreach (string text in InputArray)
            {
                if (text.Contains(SearchText))
                {
                    BunchRelated = InputArray[arrIndex].Substring(SearchText.Length+1).Split(" ".ToCharArray());
                    foreach (string textInBunch in BunchRelated)
                    {
                        RelatedTextSource.Add(textInBunch);
                        LimitOfList++;
                        if (LimitOfList == max) break;
                    }
                    return RelatedTextSource;
                    
                }
                arrIndex++;
            }
            return RelatedTextSource;


        }
        // Xử lí đầu ra cho Translate Paragraph
        public string TranslateParagraphProcess()
        {
            try
            {
                //translate.google.com/?sl=en&tl=vi&text=english%20is%20a%20language%20in%20the%20world&op=translate
                // lỗi này đã được bắt và không nhất thiết phải sửa nữa
                outParagraph = preParagraph.Replace(" ", "%20");
                outParagraph = "http://translate.google.com/?sl=en&tl=vi&text=" + outParagraph + "&op=translate";
                return outParagraph;
            }
            catch (Exception error)
            {
                error.ToString();
                outParagraph = "http://translate.google.com";
            }
            return outParagraph;
        }
        // Xử lí từ đầu vào thành một từ chuẩn cho quá trình tìm kiếm từ trong Database
        public string SearchWordProcess()
        {
            outWord = preWord;
            outWord = outWord.ToLower();
            outWord = outWord.Replace("\n", "").Replace("\r", ""); // delete \n in c#
            return outWord;
        }
        // Xử lí từ đầu vào thành một cái link âm thanh theo định dạng link của Oxford Dictionary
        public string OnlineUrlProcess(string UsOrUk)
        {

            string word = "/" + PreWord;
            string firstLetter = "/" + word.Substring(1, 1);
            string threeWordNext = "";
            string fourWordNext = "";
            string totalWord = "";
            string usSpeak_tail = "";
            if (UsOrUk.ToLower() == "uk")
            {

                // Vì độ dài của tên từ vựng đó khiến link file âm thanh thay đổi nên phải tiến hành xử lí để 
                // có thể tạo ra link phù hợp với link file.
                if (PreWord.Length > 4)
                {
                    threeWordNext = "/" + word.Substring(1, 3);
                    fourWordNext = "/" + word.Substring(1, 5);

                }
                else
                {
                    // 1 letter: www.oxfordlearnersdictionaries.com/media/english/us_pron/o/o__/o__us/o__us_1.mp3
                    // 2 letter: www.oxfordlearnersdictionaries.com/media/english/us_pron/o/on_/on__u/on__us_1_rr.mp3
                    // 3 letter: www.oxfordlearnersdictionaries.com/media/english/us_pron/t/ten/ten__/ten__us_1.mp3
                    // 4 letter: www.oxfordlearnersdictionaries.com/media/english/us_pron/f/fou/four_/four__us_1.mp3
                    if (PreWord.Length == 1)
                    {
                        threeWordNext = "/" + PreWord + "__";
                        fourWordNext = "/" + PreWord + "__gb";
                    }
                    if (PreWord.Length == 2)
                    {
                        threeWordNext = "/" + PreWord + "_";
                        fourWordNext = "/" + PreWord + "__g";
                    }
                    if (PreWord.Length == 3)
                    {
                        threeWordNext = "/" + PreWord;
                        fourWordNext = "/" + PreWord + "__";
                    }
                    if (PreWord.Length == 4)
                    {
                        threeWordNext = "/" + PreWord.Substring(0, 3);
                        fourWordNext = "/" + PreWord + "_";
                    }
                }


                usSpeak_tail = word + "__gb_1.mp3";
                totalWord = firstLetter + threeWordNext + fourWordNext + usSpeak_tail;
                onlineUrlOfUk = "https://www.oxfordlearnersdictionaries.com/media/english/uk_pron" + totalWord;
                return onlineUrlOfUk;
            }
            else
            {

                // Vì độ dài của tên từ vựng đó khiến link file âm thanh thay đổi nên phải tiến hành xử lí để 
                // có thể tạo ra link phù hợp với link file.
                if (PreWord.Length > 4)
                {
                    threeWordNext = "/" + word.Substring(1, 3);
                    fourWordNext = "/" + word.Substring(1, 5);

                }
                else
                {
                    if (PreWord.Length == 1)
                    {
                        threeWordNext = "/" + PreWord + "__";
                        fourWordNext = "/" + PreWord + "__us";
                    }
                    if (PreWord.Length == 2)
                    {
                        threeWordNext = "/" + PreWord + "_";
                        fourWordNext = "/" + PreWord + "__u";
                    }
                    if (PreWord.Length == 3)
                    {
                        threeWordNext = "/" + PreWord;
                        fourWordNext = "/" + PreWord + "__";
                    }
                    if (PreWord.Length == 4)
                    {
                        threeWordNext = "/" + PreWord.Substring(0,3);
                        fourWordNext = "/" + PreWord + "_";
                    }
                }

                //https://www.oxfordlearnersdictionaries.com/media/english/us_pron/t/til/till_/till__us_1.mp3
                //https://www.oxfordlearnersdictionaries.com/media/english/us_pron/t/til
                usSpeak_tail = word + "__us_1.mp3";
                totalWord = firstLetter + threeWordNext + fourWordNext + usSpeak_tail;
                onlineUrlOfUs = "https://www.oxfordlearnersdictionaries.com/media/english/us_pron" + totalWord;
                return onlineUrlOfUs;
            }

        }
    }
}
