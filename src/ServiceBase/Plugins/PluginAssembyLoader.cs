namespace ServiceBase.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Loader;
    using ServiceBase.Extensions;

    public static class PluginAssembyLoader
    {
        public static void LoadAssemblies(
            string path,
            IEnumerable<string> whiteList = null)
        {
            Console.WriteLine($"Loading plugin assemblies from \"{path}\"");

            IEnumerable<string> pathes = whiteList == null ?
                PluginAssembyLoader.GetAssemblyPathes(path) :
                PluginAssembyLoader.GetAssemblyPathes(path, whiteList);

            Console.WriteLine($"Found {path.Count()} assemblies");

            PluginAssembyLoader.LoadAssemblies(pathes);

            AssemblyLoadContext.Default.Resolving += (loadContext, name) =>
            {
                Console.WriteLine($"Try to resolve \"{name.FullName}\"");

                return null;
            };
        }

        public static IEnumerable<string> GetAssemblyPathes(
            string path,
            IEnumerable<string> whiteList)
        {
            List<string> list = whiteList.Select(s => s).ToList();

            foreach (string pluginPath in Directory.GetDirectories(path))
            {
                string dirName = Path.GetFileName(path.RemoveTrailingSlash());

                string listItem = list.FirstOrDefault(s => s.Equals(
                    dirName,
                    StringComparison.InvariantCultureIgnoreCase));

                if (string.IsNullOrWhiteSpace(listItem))
                {
                    list.Remove(listItem);

                    foreach (string assemblyPath in
                        Directory.EnumerateFiles(pluginPath, "*.dll"))
                    {
                        yield return assemblyPath;
                    }
                }
            }
        }

        public static IEnumerable<string> GetAssemblyPathes(
            string path)
        {
            foreach (string pluginPath in Directory.GetDirectories(path))
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

            types.TopologicalSort(x => x
                .GetCustomAttributes(typeof(DependsOnPluginAttribute), true)
                .Cast<DependsOnPluginAttribute>()
                .Select(z => z.GetType())
                .ExpandInterfaces());

            foreach (Type type in types)
            {
                TService instance =
                    (TService)Activator.CreateInstance(type);

                yield return instance;
            }
        }
    }

}

namespace ServiceBase.Plugins
{
    using System;

    /// <summary>
    /// A attribute which defines a dependency onto another task.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class DependsOnPluginAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Makes the current type depend on the specified type.
        /// </summary>
        /// <param name="type">
        /// The type or interface (then it ll depend on all implementations)
        /// which the current type depends on.
        /// </param>
        public DependsOnPluginAttribute(string pluginName)
        {
            this.PluginName = pluginName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the dependency.
        /// </summary>
        public string PluginName
        {
            get;
            private set;
        }

        #endregion
    }
}
