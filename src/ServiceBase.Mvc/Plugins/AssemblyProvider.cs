namespace ServiceBase.Mvc.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using Microsoft.Extensions.Logging;

    public class AssemblyProvider
    {
        private readonly ILogger _logger;

        private Func<Assembly, bool> IsCandidateAssembly { get; set; }

        public AssemblyProvider(ILogger logger)
        {
            this._logger = logger; 

            this.IsCandidateAssembly = assembly =>
                !assembly.FullName.StartsWith(
                    "System.", StringComparison.OrdinalIgnoreCase) &&
                !assembly.FullName.StartsWith(
                    "Microsoft.", StringComparison.OrdinalIgnoreCase);
        }

        public IEnumerable<Assembly> GetAssemblies(string path)
        {
            List<Assembly> assemblies = new List<Assembly>();

            this.GetAssembliesFromPath(assemblies, path, "bin");

            return assemblies;
        }

        private void GetAssembliesFromPath(
            List<Assembly> assemblies,
            string path,
            string subPath)
        {
            // Get all plugins folders
            foreach (string pluginPath in Directory.GetDirectories(path))
            {
                this.GetAssembliesFromPath(assemblies,
                    Path.Combine(pluginPath, subPath));
            }
        }

        private void GetAssembliesFromPath(
            List<Assembly> assemblies,
            string path)
        {
            this._logger.LogInformation(
                   "Discovering and loading assemblies from path '{0}'",
                   path);

            foreach (string assemblyPath in
                 Directory.EnumerateFiles(path, "*.dll"))
            {
                Assembly assembly = null;

                try
                {
                    // var test = Assembly.LoadFile(assemblyPath);
                    
                    assembly = AssemblyLoadContext.Default
                        .LoadFromAssemblyPath(assemblyPath);
                    
                    if (this.IsCandidateAssembly(assembly) &&
                        !assemblies.Any(a => String.Equals(
                        a.FullName,
                        assembly.FullName,
                        StringComparison.OrdinalIgnoreCase)))
                    {
                        assemblies.Add(assembly);

                        this._logger.LogInformation(
                            "Assembly '{0}' is discovered and loaded",
                            assembly.FullName);
                    }
                }
                catch (Exception e)
                {
                    this._logger.LogWarning(
                        "Error loading assembly '{0}'",
                        assemblyPath);
                }            
            }

            // load assemblies from subdirectories
            foreach (string subpath in Directory.GetDirectories(path))
            {
                this.GetAssembliesFromPath(assemblies, subpath);
            }
        }
    }
}