using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BingIntent.PCFGParser;
using BingIntent.Common;

namespace BingIntent.ExpMod
{
    public class ExpModel
    {
        private Feature2ConsecutiveSpace feature2Consecutive;
        private LexiconCollection lc;
        private IModelFactory modelFactory;

        //train process, all the feature weight
        private AccumulatedFeatureCountList featureCountGiveObservation;
        private AccumulatedFeatureCountList featureCountGiveObservationAndLable;

        //model feature weight
        private double[] featureWeight;
        private double[] priorMean;
       // private bool modelLoad;
        public ExpModel(PCFGParser.LexiconCollection lc, IModelFactory modelFactory)
        {
            this.lc = lc;
            this.modelFactory = modelFactory;
            //modelLoad = false;
        }

        private void Smooth()
        {
        }
        public void Init(int featureSetSize)
        {
          //  modelLoad = false;
            featureWeight = new double[featureSetSize];
            ArrayUtil.Fill(featureWeight, 0.0);
            priorMean = new double[featureSetSize];
            feature2Consecutive = new Feature2ConsecutiveSpace(featureSetSize);
            featureCountGiveObservation = new AccumulatedFeatureCountList(this);
            featureCountGiveObservationAndLable = new AccumulatedFeatureCountList(this);
        }
        public double GetFeatureWeight(int featureID) { return featureWeight[featureID]; }
        public int GetFeatureConsecutive(int featureID) { return feature2Consecutive.GetConsecutiveID(featureID); }
        public int ConsecutiveSize { get { return feature2Consecutive.Count; } }
        public int GetFeatureID(int consecutiveID)
        {
            return feature2Consecutive.GetFeatureID(consecutiveID);
        }
        public void Learn(ISampleStream ss, TrainAttribute ta)
        {
            for (int i = 0; i < ta.maxIter; i++)
            {
                DateTime begin = DateTime.Now;
                double posterior = CRFLearnIter(ss, ta);
                DateTime end = DateTime.Now;
                Console.WriteLine("iteration {0}, time cost {1}", i, end - begin);
            }
        }
        public void VectorAddInPlace(double[] vector, double scale, AccumulatedFeatureCountList featureCountList)
        {
            List<int> activeFeature = new List<int>();
            featureCountList.GetActivedFeatures(activeFeature);
            for (int i = 0; i < activeFeature.Count; i++)
            {
                vector[activeFeature[i]] += featureCountList.GetFeatureCount(activeFeature[i]) * scale;
            }
        }
        private double CRFLearnIter(ISampleStream ss, TrainAttribute ta)
        {
            ISample sample = null;
            int count = 0;
            double scoreSum = 0.0;
            ss.Randomize();
            ss.Rewind();
            while ((sample = ss.Next()) != null)
            {
                count++;
                if (count % 25 == 0)
                {
                    Console.WriteLine("process 25 samples\n");
                    if (count != 0)
                        Console.WriteLine("average averagePosterior {0}", scoreSum / count);
                }
                ForwardBackWard forwardBackward = new ForwardBackWard(this);
                double logLabelPath = forwardBackward.AttachSample(sample, false, true);
                featureCountGiveObservationAndLable.Clear();
                forwardBackward.CollectFeatureCount(featureCountGiveObservationAndLable, true, null);

                featureCountGiveObservation.Clear();
                double logAllPath = forwardBackward.AttachSample(sample, false, false);
                forwardBackward.CollectFeatureCount(featureCountGiveObservation, false, null);

                featureCountGiveObservationAndLable.Subtract(featureCountGiveObservation);
                scoreSum += logLabelPath - logAllPath;
                VectorAddInPlace(featureWeight, ta.stepSize, featureCountGiveObservationAndLable);
            }
            return scoreSum / ss.GetSize() ;
        }

        public void Save(string modelFile)
        {
            using (StreamWriter sw = File.CreateText(modelFile))
            {
                for (int i = 0; i < featureWeight.Length; i++)
                {
                    if (featureWeight[i] != 0.0)
                    {
                        sw.WriteLine("{0} {1}", lc.GetWord(LexType.Feature, i), featureWeight[i]);
                    }
                }
            }
        }
        public void Read(string modelFile)
        {
            List<string> modelLines = File.ReadAllLines(modelFile).ToList();
            ModelFactory.LoadModel(modelLines);
            Init(lc.GetCount(LexType.Feature));
            foreach (string line in modelLines)
            {
                string[] tk = line.Split(new char[] { ' ' });
                int featureID = lc.GetLexID(LexType.Feature, tk[0]);
                double weight = double.Parse(tk[1]);
                featureWeight[featureID] = weight;
            }
        }
        public void Parse(ISample sample, bool getProb, List<LabelSequence> viterbi)
        {
            ForwardBackWard forwardBackward = new ForwardBackWard(this);
            forwardBackward.AttachSample(sample, true, false);
            forwardBackward.GetViterbiPath(viterbi);
        }
        public void WriteModelFile(string filename)
        {
            using(StreamWriter modelWriter = new StreamWriter(filename, false, Encoding.UTF8))
            {
                for (int i = 0; i < featureWeight.Length; i++)
                {
                    if (featureWeight[i] == 0.0)
                        continue;
                    string featureName = lc.GetWord(LexType.Feature, i);
                    modelWriter.WriteLine("{0} {1}", featureName, featureWeight[i]);
                }
            }
        }
        public void ReadModelFile(List<string> modelLines)
        {
            Init(lc.GetCount(LexType.Feature));
            for (int i = 0; i < modelLines.Count; i++)
            {
                string[] token = modelLines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int featureID = lc.GetLexID(LexType.Feature, token[0]);
                featureWeight[featureID] = double.Parse(token[1]);
            }
        }
        public void Reset()
        {
        }

        public IModelFactory ModelFactory
        {
            get { return modelFactory; }
        }
        
    }
}
