using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.PCFGParser;

namespace BingIntent.CRF4SLU
{
    public class ModelFactory : FeatureMap 
    {
        public ModelFactory(ModelSetting setting, Schema schema,
            LexiconCollection lc)
            : base(setting, schema, lc)
        {

        }
        
    }
}
