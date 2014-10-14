using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.CRF4SLU.FeatureType.Base;
using BingIntent.PCFGParser;
using BingIntent.ExpMod;
using BingIntent.Common;

namespace BingIntent.CRF4SLU.FeatureType.BaseType
{
    //this feature is design for state and observation
    public abstract class WindowType : TwoDimensionFeature
    {
        private int leftOffset, rightOffset;
        public WindowType(string prefix, LexiconCollection lexiconCollection,
            IModelFactory factory, int leftOffset, int rightOffset)
            : base(lexiconCollection, LexType.State, LexType.Obervation, prefix)
        {
            this.leftOffset = leftOffset;
            this.rightOffset = rightOffset;
        }
        public override void CreateFeature(ISampleStream stream)
        {
            ISample sample = null;
            stream.Rewind();
            while ((sample = stream.Next()) != null)
            {
                int[] ob = sample.GetObservation();
                int[] states = sample.GetStates();
                for (int i = 0; i < ob.Length; i++)
                {
                    int left = Math.Max(0, i + leftOffset);
                    int right = Math.Min(ob.Length,  i + rightOffset);
                    for (int j = left; j < right; j++)
                    {
                        MakeFeature(states[i], ob[j]);
                    }
                }
            }
        }
        public int RetrieveFeatureList(FeatureCountList features, ISample sample, int frame, int state)
        {
            int count = 0;
            int[] ob = sample.GetObservation();
            int left = Math.Max(0, frame + leftOffset);
            int right = Math.Min(ob.Length, frame + rightOffset);
            for (int i = left; i < right; i++)
            {
                int fid = Retrieve(state, ob[i]);
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
