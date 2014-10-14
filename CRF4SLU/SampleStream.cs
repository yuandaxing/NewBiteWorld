using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.ExpMod;
using BingIntent.Common;
using BingIntent.PCFGParser;
using System.Xml.Linq;

namespace BingIntent.CRF4SLU
{
    class ParseFile
    {
        public static Sample[] Parse(string xmlFileName, IModelFactory factory,
            LexiconCollection lc, bool isTest)
        {
            List<Sample> samples = new List<Sample>();
            XDocument doc = XDocument.Load(xmlFileName);
            var Querys = doc.Descendants("sml").Elements();
            foreach (var Query in Querys)
            {
                XAttribute sentence = Query.LastAttribute;
                var slots = Query.Elements();
                SampleLabel sampleLabel = new SampleLabel(Query.Name.ToString(), sentence.Value);
                foreach (var slot in slots)
                {
                    string slotName = slot.Name.ToString();
                    string slotValue = slot.Value;
                    int end = int.Parse(slot.Attribute("end").Value);
                    int start = int.Parse(slot.Attribute("start").Value);
                    sampleLabel.slots.Add(new Slot(slotName, slotValue, start, end));
                }
                samples.Add(new Sample(sampleLabel, factory, lc, isTest));
            }
            return samples.ToArray();
        }
    }
    public class SampleStream : ISampleStream
    {
        private Sample[] samples;
        private int[] sampleIdxs;
        private readonly int seed = 1000;
        private int cur;
        public void Randomize()
        {
            Random rand = new Random(seed);
            for (int i = sampleIdxs.Length; i > 0; i--)
            {
                int j = rand.Next(i);
                CommonUtil.Swap(ref sampleIdxs[j], ref sampleIdxs[i-1]);
            }
        }
        public ISample Next()
        {
            if (cur >= sampleIdxs.Length)
                return null;
            return samples[sampleIdxs[cur++]];
        }
        public void Rewind()
        {
            cur = 0;
        }
        public int GetSize()
        {
            return samples.Length;
        }
        public SampleStream(string xmlFileName, IModelFactory factory,
            LexiconCollection lc, bool isTest)
        {
            samples = ParseFile.Parse(xmlFileName, factory, lc, isTest);
            cur = 0;
            sampleIdxs = new int[samples.Length];
            for (int i = 0; i < samples.Length; i++)
            {
                sampleIdxs[i] = i;
            }
        }
    }
}
