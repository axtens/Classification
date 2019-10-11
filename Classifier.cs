using Google.Cloud.Language.V1;

using Newtonsoft.Json;

using System;

namespace Classification
{
    public class Classifier
    {
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

