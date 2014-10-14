using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.CRF4SLU
{
    using BingIntent.ExpMod;
    using BingIntent.PCFGParser;
    using BingIntent.Common;

    public abstract class StateSpace : IModelFactory
    {
        private Dictionary<int, int> preambleState;
        private int[][] previousStates;
        private int[][] nextStates;
        private int[] allowedStates;
        private int[] state2Idx;
        private int finalState;
        private int initState;
        private int postState;
        private string preTag, postTag, endTag, startTag;
        public string stateSeperator;
        LexiconCollection lexiconCollection;
        ModelSetting setting;
        Schema schema;
        public StateSpace(ModelSetting modelSetting, Schema schema,
            LexiconCollection lc)
        {
            this.setting = modelSetting;
            this.schema = schema;
            this.preambleState = new Dictionary<int, int>();
            this.lexiconCollection = lc;
            preTag = "pre";
            postTag = "post";
            endTag = "end";
            startTag = "start";
            stateSeperator = ".";
            string domain = modelSetting.domain;
            string initStateString = string.Join(stateSeperator, domain, startTag);
            this.initState = lexiconCollection.AddGetLexID(LexType.State, initStateString);
            string finalStateString = string.Join(stateSeperator, domain, endTag);
            this.finalState = lexiconCollection.AddGetLexID(LexType.State, finalStateString);
            string postStateString = string.Join(stateSeperator, domain, postTag);
            this.postState = lexiconCollection.AddGetLexID(LexType.State, postStateString);
            List<string> slots = schema.slotNames;
            allowedStates = new int[3+slots.Count * 2];
            int start = 0;
            allowedStates[start++] = initState;
            int maxStateID = initState;
            foreach (string slot in slots)
            {
                string preambleSlotString = string.Join(stateSeperator, domain, slot, preTag);
                string slotString = string.Join(stateSeperator, domain, slot);
                int curPreamble = lexiconCollection.AddGetLexID(LexType.State, preambleSlotString);
                int cur = lexiconCollection.AddGetLexID(LexType.State, slotString);
                preambleState.Add(cur, curPreamble);
                allowedStates[start++] = curPreamble;
                allowedStates[start++] = cur;
                maxStateID = MathUtil.Max(maxStateID, cur, curPreamble);
            }
            allowedStates[start++] = postState;
            allowedStates[start++] = finalState;
            maxStateID = MathUtil.Max(maxStateID, postState, finalState);
            BuildIdxPreNext(maxStateID);
        }
        private bool IsTransition(int fromID, int toID)
        {
            //start
            if (fromID == initState)
            {
                return toID != finalState && toID != initState;
            }//slot
            else if (preambleState.ContainsKey(fromID))
            {
                return toID != initState;
            }//preamble
            else if (preambleState.ContainsValue(fromID))
            {
                return preambleState.ContainsKey(toID) && preambleState[toID] == fromID;
            }
            else if (fromID == this.postState)
            {
                return toID == finalState;
            }//final
            else
            {
                return false;
            }
        }
        private void BuildIdxPreNext(int maxStateIdx)
        {
            state2Idx = new int[maxStateIdx + 1];
            previousStates = new int[allowedStates.Length][];
            nextStates = new int[allowedStates.Length][];
            ArrayUtil.Fill(state2Idx, -1, 0, state2Idx.Length);
            for (int i = 0; i < allowedStates.Length; i++)
            {
                state2Idx[allowedStates[i]] = i;
            }

            for (int i = 0; i < allowedStates.Length; i++)
            {
                List<int> from = new List<int>(), to = new List<int>();
                for (int j = 0; j < allowedStates.Length; j++)
                {
                    if(IsTransition(allowedStates[i], allowedStates[j]))
                        to.Add(j);
                    if(IsTransition(allowedStates[j], allowedStates[i]))
                        from.Add(j);
                }
                previousStates[i] = from.ToArray();
                nextStates[i] = to.ToArray();
            }
        }
        public override int[] GetAllAllowedStates()
        {
            return allowedStates;
        }
        public override int[][] GetPreviousStateTable()
        {
            return previousStates;
        }
        public override int[][] GetNextStateTable()
        {
            return nextStates;
        }
        public override int GetInitState()
        {
            return initState;
        }
        public override int GetFinalState()
        {
            return finalState;
        }
        public override int GetPreambleState(int stateID)
        {
            if (preambleState.ContainsKey(stateID))
                return preambleState[stateID];
            return -1;
        }
        public override int GetPostState()
        {
            return postState;
        }
        public override int GetStateIdx(int state)
        {
            return state2Idx[state];
        }
        public override int GetNBest()
        {
            return 1;
        }
        public override LexiconCollection GetLexiconCollection()
        {
            return lexiconCollection;
        }
        public override string GetStateSeperator()
        {
            return this.stateSeperator;
        }

    }
}
