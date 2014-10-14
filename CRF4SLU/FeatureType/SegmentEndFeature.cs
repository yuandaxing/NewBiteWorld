using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.PCFGParser;
using BingIntent.ExpMod;
using BingIntent.CRF4SLU.FeatureType.Base;
using BingIntent.CRF4SLU.FeatureType.BaseType;

namespace BingIntent.CRF4SLU.FeatureType
{
    public class SegmentEndFeature : WindowType
    {
        public SegmentEndFeature(string prefix, LexiconCollection lc, IModelFactory factory)
            : base(prefix, lc, factory, 0, 1)
        {
        }
        public override int RetrieveFeatureList(FeatureCountList features, ISample sample, int fromFrame, int toFrame, int fromState, int toState)
        {
            int count = 0;
            if (fromFrame != toFrame)
            {
                count = RetrieveFeatureList(features, sample, toFrame-1, fromState);
            }
            return count;
        }
    }
}
