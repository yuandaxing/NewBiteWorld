using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BingIntent.ExpMod;
using System.Xml.Linq;

namespace BingIntent.CRF4SLU
{
    public class ModelSetting
    {
        public List<string> featureList;
        public string modelFile, sampleFile, schemaFile;
        public TrainAttribute trainAttribute;
        public bool isTesting;
        public string domain;
        public ModelSetting(string settingFile)
        {
            featureList = new List<string>();
            modelFile = sampleFile = schemaFile = null;
            trainAttribute = new TrainAttribute();
            XDocument doc = XDocument.Load(settingFile);
            var setting = doc.Descendants("settings");
            var features = setting.Elements("enableFeature");
            foreach (var feature in features)
            {
                XAttribute enabled = feature.Attribute("enabled");
                if(enabled.Value.Equals("true"))
                {
                    featureList.Add(feature.Attribute("feature").Value);
                }
            }
            var testSetting = setting.Elements("modelSettings").ElementAt(0);
            isTesting = !testSetting.Attribute("train").Value.Equals("Yes");
            var trainSetting = setting.Elements("trainingSettings").ElementAt(0);
            trainAttribute.stepSize = double.Parse(trainSetting.Attribute("stepsize").Value);
            trainAttribute.maxIter = int.Parse(trainSetting.Attribute("iterations").Value);
            var input = doc.Descendants("inputs");
            schemaFile = input.Elements("schema").ElementAt(0).Attribute("location").Value;
            sampleFile = input.Elements("Samples").ElementAt(0).Attribute("location").Value;
            modelFile = input.Elements("trainedModel").ElementAt(0).Attribute("location").Value;
            domain = input.Elements("domain").ElementAt(0).Attribute("name").Value;
        }

        public override string ToString()
        {
            string feats = string.Join(",", featureList);
            return string.Join("\r\n", feats, modelFile, sampleFile, schemaFile,
                isTesting.ToString(), domain);
        }
    }
}
