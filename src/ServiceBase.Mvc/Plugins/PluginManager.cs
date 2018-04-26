namespace ServiceBase.Mvc.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class PluginManager
    {
        private static IEnumerable<Assembly> assemblies;
        private static IDictionary<Type, IEnumerable<Type>> types;

        public static IEnumerable<Assembly> Assemblies
        {
            get
            {
                return PluginManager.assemblies;
            }
        }

        public static void SetAssemblies(IEnumerable<Assembly> assemblies)
        {
            PluginManager.assemblies = assemblies;
            PluginManager.types = new Dictionary<Type, IEnumerable<Type>>();
        }

        public static Type GetImplementation<T>(bool useCaching = false)
        {
            return PluginManager.GetImplementation<T>(null, useCaching);
        }

        public static Type GetImplementation<T>(
            Func<Assembly, bool> predicate, bool useCaching = false)
        {
            return PluginManager
                .GetImplementations<T>(predicate, useCaching).FirstOrDefault();
        }

        public static IEnumerable<Type> GetImplementations<T>(
            bool useCaching = false)
        {
            return PluginManager.GetImplementations<T>(null, useCaching);
        }

        public static IEnumerable<Type> GetImplementations<T>(
            Func<Assembly, bool> predicate,
            bool useCaching = false)
        {
            Type type = typeof(T);

            if (useCaching && PluginManager.types.ContainsKey(type))
            {
                return PluginManager.types[type];
            }

            List<Type> implementations = new List<Type>();

            foreach (Assembly assembly in
                PluginManager.GetAssemblies(predicate))
            {
                foreach (Type exportedType in assembly.GetExportedTypes())
                {
                    if (type.GetTypeInfo().IsAssignableFrom(exportedType) &&
                        exportedType.GetTypeInfo().IsClass)
                    {
                        implementations.Add(exportedType);
                    }
                }
            }

            if (useCaching)
            {
                PluginManager.types.Add(type, implementations);
            }

            return implementations;
        }

        public static T GetService<T>(
            bool useCaching = false)
        {
            return PluginManager
                .GetService<T>(null, useCaching, new object[] { });
        }

        public static T GetService<T>(
            bool useCaching = false,
            params object[] args)
        {
            return PluginManager.GetService<T>(
                null, useCaching, args);
        }

        public static T GetService<T>(
            Func<Assembly, bool> predicate,
            bool useCaching = false)
        {
            return PluginManager
                .GetServices<T>(predicate, useCaching).FirstOrDefault();
        }

        public static T GetService<T>(
            Func<Assembly, bool> predicate,
            bool useCaching = false,
            params object[] args)
        {
            return PluginManager
                .GetServices<T>(predicate, useCaching, args).FirstOrDefault();
        }

        public static IEnumerable<T> GetServices<T>(
            bool useCaching = false)
        {
            return PluginManager
                .GetServices<T>(null, useCaching, new object[] { });
        }

        public static IEnumerable<T> GetServices<T>(
            bool useCaching = false,
            params object[] args)
        {
            return PluginManager
                .GetServices<T>(null, useCaching, args);
        }

        public static IEnumerable<T> GetServices<T>(
            Func<Assembly, bool> predicate,
            bool useCaching = false)
        {
            return PluginManager
                .GetServices<T>(predicate, useCaching, new object[] { });
        }

        public static IEnumerable<T> GetServices<T>(
            Func<Assembly, bool> predicate,
            bool useCaching = false,
            params object[] args)
        {
            List<T> instances = new List<T>();

            foreach (Type implementation in PluginManager
                .GetImplementations<T>(predicate, useCaching))
            {
                if (!implementation.GetTypeInfo().IsAbstract)
                {
                    T instance = (T)Activator
                        .CreateInstance(implementation, args);

                    instances.Add(instance);
                }
            }

            return instances;
        }

        private static IEnumerable<Assembly> GetAssemblies(
            Func<Assembly, bool> predicate)
        {
            if (predicate == null)
            {
                return PluginManager.Assemblies;
            }

            return PluginManager.Assemblies.Where(predicate);
        }
    }
}