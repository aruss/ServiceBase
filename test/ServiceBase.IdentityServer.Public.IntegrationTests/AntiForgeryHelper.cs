using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServiceBase.IdentityServer.UnitTests
{
    // http://www.stefanhendriks.com/2016/04/29/integration-testing-your-dot-net-core-app-with-an-in-memory-database/
    // http://www.stefanhendriks.com/2016/05/11/integration-testing-your-asp-net-core-app-dealing-with-anti-request-forgery-csrf-formdata-and-cookies/

    public static class ResponseHelper
    {
        public static string ExtractAntiForgeryToken(string responseText)
        {
            if (responseText == null)
            {
                throw new ArgumentNullException(nameof(responseText));
            }

            var match = Regex.Match(responseText, @"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>");
            return match.Success ? match.Groups[1].Captures[0].Value : null;
        }

        public static string ExtractReturnUrl(string responseText)
        {
            if (responseText == null)
            {
                throw new ArgumentNullException(nameof(responseText));
            }

            var match = Regex.Match(responseText, @"\<input type=""hidden"" id=""ReturnUrl"" name=""ReturnUrl"" value=""([^""]+)"" \/\>");

            if (match.Success)
            {
                var returnUrl = match.Groups[1].Captures[0].Value;
                returnUrl = System.Net.WebUtility.HtmlDecode(returnUrl);
                return returnUrl;
            }

            return null;
        }

        public static async Task<string> ExtractAntiForgeryToken(this HttpResponseMessage response)
        {
            var responseAsString = await response.Content.ReadAsStringAsync();
            return await Task.FromResult(ExtractAntiForgeryToken(responseAsString));
        }
    }

    public class CookiesHelper
    {
        // Inspired from:
        // https://github.com/aspnet/Mvc/blob/538cd9c19121f8d3171cbfddd5d842cbb756df3e/test/Microsoft.AspNet.Mvc.FunctionalTests/TempDataTest.cs#L201-L202

        public static IDictionary<string, string> ExtractCookiesFromResponse(HttpResponseMessage response)
        {
            var result = new Dictionary<string, string>();
            IEnumerable<string> values;
            if (response.Headers.TryGetValues("Set-Cookie", out values))
            {
                SetCookieHeaderValue.ParseList(values.ToList()).ToList().ForEach(cookie =>
                {
                    result.Add(cookie.Name, cookie.Value);
                });
            }
            return result;
        }

        public static HttpRequestMessage PutCookiesOnRequest(HttpRequestMessage request, IDictionary<string, string> cookies)
        {
            cookies.Keys.ToList().ForEach(key =>
            {
                request.Headers.Add("Cookie", new CookieHeaderValue(key, cookies[key]).ToString());
            });

            return request;
        }

        public static HttpRequestMessage CopyCookiesFromResponse(HttpRequestMessage request, HttpResponseMessage response)
        {
            return PutCookiesOnRequest(request, ExtractCookiesFromResponse(response));
        }
    }

    public static class RequestHelper
    {
        public static HttpRequestMessage CreatePostRequest(String path, Dictionary<string, string> formPostBodyData)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new FormUrlEncodedContent(ToFormPostData(formPostBodyData))
            };

            return httpRequestMessage;
        }

        public static List<KeyValuePair<string, string>> ToFormPostData(Dictionary<string, string> formPostBodyData)
        {
            var result = new List<KeyValuePair<string, string>>();
            formPostBodyData.Keys.ToList().ForEach(key =>
            {
                result.Add(new KeyValuePair<string, string>(key, formPostBodyData[key]));
            });

            return result;
        }
        
        public static HttpRequestMessage CreatePostRequest(this HttpResponseMessage response, string path, Dictionary<string, string> formPostBodyData)
        {        
            var httpRequestMessage = CreatePostRequest(path, formPostBodyData);
            return CookiesHelper.CopyCookiesFromResponse(httpRequestMessage, response);
        }

        public static HttpRequestMessage CreateGetRequest(this HttpResponseMessage response, string path)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, path); 
            return CookiesHelper.CopyCookiesFromResponse(httpRequestMessage, response);
        }
    }
}
