namespace ServiceBase.Mvc.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class AssemblyProvider
    {
        private readonly ILogger _logger;

        public Func<Assembly, bool> IsCandidateAssembly { get; set; }
        
        public AssemblyProvider(IServiceProvider serviceProvider)
        {
            this._logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger("Extensions");

            this.IsCandidateAssembly = assembly =>
                !assembly.FullName.StartsWith(
                    "System.", StringComparison.OrdinalIgnoreCase) &&
                !assembly.FullName.StartsWith(
                    "Microsoft.", StringComparison.OrdinalIgnoreCase);
        }

        public IEnumerable<Assembly> GetAssemblies(
            string path,
            bool includingSubpaths)
        {
            List<Assembly> assemblies = new List<Assembly>();

            this.GetAssembliesFromPath(assemblies, path, includingSubpaths);

            return assemblies;
        }

        private void GetAssembliesFromPath(
            List<Assembly> assemblies,
            string path,
            bool includingSubpaths)
        {
            if (!String.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                this._logger.LogInformation(
                    "Discovering and loading assemblies from path '{0}'",
                    path);

                foreach (string extensionPath in
                    Directory.EnumerateFiles(path, "*.dll"))
                {
                    Assembly assembly = null;

                    try
                    {
                        assembly = AssemblyLoadContext.Default
                            .LoadFromAssemblyPath(extensionPath);

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
                            extensionPath);
                    }
                }

                if (includingSubpaths)
                {
                    foreach (string subpath in Directory.GetDirectories(path))
                    {
                        this.GetAssembliesFromPath(
                            assemblies, subpath, includingSubpaths);
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(path))
                {
                    this._logger.LogWarning(
                        "Discovering and loading assemblies from path skipped: path not provided",
                        path);
                }
                else
                {
                    this._logger.LogWarning(
                        "Discovering and loading assemblies from path '{0}' skipped: path not found",
                        path);
                }
            }
        }
    }
}