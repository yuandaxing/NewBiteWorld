using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace BingIntent.PCFGParser.testPCFGParser
{
    class BigramTest
    {
        public static void test()
        {
            Lexicon lex = new Lexicon("test", LexType.Obervation);
            int id1 = lex.AddGet("what");
            int id2 = lex.AddGet("is");
            Debug.Assert(lex.GetWord(id1) == "what");
            Debug.Assert(lex.GetWord(id2) == "is");
            BigramLexicon bl = new BigramLexicon();
            int bid = bl.AddGet(id1, id2);
            KeyValuePair<int, int> kv = bl.GetBigram(bid), kv1 = new KeyValuePair<int, int>(id1, id2);
            Debug.Assert( kv1.Equals(kv));
            Console.WriteLine("pass bigram testing....");
        }
    }
}
