using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.ExpMod
{
    public class Feature2ConsecutiveSpace
    {
        private int[] feature2Consecutive;
        private List<int> consecutive2Feature;
        public Feature2ConsecutiveSpace(int featureSetSize)
        {
            feature2Consecutive = new int[featureSetSize];
            consecutive2Feature = new List<int>();
            for (int i = 0; i < featureSetSize; i++)
            {
                feature2Consecutive[i] = -1;
            }
        }
        public int GetConsecutiveID(int featureID)
        {
            if (feature2Consecutive[featureID] == -1)
            {
                feature2Consecutive[featureID] = consecutive2Feature.Count;
                consecutive2Feature.Add(featureID);
            }
            return feature2Consecutive[featureID];
        }
        public int GetFeatureID(int consecutiveID)
        {
            return consecutive2Feature[consecutiveID];
        }
        public int Count { get { return consecutive2Feature.Count; } }
    }

    public class AccumulatedFeatureCountList
    {
        private List<double> consecutiveFeatureCounts;
        private double featureWeightSum;
        private ExpModel model;
        public void Clear()
        {
            featureWeightSum = 0.0;
            for(int i = 0; i < consecutiveFeatureCounts.Count; i++)
                consecutiveFeatureCounts[i] = 0.0;

        }
        public AccumulatedFeatureCountList(ExpModel model)
        {
            this.model = model;
            consecutiveFeatureCounts = new List<double>();
            featureWeightSum = 0.0;
        }
        private void EnlargeSize(int size)
        {
            for (int i = consecutiveFeatureCounts.Count; i < size; i++)
            {
                consecutiveFeatureCounts.Add(0.0);
            }
        }
        //this may not used for collect forward backward algorithm
        private void AddScaledCount(double scale, AccumulatedFeatureCountList afc)
        {
            if (consecutiveFeatureCounts.Count < afc.consecutiveFeatureCounts.Count)
            {
                EnlargeSize(afc.consecutiveFeatureCounts.Count);
            }
            for (int i = 0; i < afc.consecutiveFeatureCounts.Count; i++)
            {
                consecutiveFeatureCounts[i] += scale * afc.consecutiveFeatureCounts[i];
            }
        }
        public void Add(int feature, double count)
        {
            int consecutiveID = model.GetFeatureConsecutive(feature);
            EnlargeSize(consecutiveID + 1);
            consecutiveFeatureCounts[consecutiveID] += count;
            featureWeightSum += count * model.GetFeatureWeight(feature);
        }
        public void Add(AccumulatedFeatureCountList afc)
        {
            AddScaledCount(1.0, afc);
        }
        public void Subtract(AccumulatedFeatureCountList afc)
        {
            AddScaledCount(-1.0, afc);
        }
        public void SubtractScaledCount(double scale, AccumulatedFeatureCountList cfg)
        {
            AddScaledCount(-1.0 * scale, cfg);
        }

        public double GetFeatureCount(int feature)
        {
            int consecutiveID = model.GetFeatureConsecutive(feature);
            if (consecutiveID >= consecutiveFeatureCounts.Count)
                return 0.0;
            return consecutiveFeatureCounts[consecutiveID];
        }
        public double Score { get { return featureWeightSum; } }
 
        public void GetActivedFeatures(List<int> retval)
        {
            retval.Clear();
            for (int i = 0; i < consecutiveFeatureCounts.Count; i++)
            {
                if (consecutiveFeatureCounts[i] != 0.0)
                {
                    retval.Add(model.GetFeatureID(i));
                }
            }
        }
        public void GetFeatureDifference(AccumulatedFeatureCountList afc, List<int> diff)
        {
        }
    }

    public class FeatureCountList
    {
        private double featureWeightSum;
        private List<int> features;
        private List<double> counts;
        private ExpModel model;

        public FeatureCountList(ExpModel model)
        {
            this.model = model;
            features = new List<int>();
            counts = new List<double>();
            featureWeightSum = 0.0;
        }
        public void Clear()
        {
            features.Clear();
            counts.Clear();
            featureWeightSum = 0.0;
        }
        public int Add(int featureID, double count)
        {
            features.Add(featureID);
            counts.Add(count);
            featureWeightSum += count * model.GetFeatureWeight(featureID);
            return features.Count;
        }
        public int Size { get { return features.Count; } }
        public double Score { get { return featureWeightSum; } }

        public int GetFeatureID(int idx)
        {
            return features[idx];
        }
        public double GetFeatureCount(int idx)
        {
            return counts[idx];
        }
    }
}
