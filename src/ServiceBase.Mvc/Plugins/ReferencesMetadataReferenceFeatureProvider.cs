// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection.PortableExecutable;
    using Microsoft.AspNetCore.Mvc.ApplicationParts;
    using Microsoft.AspNetCore.Mvc.Razor.Compilation;
    using Microsoft.CodeAnalysis;
    using Microsoft.Extensions.DependencyModel;

    // Found at https://github.com/dotnet/core-setup/issues/2981#issuecomment-322572374
    public class ReferencesMetadataReferenceFeatureProvider :
        IApplicationFeatureProvider<MetadataReferenceFeature>
    {
        public void PopulateFeature(
            IEnumerable<ApplicationPart> parts,
            MetadataReferenceFeature feature)
        {
            HashSet<string> libraryPaths =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var assemblyPart in parts.OfType<AssemblyPart>())
            {
                DependencyContext dependencyContext = DependencyContext
                    .Load(assemblyPart.Assembly);

                if (dependencyContext != null)
                {
                    foreach (CompilationLibrary library in
                        dependencyContext.CompileLibraries)
                    {
                        if ("reference".Equals(library.Type,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            foreach (var libraryAssembly in library.Assemblies)
                            {
                                libraryPaths.Add(
                                    Path.Combine(
                                        AppDomain.CurrentDomain.BaseDirectory,
                                        libraryAssembly
                                    )
                                );
                            }
                        }
                        else
                        {
                            // TODO: optimize 
                            try
                            {
                                foreach (string path in
                                    library.ResolveReferencePaths())
                                {
                                    libraryPaths.Add(path);
                                }
                            }
                            catch (Exception)
                            {
                                libraryPaths.Add(assemblyPart.Assembly.Location);
                            }
                        }
                    }
                }
                else
                {
                    libraryPaths.Add(assemblyPart.Assembly.Location);
                }
            }

            foreach (var path in libraryPaths)
            {
                feature.MetadataReferences.Add(CreateMetadataReference(path));
            }
        }

        private static MetadataReference CreateMetadataReference(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                ModuleMetadata moduleMetadata = ModuleMetadata
                    .CreateFromStream(stream, PEStreamOptions.PrefetchMetadata);

                AssemblyMetadata assemblyMetadata =
                    AssemblyMetadata.Create(moduleMetadata);

                return assemblyMetadata.GetReference(filePath: path);
            }
        }
    }
}
