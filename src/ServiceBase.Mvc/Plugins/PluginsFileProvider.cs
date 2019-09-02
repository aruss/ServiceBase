// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.FileProviders.Physical;
    using Microsoft.Extensions.Primitives;
    using ServiceBase.Extensions;

    // https://github.com/aspnet/FileSystem/blob/e469b8f244e38c346ab6a64f363df4d638cf5cee/src/FS.Physical/PhysicalFileProvider.cs
    // https://raw.githubusercontent.com/aspnet/FileSystem/e469b8f244e38c346ab6a64f363df4d638cf5cee/src/FS.Physical/Internal/PathUtils.cs

    /// <summary>
    /// This file provider uses following pattern /{BasePath}/{PluginName}/Public/*
    /// to provide static files. 
    ///
    /// For example href="/FooTheme/img/foo.png" will request /Plugins/FooTheme/Public/img/foo.png 
    /// </summary>
    public class PluginsFileProvider : IFileProvider
    {
        private readonly string _basePath;


        /// <summary>
        /// Initializes a new instance of the <see cref="PluginsFileProvider"/>
        /// class.
        /// </summary>
        /// <param name="basePath">Base path of all plugins.</param>
        public PluginsFileProvider(string basePath)
        {
            this._basePath = basePath;
        }
   
        /// <summary>
        /// Enumerate a directory at the given path, if any.
        /// </summary>
        /// <param name="subpath">Relative path that identifies the directory.
        /// d</param>
        /// <returns>Returns the contents of the directory.</returns>
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Locate a file at the given path.
        /// </summary>
        /// <param name="subpath">Relative path that identifies the file.</param>
        /// <returns>The file information. Caller must check Exists property.</returns>
        public IFileInfo GetFileInfo(string subpath)
        {
            if (string.IsNullOrEmpty(subpath) ||
                PathUtils.HasInvalidPathChars(subpath))
            {
                return new NotFoundFileInfo(subpath);
            }

            subpath = subpath.RemoveLeadingSlash();

            // Absolute paths not permitted.
            if (Path.IsPathRooted(subpath))
            {
                return new NotFoundFileInfo(subpath);
            }

            string fullPath = this.GetFullPath(subpath);

            if (string.IsNullOrWhiteSpace(fullPath))
            {
                return new NotFoundFileInfo(subpath);
            }

            return new PhysicalFileInfo(new FileInfo(fullPath));
        }

        /// <summary>
        /// Gets a file path relative to content root.
        /// </summary>
        /// <param name="path">Request path</param>
        /// <returns>A file path relative to content root.</returns>
        private string GetFullPath(string path)
        {
            List<string> chunks = path.Split("/").ToList();
            chunks.Insert(1, "wwwroot");
            path = Path.Combine(chunks.ToArray());

            if (PathUtils.PathNavigatesAboveRoot(path))
            {
                return null;
            }

            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(Path.Combine(
                    this._basePath, path));
            }
            catch
            {
                return null;
            }

            /*if (!this.IsUnderneathRoot(fullPath))
            {
                return null;
            }*/

            return fullPath;
        }

        /// <summary>
        /// Indicates if the path is inside the root path.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>True if path is inside the root path.</returns>
        private bool IsUnderneathRoot(string path)
        {
            return path.StartsWith(this._basePath,
                StringComparison.OrdinalIgnoreCase);
        }
        
        /// <summary>
        /// Creates a <see cref="IChangeToken"/> for the specified filter.
        /// </summary>
        /// <param name="filter">
        /// Filter string used to determine what files or folders to monitor.
        /// Example: **/*.cs, *.*, subFolder/**/*.cshtml.
        /// </param>
        /// <returns>
        /// An <see cref="IChangeToken"/> that is notified when a file matching
        /// filter is added, modified or deleted.</returns>
        public IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }
}
