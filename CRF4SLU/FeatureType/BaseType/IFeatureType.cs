using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.ExpMod;

namespace BingIntent.CRF4SLU.FeatureType.Base
{
    public abstract class IFeatureType
    {
        public abstract int RetrieveFeatureList(FeatureCountList features,
            ISample sample, int fromFrame, int toFrame, int fromState, int toState);
        public abstract void CreateFeature(ISampleStream stream);
        public abstract void LoadFeature(List<string> ModelLines);
    }
}
