using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.PCFGParser;
using BingIntent.Common;
using System.Diagnostics;

namespace BingIntent.CRF4SLU.FeatureType.Base
{
    public abstract class OneDimensionFeature : IFeatureType
    {
        private LexType dimension1;
        private LexiconCollection lexiconCollection;
        private string prefix;
        private int[] wordID2FeatureID;
        public OneDimensionFeature(LexiconCollection lexiconCollectoin, LexType dimension1, string prefix)
        {
            this.dimension1 = dimension1;
            this.lexiconCollection = lexiconCollectoin;
            this.prefix = prefix;
        }
        private int MakeFeature(string entry)
        {
            string feature = string.Join("#", prefix, entry);
            return lexiconCollection.AddGetLexID(LexType.Feature, feature);
        }

        public int CreateFeature()
        {
            Lexicon lex = lexiconCollection.GetLexicon(dimension1);
            wordID2FeatureID = new int[lex.Count];
            ArrayUtil.Fill(wordID2FeatureID, -1);
            for (int i = 0; i < lex.Count; i++)
            {
                wordID2FeatureID[i] = MakeFeature(lex[i]);
            }
            return lex.Count;
        }

        public int Retrieve(int lexiconID)
        {
            return wordID2FeatureID[lexiconID];
        }
    }

    public abstract class TwoDimensionFeature : IFeatureType
    {
        private NewTuple<int, int> temp = new NewTuple<int, int>(0, 0);
        private LexType dimension1, dimension2;
        private LexiconCollection lexiconCollection;
        protected Dictionary<NewTuple<int, int>, int> words2Feature;
        protected string prefix;
        public TwoDimensionFeature(LexiconCollection lexiconCollection,
            LexType D1, LexType D2, string prefix)
        {
            this.lexiconCollection = lexiconCollection;
            this.dimension1 = D1;
            this.dimension2 = D2;
            this.prefix = prefix;
            this.words2Feature = new Dictionary<NewTuple<int, int>, int>();
        }
        public int Retrieve(int ID1, int ID2)
        {
            //Tuple<int, int> words = new Tuple<int, int>(ID1, ID2);
            //if (words2Feature.ContainsKey(words))
            //    return words2Feature[words];
            //return -1;
            temp.v1 = ID1;
            temp.v2 = ID2;
            if (words2Feature.ContainsKey(temp))
                return words2Feature[temp];
            return -1;
        }
        public int MakeFeature(int ID1, int ID2)
        {
            NewTuple<int, int> twoWords = new NewTuple<int, int>(ID1, ID2);
            string str1 = lexiconCollection.GetWord(dimension1, ID1);
            string str2 = lexiconCollection.GetWord(dimension2, ID2);
            string str = string.Join("#", prefix, str1, str2);
            int featureID = lexiconCollection.AddGetLexID(LexType.Feature, str);
            words2Feature[twoWords] = featureID;
            return featureID;
        }
        public void CreateFeature()
        {
            Lexicon lex1 = lexiconCollection.GetLexicon(dimension1);
            Lexicon lex2 = lexiconCollection.GetLexicon(dimension2);
            for (int i = 0; i < lex1.Count; i++)
            {
                for (int j = 0; j < lex2.Count; j++)
                {
                    MakeFeature(i, j);
                }
            }
        }
        public override void LoadFeature(List<string> modelLines)
        {
            string prefixStr = prefix + "#";
            foreach (string line in modelLines)
            {
                if (line.StartsWith(prefixStr))
                {
                    string feature = line.Split(new char[] { ' ' })[0];
                    string[] tk = feature.Split(new char[] { '#' });
                    Debug.Assert(tk.Length == 3);
                    int w1 = lexiconCollection.AddGetLexID(dimension1, tk[1]);
                    int w2 = lexiconCollection.AddGetLexID(dimension2, tk[2]);
                    int featureID = lexiconCollection.AddGetLexID(LexType.Feature, feature);
                    Debug.Assert(w1 != -1 && w2 != -1 && featureID != -1);
                    words2Feature[new NewTuple<int, int>(w1, w2)] = featureID;
                }
            }
        }
    }

    public abstract class ThreeDimensionFeature : IFeatureType
    {
        private string prefix;
        private NewTuple<int, int, int> temp = new NewTuple<int, int, int>(0, 0, 0);
        private Dictionary<NewTuple<int, int, int>, int> threeWord2Feature;
        private LexiconCollection lexiconCollection;
        private LexType dimension1, dimension2, dimension3;
        public ThreeDimensionFeature(LexType D1, LexType D2, LexType D3,
            LexiconCollection lc, string prefix)
        {
            this.dimension1 = D1;
            this.dimension2 = D2;
            this.dimension3 = D3;
            this.prefix = prefix;
            this.lexiconCollection = lc;
            this.threeWord2Feature = new Dictionary<NewTuple<int, int, int>, int>();
        }

        public int Retrieve(int ID1, int ID2, int ID3)
        {
            //Tuple<int, int, int> words = new Tuple<int, int, int>(ID1, ID2, ID3);
            //if (threeWord2Feature.ContainsKey(words))
            //    return threeWord2Feature[words];
            //return -1;
            temp.v1 = ID1;
            temp.v2 = ID2;
            temp.v3 = ID3;
            if (threeWord2Feature.ContainsKey(temp))
                return threeWord2Feature[temp];
            return -1;
        }
        public int MakeFeature(int ID1, int ID2, int ID3)
        {
            NewTuple<int, int, int> words = new NewTuple<int, int, int>(ID1, ID2, ID3);
            string str1 = lexiconCollection.GetWord(dimension1, ID1);
            string str2 = lexiconCollection.GetWord(dimension2, ID2);
            string str3 = lexiconCollection.GetWord(dimension3, ID3);
            string str = string.Join("#", prefix, str1, str2, str3);
            int featureID = lexiconCollection.AddGetLexID(LexType.Feature, str);
            threeWord2Feature[words] = featureID;
            return featureID;
        }
        public override void LoadFeature(List<string> modelLines)
        {
            string prefixStr = prefix + "#";
            foreach (string line in modelLines)
            {
                if (line.StartsWith(prefixStr))
                {
                    string feature = line.Split(new char[] { ' ' })[0];
                    string[] tk = feature.Split(new char[] { '#' });
                    Debug.Assert(tk.Length == 4);
                    int w1 = lexiconCollection.AddGetLexID(dimension1, tk[1]);
                    int w2 = lexiconCollection.AddGetLexID(dimension2, tk[2]);
                    int w3 = lexiconCollection.AddGetLexID(dimension3, tk[3]);
                    int featureID = lexiconCollection.AddGetLexID(LexType.Feature, feature);
                    threeWord2Feature[new NewTuple<int, int, int>(w1, w2, w3)] = featureID;
                }
            }
        }
    }
}
