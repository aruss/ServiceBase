// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static partial class RandomExtensions
    {
        private static Random instance;

        public static Random Instance
        {
            get
            {
                return RandomExtensions.instance ??
                       (RandomExtensions.instance = new Random(
                           Guid.NewGuid().GetHashCode()));
            }
        }

        public static T Next<T>(this Random rnd, IEnumerable<T> list)
        {
            return (T)list.ElementAt(rnd.Next(0, list.Count()));
        }

        public static T Next<T>(this Random rnd, System.Type type)
        {
            ArrayList constants = new ArrayList();

            FieldInfo[] fieldInfos = type.GetFields(
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy);

            // replace by linq
            foreach (FieldInfo fi in fieldInfos)
            {
                if (fi.IsLiteral && !fi.IsInitOnly)
                {
                    constants.Add(fi);
                }
            }

            return (T)(constants[rnd.Next(0, constants.Count)] as FieldInfo)
                .GetValue(null);
        }

        public static float NextFloat(this Random rnd)
        {
            const float scale = 1.0f / 2147483648;
            return (float)(rnd.Next() & 0x7fffffff) * scale;
        }

        public static float NextFloat(
            this Random rnd,
            float maxValue)
        {
            return NextFloat(rnd) * maxValue;
        }

        public static float NextFloat(
            this Random rnd,
            float minValue,
            float maxValue)
        {
            return NextFloat(rnd) * (maxValue - minValue) + minValue;
        }

        public static double NextDouble(
            this Random rnd,
            double maxValue)
        {
            return rnd.NextDouble() * maxValue;
        }

        public static double NextDouble(
            this Random rnd,
            double minValue,
            double maxValue)
        {
            return rnd.NextDouble() * (maxValue - minValue) + minValue;
        }

        public static bool NextBool(this Random rnd)
        {
            return rnd.Next(0, 2) != 0;
        }

        public static DateTime Next(
            this Random rnd,
            DateTime min,
            DateTime max)
        {
            if (max <= min)
            {
                throw new ArgumentException("Max must be greater than min.");
            }

            long minTicks = min.Ticks;
            long maxTicks = max.Ticks;
            double rn = (Convert.ToDouble(maxTicks)
               - Convert.ToDouble(minTicks)) * rnd.NextDouble()
               + Convert.ToDouble(minTicks);

            return new DateTime(Convert.ToInt64(rn));
        }

        public static T NextEnum<T>(this Random rnd)
        {
            string[] items = Enum.GetNames(typeof(T));
            Random r = new Random((int)DateTime.Now.Ticks);
            string e = items[r.Next(0, items.Length - 1)];

            return (T)Enum.Parse(typeof(T), e, true);
        }
    }
}