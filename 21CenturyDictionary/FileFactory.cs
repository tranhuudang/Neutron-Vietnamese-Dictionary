using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NeutronDictionary
{
    public class FileFactory
    {

        public static void AddItem(string word, string filepath)
        {
            // thêm từ mới vào file favourite hoặc history
            string root = Path.GetDirectoryName(filepath);
            if (FileContain(word, filepath) == -1)
            {
                StreamWriter write = new StreamWriter(filepath, true);
                write.WriteLine(word);
                write.Close();
            }
        }
        public static void RemoveItem(string word, string filepath)
        {
            int SearchValue = FileContain(word, filepath);
            // nếu tìm thấy được từ đó trong file
            if (SearchValue != -1)
            {
                string[] AllText = TextInFileToStringArray(filepath);
                File.Delete(filepath);
                StreamWriter write = new StreamWriter(filepath, true);
                AllText[SearchValue-1] = "";
                
                foreach (string Line in AllText)
                {
                    if (Line != "")
                    {
                        write.WriteLine(Line);
                    }
                }
                write.Close();

            }
        }
        public static bool DeleteFile(string filepath)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string[] TextInFileToStringArray(string filepath)
        {
            StreamReader textFile = new StreamReader(filepath);
            string[] texts = textFile.ReadToEnd().Split('\n');
            textFile.Close();
            return texts;
        }
        public static int FileContain(string word, string filepath)
        {
            WordFactory wordInDuty = new WordFactory();
            wordInDuty.PreWord = word;
            if (File.Exists(filepath))
            {
                string[] content = TextInFileToStringArray(filepath);
                int count = 0;
                foreach (string line in content)
                {
                    count++;
                    if (line.Contains(wordInDuty.PreWord))
                    {
                        // trả về giá trị là vị trí của từ trong chuỗi lấy ra từ file.
                        return count;
                    }
                }
            }
            // trả về giá trị -1 nếu không tìm thấy từ đang tìm trong file.
            return -1;
        }
    }
}
