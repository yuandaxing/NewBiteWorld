using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BingIntent.PCFGParser;

namespace BingIntent.ExpMod
{
    public class LabelSequence
    {
        private List<int> labels;
        private List<bool> isBoundary;
        private List<double> labelPosterior;
        private double score;
        private double prob;

        public LabelSequence(int size)
        {
            labels = new List<int>();
            isBoundary = new List<bool>();
            labelPosterior = new List<double>();
            prob = score = Ln.lnZero;
            for (int i = 0; i < size; i++)
            {
                labels.Add(-1);
                isBoundary.Add(false);
                labelPosterior.Add(Ln.lnZero);
            }
        }

        public void SetBoundary(int pos) { isBoundary[pos] = true; }
        public bool IsBoundary(int pos) { return isBoundary[pos]; }
        public List<bool> Boundaries { get { return isBoundary; } }
        public double Score
        {
            get { return score; }
            set { score = value; }
        }
        public double Prob
        {
            get { return prob; }
            set { prob = value; }
        }
        public List<int> States { get { return labels; } }

        public void SetPosterior(int i, double prob)
        {
            labelPosterior[i] = prob;
        }
        public void SetState(int i, int stateID)
        {
            labels[i] = stateID;
        }
        public void Write(StreamWriter sw, ISample sample, PCFGParser.LexiconCollection lc)
        {
            List<string> sentence = sample.GetObservationString();
            sentence.ForEach(s => sw.Write("{0} ", s));
            sw.WriteLine();
            labels.ForEach(label => sw.Write("{0} ", lc.GetWord(LexType.State, label)));
            sw.WriteLine();
        }
    }
}
