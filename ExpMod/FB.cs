using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.ExpMod
{

    public class NBestTrace
    {
        private FBCell curCell;
        private double lnScore;
        private NBestTrace prevTrace;

        public NBestTrace(NBestTrace prevTrace, FBCell curCell, double score)
        {
            this.curCell = curCell;
            this.lnScore = score;
            this.prevTrace = prevTrace;
        }
        public FBCell CurCell
        {
            get { return curCell; }
        }
        public NBestTrace PrevTrace
        {
            get { return prevTrace; }
        }
        public double Score
        {
            get {return lnScore; }
        }
    }
    public class NBestTraces
    {
        private List<NBestTrace> traces;
        private int size;
        public NBestTraces(int n)
        {
            this.size = n;
            this.traces = new List<NBestTrace>();
        }
        public List<NBestTrace> Traces{
            get { return traces; }
        }
        public void AddTrace(NBestTrace nBestTrace)
        {
            int i;
            for(i = 0; i < traces.Count; i++)
            {
                if (nBestTrace.Score >= traces[i].Score)
                    break;
            }
            traces.Insert(i, nBestTrace);
            if (traces.Count > size)
                traces.RemoveAt(traces.Count - 1);
        }      
    }
    class ForwardBackWard
    {
        private FBTable fbTable;
        private double alpha, beta;
        private ExpModel expModel;
        private ISample sample;

        public ForwardBackWard(ExpModel expModel)
        {
            this.expModel = expModel;
            fbTable = new SemiMarkov(expModel);
            alpha = beta = Ln.lnZero;
        }
        public double AttachSample(ISample sample, bool viterbi, bool markedState)
        {
            return AttachSample(sample, viterbi, markedState, null);
        }
        public double AttachSample(ISample sample, bool viterbi, bool markedState, LabelSequence lableSequence)
        {
            this.sample = sample;
            alpha = fbTable.AttachSample(sample, viterbi, markedState, lableSequence);
            return alpha;
        }
        public void CollectFeatureCount(AccumulatedFeatureCountList featureCounts,
            bool markedState, LabelSequence labelSequence)
        {
            fbTable.CollectFeatureCount(featureCounts, fbTable.GetForwardScore(), markedState, labelSequence);
        }
        public void GetViterbiPath(List<LabelSequence> paths)
        {
            NBestTraces traces = fbTable.GetFinalCell().InComingTraces;
            for (int i = 0; i < traces.Traces.Count; i++)
            {
                LabelSequence labelSequence = new LabelSequence(sample.GetSize());
                fbTable.GetTraceSeqence(traces.Traces[i], labelSequence);
                paths.Add(labelSequence); 
            }
        }
    }
}
