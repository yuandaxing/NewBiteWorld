using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using BingIntent.CRF4SLU;

namespace Bite
{
    class MainEntry
    {
        public static void Train(string trainConfig)
        {
            //Console.WriteLine(Ln.Exp(x1));
            CRFModel crf = new
                CRFModel(trainConfig);
            crf.Train();
        }
        public static void Test(string testConfig, string outputFile)
        {
            CRFModel crf = new
                CRFModel(testConfig);
            crf.Test(outputFile);

        }
        public static void Main(string[] args)
        {
            Debug.Assert(args.Length != 0);
            if (args[0] == "train")
            {
                Train(args[1]);
            }
            else if (args[0] == "test")
            {
                Test(args[1], args[2]);
            }
            else
            {
                Console.WriteLine("bad parameter");
            }   
        }
    }
}
