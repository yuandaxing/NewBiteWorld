using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.CRF4SLU.FeatureType.Base;
using BingIntent.PCFGParser;
using BingIntent.ExpMod;

namespace CRF4SLU.FeatureType
{
    public class StateTransition : TwoDimensionFeature
    {
        IModelFactory factory = null;
        public StateTransition(string prefix, LexiconCollection lc,
            IModelFactory factory)
            : base(lc, LexType.State, LexType.State, prefix)
        {
            this.factory = factory;
        }
        public override void CreateFeature(ISampleStream stream)
        {
            int[][] preTran = factory.GetPreviousStateTable();
            int[] allow = factory.GetAllAllowedStates();
            for (int i = 0; i < preTran.Length; i++)
            {
                int[] previous = preTran[i];
                for (int j = 0; j < previous.Length; j++)
                {
                    int prev = allow[previous[j]];
                    int next = allow[i];
                    MakeFeature(prev, next);
                }
            }
        }
        public override int RetrieveFeatureList(FeatureCountList features, ISample sample, int fromFrame, int toFrame, int fromState, int toState)
        {
            int count = 0;
            if (fromFrame == toFrame)
            {
                int fid = Retrieve(fromState, toState);
                if (fid >= 0)
                {
                    features.Add(fid, 1.0);
                    count++;
                }
            }
            return count;
        }
    }
}
