using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.PCFGParser;
using BingIntent.ExpMod;
using System.IO;

namespace BingIntent.CRF4SLU
{
    public class CRFModel
    {
        private Schema schema;
        private LexiconCollection lexiconCollection;
        private ModelSetting setting;
        private ModelFactory factory;
        private ExpModel model;
        public CRFModel(string settingFile)
        {
            this.lexiconCollection = LexiconCollection.I;
            this.setting = new ModelSetting(settingFile);
            this.schema = new Schema(setting.schemaFile, lexiconCollection);
            factory = new ModelFactory(setting, schema, lexiconCollection);
            model = new ExpModel(lexiconCollection, factory);
        }
        public void Test(string outputFile)
        {

            factory.EnableFeature();
            model.Read(setting.modelFile);
            SampleStream stream = new SampleStream(setting.sampleFile, factory,
    lexiconCollection, setting.isTesting);
            ISample sample;
            DateTime start = DateTime.Now;
            double count = 0;
            using (StreamWriter resultWriter = File.CreateText(outputFile))
            {
                while ((sample = stream.Next()) != null)
                {
                    List<LabelSequence> result = new List<LabelSequence>();
                    model.Parse(sample, true, result);
                    result[0].Write(resultWriter, sample, lexiconCollection);
                    count++;
                }
            }
            DateTime end = DateTime.Now;
            TimeSpan total = end - start;
            double average = total.TotalMilliseconds * 1.0 / count;
            Console.WriteLine(@"samples: {0}, total time {1}, average time {2}",
                count, total, average);

        }

        public void Train()
        {
            SampleStream stream = new SampleStream(setting.sampleFile, factory,
                lexiconCollection, setting.isTesting);
            factory.EnableFeature();
            factory.CreateFeatures(stream);
            model.Init(lexiconCollection.GetCount(LexType.Feature));
            model.Learn(stream, setting.trainAttribute);
            model.Save(setting.modelFile);
        }
    }
}
