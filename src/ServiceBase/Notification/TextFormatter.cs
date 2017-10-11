namespace ServiceBase.Notification
{
    using System.Collections.Generic;
    using System.IO;

    public class TextFormatter
    {
        public string Format(
            string path,
            IDictionary<string, object> viewData)
        {
            return TokenizeText(File.ReadAllText(path), viewData);
        }

        public string TokenizeText(
            string template,
            IDictionary<string, object> viewData)
        {
            var result = template;
            foreach (var item in viewData)
            {
                result = result
                    .Replace($"{{{item.Key}}}", item.Value.ToString());
            }

            return result;
        }
    }
}