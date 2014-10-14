using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.PCFGParser;
using BingIntent.ExpMod;
using System.Diagnostics;
using BingIntent.CRF4SLU.FeatureType.Base;

namespace BingIntent.CRF4SLU.FeatureType
{
    public class UniGramFeature : TwoDimensionFeature
    {
        private string featureName;
        private LexiconCollection lexiconCollection;
        private IModelFactory factory;
        public UniGramFeature(string featureName, LexiconCollection lc, 
            IModelFactory factory) :
            base(lc, LexType.State, LexType.Obervation, featureName)
        {
            this.featureName = featureName;
            this.lexiconCollection = lc;
            this.factory = factory;
        }
        public override void CreateFeature(ISampleStream stream)
        {
            CreateFeature();
            //HashSet<string> hashSetString = new HashSet<string>();
            //ISample sample;
            //stream.Rewind();
            //while ((sample = stream.Next()) != null)
            //{
            //    foreach (string tk in sample.GetObservationString())
            //    {
            //        hashSetString.Add(tk);
            //    }
            //}
            //int startState = factory.GetInitState();
            //int endState = factory.GetFinalState();
            //int[] states = factory.GetAllAllowedStates();
            //for (int i = 0; i < states.Length; i++)
            //{
            //    int state = states[i];
            //    if (state != startState && state != endState)
            //    {
            //        foreach (string tk in hashSetString)
            //        {
            //            int wordID = lexiconCollection.GetLexID(LexType.Obervation, tk);
            //            Tuple<int, int> feature2ID = new Tuple<int,int>(state, wordID);                            ;
            //            words2Feature[feature2ID] = MakeFeature(state, wordID);
            //        }
            //    }
            //}
        }
        public override int RetrieveFeatureList(FeatureCountList features,
            ISample sample, int fromFrame, int toFrame, int fromState, int toState)
        {
            int count = 0;
            if (fromFrame != toFrame)
            {
                Debug.Assert(fromState == toState);
                int[] obervation = sample.GetObservation();
                for (int i = fromFrame; i < toFrame; i++)
                {
                    int fid = Retrieve(fromState, obervation[i]);
                    if (fid >= 0)
                    {
                        features.Add(fid, 1.0);
                        count++;
                    }
                }
            }
            return count;
        }
       
    }
}
