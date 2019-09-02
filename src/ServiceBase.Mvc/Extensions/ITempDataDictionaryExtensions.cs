// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Extensions
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Newtonsoft.Json;

    public static partial class TempDataExtensions
    {
        public static bool TryLoadData<T>(
            this ITempDataDictionary tempData,
            out T data)
        {
            data = default(T);
            string entryKey = typeof(T).Name;

            if (tempData.ContainsKey(entryKey))
            {
                string value = tempData[entryKey] as string;
                byte[] bytes = Convert.FromBase64String(value);
                value = TempDataExtensions.Decompress(bytes);
                data = JsonConvert.DeserializeObject<T>(value);
                tempData.Remove(entryKey);

                return true;
            }

            return false;
        }

        public static void StoreData<T>(
            this ITempDataDictionary tempData,
            T data)
        {
            if (data == null)
            {
                return;
            }

            string entryKey = data.GetType().Name;
            string value = JsonConvert.SerializeObject(data);
            byte[] bytes = TempDataExtensions.Compress(value);

            if (bytes != null)
            {
                value = Convert.ToBase64String(bytes);
                tempData[entryKey] = value;
            }
        }

        private static byte[] Compress(string value)
        {
            if (value == null)
            {
                return null;
            }

            byte[] data = Encoding.UTF8.GetBytes(value);

            using (MemoryStream input = new MemoryStream(data))
            using (MemoryStream output = new MemoryStream())
            {
                using (Stream cs = new DeflateStream(
                    output,
                    CompressionMode.Compress))
                {
                    input.CopyTo(cs);
                }

                return output.ToArray();
            }
        }

        private static string Decompress(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            using (MemoryStream input = new MemoryStream(data))
            using (MemoryStream output = new MemoryStream())
            {
                using (Stream cs = new DeflateStream(
                    input,
                    CompressionMode.Decompress))
                {
                    cs.CopyTo(output);
                }

                byte[] bytes = output.ToArray();
                string result = Encoding.UTF8.GetString(bytes);
                return result;
            }
        }
    }
}
