using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.PCFGParser;

namespace BingIntent.CRF4SLU.UnitTest
{
    public class TestSchema
    {
        public static void test()
        {
            Schema s = new Schema(@"C:\share\work\enu_Calendar-V1\final\enu_calendar.slots_schema.xml",
                LexiconCollection.I);
            Console.WriteLine(s.ToString());
        }
    }
}
