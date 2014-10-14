using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.CRF4SLU.FeatureType.Base;
using BingIntent.PCFGParser;
using BingIntent.ExpMod;
using System.Diagnostics;
using BingIntent.Common;

namespace CRF4SLU.FeatureType
{
    public class SegmentLenFeature : TwoDimensionFeature
    {
        LexiconCollection lexiconCollection;
        IModelFactory factory;
        public SegmentLenFeature(string prefix, LexiconCollection lexiconCollection,
            IModelFactory factory) :
            base(lexiconCollection, LexType.State, LexType.UnDefine, prefix)
        {
            this.lexiconCollection = lexiconCollection;
            this.factory = factory;
        }
        public override void CreateFeature(ISampleStream stream)
        {
            ISample sample = null;

            stream.Rewind();
            HashSet<int> lens = new HashSet<int>();
            while ((sample = stream.Next()) != null)
            {
                int prev = 0;
                for (int i = 0; i <= sample.GetSize(); i++)
                {
                    if (sample.IsBoundary(i))
                    {
                        int len = i - prev;
                        prev = i;
                        if (len > 0)
                            lens.Add(len);
                    }
                }
            }
            int[] states = factory.GetAllAllowedStates();
            for (int i = 0; i < states.Length; i++)
            {
                int state = states[i];
                foreach (int len in lens)
                {
                    string stateStr = lexiconCollection.GetWord(LexType.State, state);
                    string feature = string.Join("#", prefix, stateStr, len.ToString());
                    int fid = lexiconCollection.AddGetLexID(LexType.Feature, feature);
                    NewTuple<int, int> twoWord = new NewTuple<int, int>(state, len);
                    words2Feature[twoWord] = fid;
                }
            }
        }
        public override void LoadFeature(List<string> modelLines)
        {
            string prevStr = prefix + "#";
            foreach (string line in modelLines)
            {
                if (line.StartsWith(prevStr))
                {
                    string feature = line.Split(new char[] { ' ' })[0];
                    string[] tk = feature.Split(new char[] { '#' });
                    Debug.Assert(tk.Length == 3);
                    int w1 = lexiconCollection.GetLexID(LexType.State, tk[1]);
                    int w2 = int.Parse(tk[2]);
                    int featureID = lexiconCollection.AddGetLexID(LexType.Feature, feature);
                    words2Feature[new NewTuple<int, int>(w1, w2)] = featureID;
                }
            }
        }
        public override int RetrieveFeatureList(FeatureCountList features, ISample sample, int fromFrame, int toFrame, int fromState, int toState)
        {
            int count = 0;
            if (fromFrame != toFrame)
            {
                //Tuple<int, int> twow = new Tuple<int, int>(fromState, toFrame - fromFrame);
                //if (words2Feature.ContainsKey(twow))
                //{
                //    features.Add(words2Feature[twow], 1.0);
                //    count++;
                //}
                int fid = Retrieve(fromState, toFrame - fromFrame);
                if (fid != -1)
                {
                    features.Add(fid, 1.0);
                    count++;
                }
            }
            return count;
        }
    }
}
