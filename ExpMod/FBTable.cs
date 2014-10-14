using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.ExpMod
{
    public abstract class  FBTable
    {
        protected IModelFactory factory;
        protected ISample sample;
        private FBCell[] cells;
        private int height, width;
        private int initStateIdx, finalStateIdx;

        public FBTable(ExpModel model)
        {
            this.factory = model.ModelFactory;
            int[] allowStates = factory.GetAllAllowedStates();
            height = allowStates.Length;
            for (int i = 0; i < allowStates.Length; i++)
            {
                if (allowStates[i] == factory.GetFinalState())
                    finalStateIdx = i;
                if (allowStates[i] == factory.GetInitState())
                    initStateIdx = i;
            }
        }
        private void init(ISample sample)
        {
            this.sample = sample;
            width = sample.GetSize() + 1;
            cells = new FBCell[height * width];
            for (int i = 0; i < height * width; i++)
            {
                int frame = i / height;
                int stateIdx = i % height;
                cells[i] = new FBCell(frame, stateIdx);
            }
        }

        public FBCell GetCell(int frame, int stateIdx)
        {
            return cells[frame * height + stateIdx];
        }
        public FBCell GetInitCell()
        {
            return GetCell(0, initStateIdx);
        }
        public FBCell GetFinalCell()
        {
            return GetCell(width-1, finalStateIdx);
        }
        public double GetForwardScore() { return GetFinalCell().InComingAlpha; }
        public double GetBackwardScore() { return GetInitCell().InComingBeta; }

        /// <summary>
        /// some virtual function to override in SemiMarkov 
        /// </summary>
        public abstract double Forward(bool markedStates, LabelSequence labelSequence);

        public abstract double Backward(bool markedStates, LabelSequence labelSequence);

        public abstract void ViterbiSearch(ISample sample);

        public abstract void CollectFeatureCount(AccumulatedFeatureCountList featureCounts,
            double totalLnScore, bool markedStates, LabelSequence labelSequence);


        public double AttachSample(ISample sample, bool viterbi, bool markedState, LabelSequence labelSequence)
        {
            init(sample);

            if (viterbi)
            {
                ViterbiSearch(sample);
                return GetFinalCell().BestIncomingScore;
            }
            else
            {
                double alpha = Forward(markedState, labelSequence);
                double beta = Backward(markedState, labelSequence);
                return alpha;
            }
        }
        //public void GetFramePosteriorDistribute(double totalLnScore, List<List<double>> distribute);
        public void GetTraceSeqence(NBestTrace trace, LabelSequence labelSequence)
        {
            int[] allowedStates = factory.GetAllAllowedStates();
            int preFrame = trace.CurCell.Frame;
            NBestTrace curTrace = trace.PrevTrace;
            while(curTrace != null)
            {
                if (curTrace.CurCell.Frame != preFrame)
                {
                    for (int i = curTrace.CurCell.Frame; i < preFrame; i++)
                    {
                        labelSequence.SetState(i, allowedStates[curTrace.CurCell.StateIdx]);
                    }
                    labelSequence.SetBoundary(curTrace.CurCell.Frame);
                    preFrame = curTrace.CurCell.Frame;
                }
                curTrace = curTrace.PrevTrace;
            }
        }
    }
}
