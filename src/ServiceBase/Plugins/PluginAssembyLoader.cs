﻿namespace ServiceBase.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Extensions;

    public static class PluginAssembyLoader
    {
        public static List<Assembly> Assemblies { get; }
            = new List<Assembly>();

        private static ILogger _logger;

        /// <summary>
        /// Get all the assemblies from white listed plugins
        /// including all subdirectories into current app domain.
        /// </summary>
        /// <param name="basePath">Base directory path for plugins.</param>
        /// <param name="whiteList">List of plugin names that should be loded,
        /// all other plugins will be ignored.</param>
        public static void LoadAssemblies(
            string basePath,
            ILoggerFactory loggerFactory,
            IEnumerable<string> whiteList = null)
        {
            PluginAssembyLoader._logger =
                loggerFactory.CreateLogger(typeof(PluginAssembyLoader));

            if (whiteList == null || whiteList.Count() == 0)
            {
                PluginAssembyLoader._logger.LogInformation(
                    $"Loading plugin assemblies from \"{basePath}\"");
            }
            else
            {
                PluginAssembyLoader._logger.LogInformation(
                    "Loading white listed plugin assemblies from \"{0}\"\n\t- {1}",
                    basePath,
                    String.Join("\n\t- ", whiteList));
            }

            // Get all the pathes recursevly respecting whitelist
            string[] assemblyPathes = PluginAssembyLoader
                .GetAssemblyPathesForAllPlugins(basePath, whiteList)
                .ToArray();

            PluginAssembyLoader._logger
                .LogDebug($"Found {assemblyPathes.Count()} plugin assemblies");

            // Load all the assemblies
            PluginAssembyLoader.LoadAssemblies(assemblyPathes);

            PluginAssembyLoader._logger
               .LogDebug($"Loading plugin assemblies completed");

            AssemblyLoadContext.Default.Resolving += (loadContext, name) =>
            {
                PluginAssembyLoader._logger.LogDebug(
                    $"Try to resolve \"{name.FullName}\"");

                return null;
            };
        }

        private static IEnumerable<string> GetAssemblyPathesForAllPlugins(
            string basePath,
            IEnumerable<string> whiteList = null)
        {
            // Return all the plugin folder names if white list is empty
            if (whiteList == null || whiteList.Count() == 0)
            {
                foreach (string pluginPath in
                    Directory.GetDirectories(basePath))
                {
                    foreach (string assemblyPath in PluginAssembyLoader.
                        GetAssemblyPathesForPlugin(pluginPath))
                    {
                        yield return assemblyPath;
                    }
                }

                yield break;
            }

            // If white list is presend then return the names of plugin folders
            // while respecting the white list         
            List<string> list = whiteList.Select(s => s).ToList();

            foreach (string pluginPath in Directory.GetDirectories(basePath))
            {
                string dirName = Path.GetFileName(
                    pluginPath.RemoveTrailingSlash());

                string listItem = list.FirstOrDefault(s => s.Equals(
                    dirName,
                    StringComparison.InvariantCultureIgnoreCase));

                if (!String.IsNullOrWhiteSpace(listItem))
                {
                    list.Remove(listItem);

                    foreach (string assemblyPath in PluginAssembyLoader
                        .GetAssemblyPathesForPlugin(pluginPath))
                    {
                        yield return assemblyPath;
                    }
                }
            }
        }

        /// <summary>
        /// Get all the assembly (*.dll) pathes from the plugin directory
        /// including all subdirectories.
        /// </summary>
        /// <param name="pluginPath">Base path of the plugin directory.</param>
        /// <returns>A list of assembly pathes.</returns>
        private static IEnumerable<string> GetAssemblyPathesForPlugin(
            string pluginPath)
        {
            foreach (string assemblyPath in
                  Directory.EnumerateFiles(pluginPath, "*.dll"))
            {
                yield return assemblyPath;
            }

            foreach (string subDirPath in
                Directory.GetDirectories(pluginPath))
            {
                foreach (var subAssemblyPath in PluginAssembyLoader
                    .GetAssemblyPathesForPlugin(subDirPath))
                {
                    yield return subAssemblyPath;
                }
            }
        }

        /// <summary>
        /// Loads assemblies into current app domain.
        /// </summary>
        /// <param name="assemblyPathes">
        /// List of assembly pathes to load.
        /// </param>
        private static void LoadAssemblies(IEnumerable<string> assemblyPathes)
        {
            foreach (var assemblyPath in assemblyPathes)
            {
                try
                {
                    PluginAssembyLoader._logger.LogDebug(
                        $"Try loading assembly \"{assemblyPath}\"");

                    Assemblies.Add(AssemblyLoadContext.Default
                        .LoadFromAssemblyPath(assemblyPath));
                }
                catch (/*BadImageFormatException*/ Exception)
                {
                    PluginAssembyLoader._logger.LogDebug(
                        $"Couldn't load assembly \"{assemblyPath}\"");
                }
            }
        }

        /// <summary>
        /// Finds implementation types for TService.
        /// </summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <returns>A list of implementation types.</returns>
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

        /// <summary>
        /// Get instances of requested services topologicaly sorted by
        /// <see cref="DependsOnPluginAttribute"/>.
        /// </summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <returns>A list of service instances.</returns>
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