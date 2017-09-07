using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ServiceBase.Extensions
{
    public static class RandomExtensions
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
            var constants = new ArrayList();

            var fieldInfos = type.GetFields(
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.FlattenHierarchy);

            // replace by linq
            foreach (var fi in fieldInfos)
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

        public static float NextFloat(this Random rnd, float maxValue)
        {
            return NextFloat(rnd) * maxValue;
        }

        public static float NextFloat(
            this Random rnd, float minValue, float maxValue)
        {
            return NextFloat(rnd) * (maxValue - minValue) + minValue;
        }

        public static double NextDouble(this Random rnd, double maxValue)
        {
            return rnd.NextDouble() * maxValue;
        }

        public static double NextDouble(
            this Random rnd, double minValue, double maxValue)
        {
            return rnd.NextDouble() * (maxValue - minValue) + minValue;
        }

        public static bool NextBool(this Random rnd)
        {
            return rnd.Next(0, 2) != 0;
        }

        public static DateTime Next(
            this Random rnd, DateTime min, DateTime max)
        {
            if (max <= min)
            {
                throw new ArgumentException("Max must be greater than min.");
            }

            var minTicks = min.Ticks;
            var maxTicks = max.Ticks;
            var rn = (Convert.ToDouble(maxTicks)
               - Convert.ToDouble(minTicks)) * rnd.NextDouble()
               + Convert.ToDouble(minTicks);

            return new DateTime(Convert.ToInt64(rn));
        }

        public static T NextEnum<T>(this Random rnd)
        {
            var items = Enum.GetNames(typeof(T));
            var r = new Random((int)DateTime.Now.Ticks);
            var e = items[r.Next(0, items.Length - 1)];

            return (T)Enum.Parse(typeof(T), e, true);
        }
    }
}
