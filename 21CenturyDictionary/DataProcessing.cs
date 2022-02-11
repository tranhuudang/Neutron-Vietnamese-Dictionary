using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NeutronDictionary
{
    static class DataProcessing
    {
        // pre-process data to improve searching speed
        //public static string allTextInFile = File.ReadAllText("envi_basic.txt");
        public static string allTextInFile = Properties.Resources.envi_basic;
        // contain a full list of all word in english 
        public static string allWordsInFile = Properties.Resources.words300k;
        public static string[] paragraph = allTextInFile.Split("#".ToCharArray());
        public static string[] wordsList = allWordsInFile.Split("\n".ToCharArray());
        // Load Related Text Resource
        public static string[] AllRelatedText = File.ReadAllText("Finizi.ini").Split("#".ToCharArray());
    }
}
