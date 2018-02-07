namespace ServiceBase.Mvc.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class ExtensionManager
    {
        private static IEnumerable<Assembly> assemblies;
        private static IDictionary<Type, IEnumerable<Type>> types;

        public static IEnumerable<Assembly> Assemblies
        {
            get
            {
                return ExtensionManager.assemblies;
            }
        }

        public static void SetAssemblies(IEnumerable<Assembly> assemblies)
        {
            ExtensionManager.assemblies = assemblies;
            ExtensionManager.types = new Dictionary<Type, IEnumerable<Type>>();
        }

        public static Type GetImplementation<T>(bool useCaching = false)
        {
            return ExtensionManager.GetImplementation<T>(null, useCaching);
        }

        public static Type GetImplementation<T>(
            Func<Assembly, bool> predicate, bool useCaching = false)
        {
            return ExtensionManager
                .GetImplementations<T>(predicate, useCaching).FirstOrDefault();
        }

        public static IEnumerable<Type> GetImplementations<T>(
            bool useCaching = false)
        {
            return ExtensionManager.GetImplementations<T>(null, useCaching);
        }

        public static IEnumerable<Type> GetImplementations<T>(
            Func<Assembly, bool> predicate,
            bool useCaching = false)
        {
            Type type = typeof(T);

            if (useCaching && ExtensionManager.types.ContainsKey(type))
            {
                return ExtensionManager.types[type];
            }

            List<Type> implementations = new List<Type>();

            foreach (Assembly assembly in
                ExtensionManager.GetAssemblies(predicate))
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
                ExtensionManager.types.Add(type, implementations);
            }

            return implementations;
        }

        public static T GetService<T>(
            bool useCaching = false)
        {
            return ExtensionManager
                .GetService<T>(null, useCaching, new object[] { });
        }

        public static T GetService<T>(
            bool useCaching = false,
            params object[] args)
        {
            return ExtensionManager.GetService<T>(
                null, useCaching, args);
        }

        public static T GetService<T>(
            Func<Assembly, bool> predicate,
            bool useCaching = false)
        {
            return ExtensionManager
                .GetServices<T>(predicate, useCaching).FirstOrDefault();
        }

        public static T GetService<T>(
            Func<Assembly, bool> predicate,
            bool useCaching = false,
            params object[] args)
        {
            return ExtensionManager
                .GetServices<T>(predicate, useCaching, args).FirstOrDefault();
        }

        public static IEnumerable<T> GetServices<T>(
            bool useCaching = false)
        {
            return ExtensionManager
                .GetServices<T>(null, useCaching, new object[] { });
        }

        public static IEnumerable<T> GetServices<T>(
            bool useCaching = false,
            params object[] args)
        {
            return ExtensionManager
                .GetServices<T>(null, useCaching, args);
        }

        public static IEnumerable<T> GetServices<T>(
            Func<Assembly, bool> predicate,
            bool useCaching = false)
        {
            return ExtensionManager
                .GetServices<T>(predicate, useCaching, new object[] { });
        }

        public static IEnumerable<T> GetServices<T>(
            Func<Assembly, bool> predicate,
            bool useCaching = false,
            params object[] args)
        {
            List<T> instances = new List<T>();

            foreach (Type implementation in ExtensionManager
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
                return ExtensionManager.Assemblies;
            }

            return ExtensionManager.Assemblies.Where(predicate);
        }
    }
}