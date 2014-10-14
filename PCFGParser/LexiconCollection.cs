using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent
{
    namespace PCFGParser
    {
        public class BigramLexicon
        {
            private Dictionary<KeyValuePair<int, int>, int> words2ID = null;
            private List<KeyValuePair<int, int>> ID2wordIDs = null;
            public BigramLexicon()
            {
                words2ID = new Dictionary<KeyValuePair<int, int>, int>();
                ID2wordIDs = new List<KeyValuePair<int, int>>();
            }
            public int Get(int wordID1, int wordID2)
            {
                KeyValuePair<int, int> key = new KeyValuePair<int, int>(wordID1, wordID2);
                if (words2ID.ContainsKey(key))
                    return words2ID[key];
                return -1;
            }
            public int AddGet(int wordID1, int wordID2)
            {
                KeyValuePair<int, int> key = new KeyValuePair<int, int>(wordID1, wordID2);
                if (!words2ID.ContainsKey(key))
                {
                    words2ID[key] = ID2wordIDs.Count;
                    ID2wordIDs.Add(key);
                }
                return words2ID[key];
            }
            public KeyValuePair<int, int> GetBigram(int id)
            {
                return ID2wordIDs[id];
            }

        }
        public class LexiconCollection
        {
            private Lexicon[] collections = null;
            private BigramLexicon bigram = null;
            private LexType[] lexTypes = {LexType.Obervation, LexType.Slot, LexType.State,
                                     LexType.Feature, LexType.CFG, LexType.Dynamic };
            static private LexiconCollection instance = null;
            private int beginDoc = -1, endDoc = -1;

            private LexiconCollection()
            {
                bigram = new BigramLexicon();
                collections = new Lexicon[lexTypes.Length];
                for (int i = 0; i < lexTypes.Length; i++)
                    collections[i] = new Lexicon(lexTypes[i].ToString(), lexTypes[i]);
                beginDoc = collections[(int)LexType.Obervation].AddGet("startdoc");
                endDoc = collections[(int)LexType.Obervation].AddGet("enddoc");
            }
            private static LexiconCollection Instance()
            {
                if (instance == null)
                {
                    instance = new LexiconCollection();
                }
                return instance;
            }
            public static LexiconCollection I { get { return Instance(); } }
            public int StartDoc { get { return beginDoc; } }
            public int EndDoc { get { return endDoc; } }

            public int GetBigramID(int wordID1, int wordID2) { return bigram.Get(wordID1, wordID2); }
            public int AddGetBigramID(int wordID1, int wordID2) { return bigram.AddGet(wordID1, wordID2); }
            public KeyValuePair<int, int> GetBigram(int bigramID) { return bigram.GetBigram(bigramID); }
            public int GetLexID(LexType lexType, string word) { return collections[(int)lexType].Get(word); }
            public int AddGetLexID(LexType lexType, string word) { return collections[(int)lexType].AddGet(word); }
            public string GetWord(LexType lexType, int ID) { return collections[(int)lexType].GetWord(ID); }

            public List<int> Sentence2IDs(List<string> sentence)
            {
                List<int> IDs = new List<int>();
                Lexicon lex = collections[(int)LexType.Obervation];
                for (int i = 0; i < sentence.Count; i++)
                    IDs.Add(lex.Get(sentence[i]));
                return IDs;
            }
            public int GetCount(LexType e)
            {
                return collections[(int)e].Count;
            }
            public Lexicon GetLexicon(LexType e)
            {
                return collections[(int)e];
            }
        }
    }
}
