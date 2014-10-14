using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.PCFGParser;
using System.Xml.Linq;

namespace BingIntent.CRF4SLU
{
    public class Schema
    {
        public List<string> slotNames;
        public LexiconCollection lc;
        public Schema(string schemaFile, LexiconCollection lc)
        {
            slotNames = new List<string>();
            this.lc = lc;
            XDocument doc = XDocument.Load(schemaFile);
            var slotClass = doc.Descendants("class");
            foreach (var slot in slotClass.Elements("slot"))
            {
                slotNames.Add(slot.Attribute("name").Value);
            }
        }
        public void SlotAddLexicon()
        {
            foreach (string slot in slotNames)
                lc.AddGetLexID(LexType.Slot, slot);
        }

        public override string ToString()
        {
            return string.Join(",", slotNames);
        }
    }
}
