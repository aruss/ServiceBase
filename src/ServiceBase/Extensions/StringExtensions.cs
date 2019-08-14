// Parts of the code are borrowed from Brock Allen & Dominick Baier.

namespace ServiceBase.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Encodings.Web;

    public static partial class StringExtensions
    {
        private static readonly string[] Booleans =
            new string[] { "true", "yes", "on", "1" };

        [DebuggerStepThrough]
        public static bool ToBoolean(
            this string value, bool defaultValue = false)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return Booleans.Contains(value.ToString().ToLower());
            }

            return defaultValue;
        }

        [DebuggerStepThrough]
        public static string ToSpaceSeparatedString(
            this IEnumerable<string> list)
        {
            if (list == null)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder(100);

            foreach (var element in list)
            {
                sb.Append(element + " ");
            }

            return sb.ToString().Trim();
        }

        [DebuggerStepThrough]
        public static IEnumerable<string> FromSpaceSeparatedString(
            this string input)
        {
            input = input.Trim();
            return input
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        public static List<string> ParseScopesString(this string scopes)
        {
            if (scopes.IsMissing())
            {
                return null;
            }

            scopes = scopes.Trim();
            var parsedScopes = scopes
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToList();

            if (parsedScopes.Any())
            {
                parsedScopes.Sort();
                return parsedScopes;
            }

            return null;
        }

        [DebuggerStepThrough]
        public static bool IsMissing(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        [DebuggerStepThrough]
        public static bool IsMissingOrTooLong(this string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            if (value.Length > maxLength)
            {
                return true;
            }

            return false;
        }

        [DebuggerStepThrough]
        public static bool IsPresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        [DebuggerStepThrough]
        public static string EnsureLeadingSlash(this string url)
        {
            if (!url.StartsWith("/"))
            {
                return "/" + url;
            }

            return url;
        }

        [DebuggerStepThrough]
        public static string EnsureTrailingSlash(this string url)
        {
            if (!url.EndsWith("/"))
            {
                return url + "/";
            }

            return url;
        }

        [DebuggerStepThrough]
        public static string RemoveLeadingSlash(this string url)
        {
            if (url != null && url.StartsWith("/"))
            {
                url = url.Substring(1);
            }

            return url;
        }

        [DebuggerStepThrough]
        public static string RemoveTrailingSlash(this string url)
        {
            if (url != null && url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return url;
        }

        [DebuggerStepThrough]
        public static string CleanUrlPath(this string url)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                url = "/";
            }

            if (url != "/" && url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return url;
        }

        [DebuggerStepThrough]
        public static bool IsLocalUrl(this string url)
        {
            return
                !String.IsNullOrEmpty(url) &&

                // Allows "/" or "/foo" but not "//" or "/\".
                ((url[0] == '/' && (url.Length == 1 ||
                    (url[1] != '/' && url[1] != '\\'))) ||

                // Allows "~/" or "~/foo".
                (url.Length > 1 && url[0] == '~' && url[1] == '/'));
        }

        [DebuggerStepThrough]
        public static string AddQueryString(this string url, string query)
        {
            if (!url.Contains("?"))
            {
                url += "?";
            }
            else if (!url.EndsWith("&"))
            {
                url += "&";
            }

            return url + query;
        }

        [DebuggerStepThrough]
        public static string AddQueryString(
            this string url, string name, string value)
        {
            return url.AddQueryString(name + "=" +
                UrlEncoder.Default.Encode(value));
        }

        [DebuggerStepThrough]
        public static string AddHashFragment(this string url, string query)
        {
            if (!url.Contains("#"))
            {
                url += "#";
            }

            return url + query;
        }

        /*[DebuggerStepThrough]
        public static NameValueCollection ReadQueryStringAsNameValueCollection(this string url)
        {
            if (url != null)
            {
                var idx = url.IndexOf('?');
                if (idx >= 0)
                {
                    url = url.Substring(idx + 1);
                }
                var query = QueryHelpers.ParseNullableQuery(url);
                if (query != null)
                {
                    return query.AsNameValueCollection();
                }
            }

            return new NameValueCollection();
        }*/

        [DebuggerStepThrough]
        public static bool IsSecureUrl(this string url)
        {
            return url.StartsWith("https://"); 
        }

        [DebuggerStepThrough]
        public static string GetOrigin(this string url)
        {
            if (url != null && (
                    url.StartsWith("http://") ||
                    url.StartsWith("https://")
                )
            )
            {
                var idx = url.IndexOf("//", StringComparison.Ordinal);
                if (idx > 0)
                {
                    idx = url.IndexOf("/", idx + 2, StringComparison.Ordinal);
                    if (idx >= 0)
                    {
                        url = url.Substring(0, idx);
                    }
                    return url;
                }
            }

            return null;
        }

        [DebuggerStepThrough]
        public static string ToMd5(this string input)
        {
            using (var md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(
                    Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                var sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }

        [DebuggerStepThrough]
        public static string GetFullPath(this string path, string rootPath)
        {
            if (!Path.IsPathRooted(path))
            {
                return Path.GetFullPath(
                    Path.Combine(rootPath.RemoveTrailingSlash(),
                    path.RemoveLeadingSlash())
                );
            }

            return Path.GetFullPath(path);
        }

        private static readonly string[] UglyBase64 = { "+", "/", "=" };

        [DebuggerStepThrough]
        public static string StripUglyBase64(this string s)
        {
            if (String.IsNullOrWhiteSpace(s))
            {
                return s;
            }

            foreach (var ugly in UglyBase64)
            {
                s = s.Replace(ugly, String.Empty);
            }

            return s;
        }
    }
}