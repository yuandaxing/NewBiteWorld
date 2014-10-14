using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.CRF4SLU.UnitTest
{
    class TestModelSetting
    {
        public static void test()
        {
            ModelSetting ms = new ModelSetting(@"C:\share\work\enu_Calendar-V1\SLOT\enu_calendar.train_config.xml");
            Console.WriteLine(ms.ToString());
        }
    }
}
