using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.CRF4SLU
{
    using BingIntent.ExpMod;
    using BingIntent.Common;
    using BingIntent.PCFGParser;

    class Slot
    {
        public string slotName, slotValue;
        public int start, end;
        public Slot(string slotName, string slotValue, int start, int end)
        {
            this.slotName = slotName;
            this.slotValue = slotValue;
            this.start = start;
            this.end = end;
        }
        public override string ToString()
        {
            return string.Join("\t", slotName, slotValue, start.ToString(), end.ToString());
        }
    }
    class SampleLabel
    {
        public string domain, sampleValue;
        public List<Slot> slots;
        public SampleLabel(string domain, string sampleValue)
        {
            this.slots = new List<Slot>();
            this.domain = domain;
            this.sampleValue = sampleValue;
        }
    }
    class Sample : ISample
    {
        private LexiconCollection lc;
        private int[] observation, state;
        private string[] observationString;
        private bool[] boundary;
        private void InitState(SampleLabel sampleLable, IModelFactory factory)
        {
            LexiconCollection lc = factory.GetLexiconCollection();
            int prePos = 0;
            for (int i = 0; prePos < observation.Length &&
                i < sampleLable.slots.Count; i++)
            {
                Slot cur = sampleLable.slots[i];
                string slotString = sampleLable.domain + factory.GetStateSeperator() + cur.slotName;
                int stateID = lc.GetLexID(LexType.State, slotString);
                ArrayUtil.Fill(state, factory.GetPreambleState(stateID), prePos, cur.start);
                ArrayUtil.Fill(state, stateID, cur.start, cur.end);
                prePos = cur.end;
            }
            ArrayUtil.Fill(state, factory.GetPostState(), prePos, state.Length);
        }
        private void InitObs(bool isTest)
        {
            for (int i = 0; i < observationString.Length; i++)
            {
                if (isTest)
                {
                    observation[i] = lc.GetLexID(LexType.Obervation, observationString[i]);
                }
                else
                {
                    observation[i] = lc.AddGetLexID(LexType.Obervation, observationString[i]);
                }
            }
        }
        public Sample(SampleLabel sampleLabel, IModelFactory factory, LexiconCollection lc, bool isTest)
        {
            this.lc = lc;
            this.observationString = sampleLabel.sampleValue.Split(new char[] { ' ' });
            this.observation = new int[observationString.Length];
            this.state = new int[observationString.Length];
            this.boundary = new bool[observationString.Length + 1];
            ArrayUtil.Fill(this.boundary, false);
            ArrayUtil.Fill(this.state, -1);
            ArrayUtil.Fill(this.observation, -1);
            this.boundary[0] = true;
            this.boundary[boundary.Length - 1] = true;
            for (int i = 0; i < sampleLabel.slots.Count; i++)
            {
                this.boundary[sampleLabel.slots[i].start] = true;
                this.boundary[sampleLabel.slots[i].end] = true;
            }
            InitState(sampleLabel, factory);
            InitObs(isTest);
        }

        public int GetSize()
        {
            return observationString.Length;
        }
        public int[] GetStates()
        {
            return state;
        }
        public int[] GetObservation()
        {
            return observation;
        }
        public List<string> GetObservationString()
        {
            return observationString.ToList();
        }
        public bool IsBoundary(int frame)
        {
            return boundary[frame];
        }
    }
}
