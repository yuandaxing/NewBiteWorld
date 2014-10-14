using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.ExpMod
{
    public class FBCell
    {
        private int frame;
        private int stateIdx;
        private double alpha, inComingAlpha, beta, inComingBeta;
        private NBestTraces inComingTrace, outGoingTrace;
        public FBCell(int frame, int stateIdx)
        {
            alpha = inComingAlpha = beta = inComingBeta = Ln.lnZero;
            this.frame = frame;
            this.stateIdx = stateIdx;
            inComingTrace = outGoingTrace = null;
        }

        public double BestOutgoingScore
        {
            get
            {
                if (outGoingTrace == null || outGoingTrace.Traces.Count == 0)
                    return Ln.lnZero;
                return outGoingTrace.Traces[0].Score;
            }

        }
        public double BestIncomingScore
        {
            get
            {
                if (inComingTrace == null || inComingTrace.Traces.Count == 0)
                    return Ln.lnZero;
                return inComingTrace.Traces[0].Score;
            }
        }
        public void AddNBestTrace(FBCell currentCell, FBCell fromCell, double tranScore,
            int NBest)
        {
            //currentCell is init Cell
            if (fromCell == null)
            {
                outGoingTrace = new NBestTraces(NBest);
                outGoingTrace.AddTrace(new NBestTrace(null, currentCell, 0.0));
            }
            else if (fromCell.frame == this.frame)
            {
                if (fromCell.outGoingTrace != null)
                {
                    inComingTrace = (inComingTrace == null ? new NBestTraces(NBest) : inComingTrace);
                    for (int i = 0; i < fromCell.outGoingTrace.Traces.Count; i++)
                    {
                        NBestTrace preTrace = fromCell.outGoingTrace.Traces[i];
                        inComingTrace.AddTrace(new NBestTrace(preTrace, currentCell,
                            Ln.Mult(preTrace.Score, tranScore)));
                    }
                }
            }
            else
            {
                if (fromCell.inComingTrace != null)
                {
                    outGoingTrace = (outGoingTrace == null ? new NBestTraces(NBest) : outGoingTrace);
                    NBestTraces tr = fromCell.inComingTrace;
                    for (int i = 0; i < tr.Traces.Count; i++)
                    {
                        NBestTrace preTrace = tr.Traces[i];
                        outGoingTrace.AddTrace(new NBestTrace(preTrace, currentCell,
                            Ln.Mult(preTrace.Score, tranScore)));
                    }
                }
            }
        }
        public int StateIdx
        {
            get { return stateIdx; }
        }

        public double Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }
        public double InComingAlpha
        {
            get { return inComingAlpha; }
            set { inComingAlpha = value; }
        }
        public double Beta
        {
            get { return beta; }
            set { beta = value; }
        }
        public double InComingBeta
        {
            get { return inComingBeta; }
            set { inComingBeta = value; }
        }
        public int Frame
        {
            get { return frame; }
        }
        public NBestTraces InComingTraces
        {
            get { return inComingTrace; }
        }
        public NBestTraces OutGoingTraces
        {
            get { return outGoingTrace; }
        }

    }
}
