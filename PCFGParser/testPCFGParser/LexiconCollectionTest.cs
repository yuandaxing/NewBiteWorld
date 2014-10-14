using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace BingIntent.PCFGParser.testPCFGParser
{
    class LexiconCollectionTest
    {
        public static void test()
        {
            LexiconCollection lc1 = LexiconCollection.I, lc2 = LexiconCollection.I;
            Debug.Assert(lc1 == lc2);
            string[] sentence = { "what", "is", "the", "weather", "today" };
            for (int i = 0; i < sentence.Length; i++)
                lc1.AddGetLexID(LexType.Obervation, sentence[i]);
            List<int> IDs = LexiconCollection.I.Sentence2IDs(sentence.ToList());
            for (int i = 0; i < IDs.Count; i++)
                Debug.Assert(lc1.GetWord(LexType.Obervation, IDs[i]) == sentence[i]);
            Debug.Assert(lc1.GetWord(LexType.Obervation, lc1.StartDoc) == "startdoc");
            Debug.Assert(lc1.GetWord(LexType.Obervation, lc1.EndDoc) == "enddoc");
            Console.WriteLine("pass lexicon collection testing");

            KeyValuePair<LexType, string>[] testSet = { 
                                  new KeyValuePair<LexType, string>(LexType.Obervation, "haha"),
                                  new KeyValuePair<LexType, string>(LexType.CFG, "rule1"),
                                  new KeyValuePair<LexType, string>(LexType.Dynamic, "Dynamic"),
                                  new KeyValuePair<LexType, string>(LexType.Feature, "feature"),
                                  new KeyValuePair<LexType, string>(LexType.Slot, "weather"),
                                  new KeyValuePair<LexType, string>(LexType.State, "previous"),
                                                      };

            for (int i = 0; i < testSet.Length; i++)
            {
                Debug.Assert(lc2.GetWord(testSet[i].Key, lc1.AddGetLexID(testSet[i].Key, testSet[i].Value))
                    == testSet[i].Value);
                Debug.Assert(lc1.AddGetLexID(testSet[i].Key, testSet[i].Value)
                    == lc2.GetLexID(testSet[i].Key, testSet[i].Value));
            }

            for (int i = 1; i < IDs.Count; i++)
            {
                int bigramID = lc1.AddGetBigramID(IDs[i-1], IDs[i]);
                Debug.Assert(bigramID == lc2.GetBigramID(IDs[i-1], IDs[i]));
                KeyValuePair<int, int> kv = new KeyValuePair<int, int>(IDs[i - 1], IDs[i]);
                Debug.Assert(lc1.GetBigram(bigramID).Equals(kv));
            }

            Console.WriteLine("pass lexicons testing");
        }
    }
}
