using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.CRF4SLU.FeatureType.BaseType;
using BingIntent.PCFGParser;
using BingIntent.ExpMod;

namespace CRF4SLU.FeatureType
{
    public class PrevWordFeature : WindowType
    {
        public PrevWordFeature(string prefix, LexiconCollection lc,
            IModelFactory factory)
            : base(prefix, lc, factory, -1, 0)
        {
        }
        public override int RetrieveFeatureList(FeatureCountList features, 
            ISample sample, int fromFrame, int toFrame, int fromState, int toState)
        {
            int count = 0;
            if (fromFrame != toFrame)
            {
                count = RetrieveFeatureList(features, sample, fromFrame, fromState);
            }
            return count;
        }
    }
}
