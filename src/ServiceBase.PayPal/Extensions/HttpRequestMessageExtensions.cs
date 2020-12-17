namespace ServiceBase.PayPal
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    using HeaderKV = System.Collections.Generic.KeyValuePair<
        string,
        System.Collections.Generic.IEnumerable<string>>;

    using PropKV = System.Collections.Generic.KeyValuePair<string, object>;

    public static class HttpRequestMessageExtensions
    {
        public static HttpRequestMessage Clone(
            this HttpRequestMessage request)
        {
            HttpRequestMessage clone = new HttpRequestMessage(
                request.Method,
                request.RequestUri)
            {
                Content = request.Content,
                Version = request.Version
            };

            foreach (PropKV prop in request.Properties)
            {
                clone.Properties.Add(prop);
            }

            foreach (HeaderKV header in request.Headers)
            {
                clone.Headers
                    .TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }

        public static HttpRequestMessage AddBearerAuthorizationHeader(
            this HttpRequestMessage message,
            string token)
        {
            message.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return message;
        }

        public static HttpRequestMessage AddBasicAuthorizationHeader(
            this HttpRequestMessage message,
            string username,
            string password)
        {
            message.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{username}:{password}")
                )
            );

            return message;
        }

        public static void AddJsonBody<TBody>(
            this HttpRequestMessage message,
            TBody body,
            JsonSerializerSettings jsonSerializerSettings)
        {
            message.Content = new StringContent(
                JsonConvert.SerializeObject(body, jsonSerializerSettings),
                Encoding.UTF8,
                "application/json"
            );
        }
    }

    public static class HttpResponseMessageExtensions
    {
        public static async Task<TResult> TryParseAsync<TResult>(
           this HttpResponseMessage message,
           System.Net.HttpStatusCode statusCode)
           where TResult : class, new()
        {
            if (message.StatusCode == statusCode)
            {
                try
                {
                    return await message.Content.ReadAsAsync<TResult>();
                }
                catch (Exception ex)
                {
                    // TODO: make terror here 
                    throw ex;
                }

            }

            return default(TResult);
        }

        public static async Task<T> ReadAsAsync<T>(this HttpContent content) =>
            JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync());
    }
}
