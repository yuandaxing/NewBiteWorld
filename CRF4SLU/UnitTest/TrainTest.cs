using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.ExpMod;
using System.Diagnostics;

namespace BingIntent.CRF4SLU.UnitTest
{
    class TrainTest
    {
        public static void Train()
        {
            //Console.WriteLine(Ln.Exp(x1));
            CRFModel crf = new
                CRFModel(@"C:\Users\enu_calendar.TRAIN.config");
            crf.Train();
        }
        public static void Test()
        {
            CRFModel crf = new
                CRFModel(@"E:\project\enu_Calendar-V1\final\enu_calendar.train_config.xml");
            crf.Test(@"E:\project\enu_Calendar-V1\result.txt");
   
        }
    }
}
