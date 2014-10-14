using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.ExpMod
{
    public struct TrainAttribute
    {
        public int maxIter;
        public double stepSize;
        public double gaussianVariance; //never use
        public TrainAttribute(int unused)
        {
            maxIter = 20;
            stepSize = 0.015;
            gaussianVariance = 1.0;
        }
        public override string ToString()
        {
            return string.Join("\t", maxIter.ToString(), stepSize.ToString(),
                gaussianVariance.ToString());
        }
    }
    public abstract class IModelFactory
    {
        public abstract int[] GetAllAllowedStates();
        public abstract int[][] GetPreviousStateTable();
        public abstract int[][] GetNextStateTable();
        public abstract int RetrieveFeatureList(FeatureCountList features, ISample sample, int fromFrame, int toFrame,
               int fromState, int toState);
        public abstract int GetInitState();
        public abstract int GetFinalState();
        public abstract int GetPreambleState(int stateID);
        public abstract int GetPostState();
        public abstract int GetStateIdx(int state);
        public abstract PCFGParser.LexiconCollection GetLexiconCollection();
        public abstract int GetNBest();
        public abstract string GetStateSeperator();
        public abstract void LoadModel(List<string> modelLines);
    }
}
