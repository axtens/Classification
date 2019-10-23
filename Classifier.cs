using Google.Cloud.Language.V1;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace Classification
{
    [Serializable()]
    public class Cargo
    {
        public string Name { get; set; }
        public double Confidence { get; set; }
    }

    [Serializable()]
    public class Classification
    {
        public object Error { get; set; }
        public List<Cargo> Cargo { get; set; }
    }

    public class Classifier
    {
        public static string ClassifyPlainTextAsXml(string text)
        {
            //System.Diagnostics.Debugger.Launch();
            var classification = ClassifyPlainText(text);
            var classificationObject = JsonConvert.DeserializeObject<Classification>(classification);
            return classificationObject.Serialize<Classification>();
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

