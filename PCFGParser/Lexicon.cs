using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
namespace BingIntent
{
    namespace PCFGParser
    {
        /// <summary>
        /// Slot is Tag, and state is PreTag, Tag, Begin, end and so on
        /// </summary>
        public enum LexType
        {
            Obervation = 0,
            Slot,
            State,
            Feature,
            CFG,
            Dynamic,
            UnDefine
        };
        public class Lexicon
        {
            private LexType lexType = LexType.UnDefine;
            private string description = null;
            Dictionary<string, int> str2WordID = null;
            List<string> wordID2Str = null;
            public Lexicon(string des, LexType lexType)
            {
                this.description = des;
                this.lexType = lexType;
                str2WordID = new Dictionary<string, int>();
                wordID2Str = new List<string>();
            }
            public int Get(string word)
            {
                if (str2WordID.ContainsKey(word))
                    return str2WordID[word];
                return -1;
            }
            public int AddGet(string word)
            {
                if (!str2WordID.ContainsKey(word))
                {
                    str2WordID[word] = wordID2Str.Count();
                    wordID2Str.Add(word);
                }
                return Get(word);
            }
            public string GetWord(int id)
            {
                Debug.Assert(id < wordID2Str.Count);
                return wordID2Str[id];
            }
            public int Count { get { return wordID2Str.Count; } }
            public string this[int idx]
            {
                get { return wordID2Str[idx]; }
                set { wordID2Str[idx] = value; }
            }
        }
    }
}
