namespace ServiceBase.Mvc.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Loader;

    public static class PluginAssembyLoader
    {
        public static void LoadAssemblies(string path)
        {
            Console.WriteLine($"Loading plugin assemblies from \"{path}\"");

            IEnumerable<string> pathes =
                PluginAssembyLoader.GetAssemblyPathes(path);

            Console.WriteLine($"Found {path.Count()} assemblies");

            PluginAssembyLoader.LoadAssemblies(pathes);

            AssemblyLoadContext.Default.Resolving += (loadContext, name) =>
            {
                Console.WriteLine($"Try to resolve \"{name.FullName}\"");

                return null;
            };
        }

        public static IEnumerable<string> GetAssemblyPathes(string path)
        {
            foreach (var pluginPath in Directory.GetDirectories(path))
            {
                foreach (string assemblyPath in
                    Directory.EnumerateFiles(pluginPath, "*.dll"))
                {
                    yield return assemblyPath;
                }
            }
        }

        public static void LoadAssemblies(IEnumerable<string> assemblyPathes)
        {
            foreach (var assemblyPath in assemblyPathes)
            {
                Console.WriteLine($"Loading assembly from \"{assemblyPath}\"");

                try
                {
                    AssemblyLoadContext.Default
                        .LoadFromAssemblyPath(assemblyPath);
                }
                catch (/*BadImageFormatException*/ Exception)
                {
                    Console.WriteLine(
                        $"Couldn't load assembly \"{assemblyPath}\"");
                }
            }
        }

        public static IEnumerable<Type> GetTypes<TService>()
        {
            Type type = typeof(TService);

            IEnumerable<Type> types =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x =>
                        type.IsAssignableFrom(x) &&
                        !x.IsInterface &&
                        !x.IsAbstract);

            return types;
        }

        public static IEnumerable<TService> GetServices<TService>()
        {
            IEnumerable<Type> types = PluginAssembyLoader.GetTypes<TService>();

            foreach (Type type in types)
            {
                TService instance =
                    (TService)Activator.CreateInstance(type);

                yield return instance;
            }
        }
    }
}
