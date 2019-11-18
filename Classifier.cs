using Google.Cloud.Language.V1;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Classification
{
    [Serializable()]
    public class NamesConfidence
    {
        public string Name0 { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string Name4 { get; set; }
        public double Confidence { get; set; }
    }

    [Serializable()]
    public class Classification
    {
        public object Error { get; set; }
        public NamesConfidence[] Cargo { get; set; }
    }

    public static class Classifier
    {
        public static string ClassifyPlainTextAsXml(string text, bool debug = false)
        {
            if (debug) System.Diagnostics.Debugger.Launch();
            dynamic classification = JsonConvert.DeserializeObject(ClassifyPlainText(text)); // return error and cargo_array
            var xmlOutput = new Classification();
            if (classification.Error != null)
            {
                string tmp = Convert.ToString(classification.Error.Value);
                var XDoc = new XDocument(
                    new XElement("Classification",
                        new XElement("Error", tmp == string.Empty ? " " : tmp),
                        new XElement("Cargo", "")));
                return XDoc.ToString(SaveOptions.OmitDuplicateNamespaces);
            }
            var cargoTemp = new List<NamesConfidence>();
            //xmlOutput.Cargo = new List<NamesConfidence>();
            foreach (dynamic item in classification.Cargo)
            {
                var namesConfidence = new NamesConfidence
                {
                    Confidence = item.Confidence.Value
                };
                var segments = item.Name.Value.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                namesConfidence.Name0 = segments.Length > 0 ? segments[0] : string.Empty;
                namesConfidence.Name1 = segments.Length > 1 ? segments[1] : string.Empty;
                namesConfidence.Name2 = segments.Length > 2 ? segments[2] : string.Empty;
                namesConfidence.Name3 = segments.Length > 3 ? segments[3] : string.Empty;
                namesConfidence.Name4 = segments.Length > 4 ? segments[4] : string.Empty;
                cargoTemp.Add(namesConfidence);
            }
            if (cargoTemp != null)
                xmlOutput.Cargo = cargoTemp.ToArray();
            xmlOutput.Error = String.Empty;
            //xmlOutput.Cargo.Add(namesConfidence);
            return xmlOutput.Serialize<Classification>();
        }

        public static string ClassifyHtmlAsXml(string html)
        {
            var classfication = ClassifyHtml(html);
            Classification classificationObject = JsonConvert.DeserializeObject<Classification>(classfication);
            return classificationObject.Serialize<Classification>();
        }

        public static string ClassifyPlainText(string text)
        {
            var document = new Document
            {
                Type = Document.Types.Type.PlainText,
                Content = text
            };
            string error;
            object cargo;
            try
            {
                ClassifyTextResponse response = LanguageServiceClient.Create().ClassifyText(document);
                cargo = response.Categories;
                error = null;
            }
            catch (Exception e)
            {
                error = e.Message;
                cargo = string.Empty;
            }
            return JsonConvert.SerializeObject(new JsonResponse()
            {
                Error = error,
                Cargo = cargo
            });

        }

        public static string ClassifyHtml(string html) => Classify(html);

        public static string Classify(string html)
        {
            var document = new Document
            {
                Type = Document.Types.Type.Html,
                Content = html
            };
            string error;
            object cargo;
            try
            {
                var client = LanguageServiceClient.Create();
                ClassifyTextResponse response = client.ClassifyText(document);
                cargo = response.Categories;
                error = null;
            }
            catch (Exception e)
            {
                error = e.Message;
                cargo = string.Empty;
            }
            return JsonConvert.SerializeObject(new JsonResponse()
            {
                Error = error,
                Cargo = cargo
            });
        }
    }
}

