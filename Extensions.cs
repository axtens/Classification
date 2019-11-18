using System.IO;
using System.Xml.Serialization;

namespace Classification
{
    public static class Extensions
    {
        public static T Deserialize<T>(this string toDeserialize)
        {
            using (var textReader = new StringReader(toDeserialize))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(textReader);
            }
        }

        public static string Serialize<T>(this T toSerialize)
        {
            using (StringWriter textWriter = new StringWriter())
            {
                new XmlSerializer(typeof(T)).Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
}
