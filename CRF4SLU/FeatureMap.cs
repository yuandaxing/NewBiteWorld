using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.PCFGParser;
using BingIntent.ExpMod;
using BingIntent.CRF4SLU.FeatureType;
using BingIntent.CRF4SLU.FeatureType.Base;
using CRF4SLU.FeatureType;

namespace BingIntent.CRF4SLU
{
    public abstract class FeatureMap : StateSpace
    {
        private Dictionary<string, IFeatureType> string2FeatureType;
        private List<IFeatureType> featureTypes;
        Schema schema;
        ModelSetting modelSetting;
        LexiconCollection lexiconCollection;
        public void EnableFeature()
        {
            string2FeatureType = new Dictionary<string, IFeatureType>() {
                {"UG", new UniGramFeature("UG", this.lexiconCollection, this)},
                {"PRW", new PrevWordFeature("PRW", this.lexiconCollection, this)},
                {"NXW", new NextWordFeature("NXW", this.lexiconCollection, this)},
                {"SGS", new SegmentStartFeature("SGS", this.lexiconCollection, this)},
                {"SGE", new SegmentEndFeature("SGE", this.lexiconCollection, this)},
                {"BI", new BigramFeature("BI", this.lexiconCollection, this)},
                {"TR", new StateTransition("TR", this.lexiconCollection, this)},
                {"SGL", new SegmentLenFeature("SGL", this.lexiconCollection, this)}
            };

            foreach (string featureStr in modelSetting.featureList)
            {
                if (string2FeatureType.ContainsKey(featureStr))
                {
                    featureTypes.Add(string2FeatureType[featureStr]);
                    Console.WriteLine("enable {0}", featureStr);
                }
            }
        }

        public override void LoadModel(List<string> modelLines)
        {
            foreach (IFeatureType feature in featureTypes)
            {
                feature.LoadFeature(modelLines);
            }
        }

        public FeatureMap(ModelSetting setting, Schema schema, LexiconCollection lc) :
            base(setting, schema, lc)
        {
            this.modelSetting = setting;
            featureTypes = new List<IFeatureType>();
            this.schema = schema;
            this.lexiconCollection = lc;
        }

        public override int RetrieveFeatureList(ExpMod.FeatureCountList features,
            ExpMod.ISample sample, int fromFrame, int toFrame, int fromState, int toState)
        {
            int sum = 0;
            foreach (IFeatureType ft in featureTypes)
            {
                sum += ft.RetrieveFeatureList(features, sample, fromFrame, toFrame,
                    fromState, toState);
            }
            return sum;
        }

        public void CreateFeatures(ISampleStream stream)
        {
            foreach (IFeatureType ft in featureTypes)
                ft.CreateFeature(stream);
        }
    }
}
