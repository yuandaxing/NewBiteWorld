using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.CRF4SLU.FeatureType.Base;
using BingIntent.PCFGParser;
using BingIntent.ExpMod;

namespace BingIntent.CRF4SLU.FeatureType
{
    public class BigramFeature : ThreeDimensionFeature
    {
        LexiconCollection lexiconCollection = null;
        IModelFactory factory = null;
        public BigramFeature(string prefix, LexiconCollection lc,
            IModelFactory factory) :
            base(LexType.State, LexType.Obervation, LexType.Obervation, lc, prefix)
        {
            this.lexiconCollection = lc;
            this.factory = factory;
        }
        public override void CreateFeature(ISampleStream stream)
        {
            ISample sample = null;
            stream.Rewind();
            int start = lexiconCollection.StartDoc;
            while ((sample = stream.Next()) != null)
            {
                int[] ob = sample.GetObservation();
                int[] states = sample.GetStates();
                for (int i = 0; i < ob.Length; i++)
                {
                    if (sample.IsBoundary(i))
                    {
                        MakeFeature(states[i], start, ob[i]);
                    }
                    else
                    {
                        MakeFeature(states[i], ob[i - 1], ob[i]);
                    }
                }
            } 
        }
        public override int RetrieveFeatureList(FeatureCountList features, 
            ISample sample, int fromFrame, int toFrame, int fromState, int toState)
        {
            int count = 0;
            if (fromFrame != toFrame)
            {
                int start = lexiconCollection.StartDoc;
                int[] ob = sample.GetObservation();
                
                for (int i = fromFrame; i < toFrame; i++)
                {
                    int fid = -1;
                    if (i == fromFrame)
                    {
                        fid = Retrieve(fromState, start, ob[i]);
                    }
                    else
                    {
                        fid = Retrieve(fromState, ob[i - 1], ob[i]);
                    }

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
