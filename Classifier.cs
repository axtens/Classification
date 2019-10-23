using Google.Cloud.Language.V1;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace Classification
{
    [Serializable()]
    public class NameConfidence
    {
        public string Name { get; set; }
        public double Confidence { get; set; }
    }

    [Serializable()]
    public class RootObject
    {
        public List<NameConfidence> NameConfidences { get; set; }
    }

    public class Classifier
    {
        public static string ClassifyPlainTextAsXml(string text)
        {
            var classfication = ClassifyPlainText(text);
            RootObject classificationObject = JsonConvert.DeserializeObject<RootObject>(classfication);
            return classificationObject.Serialize<RootObject>();
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

