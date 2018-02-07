namespace ServiceBase.Mvc.Theming
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
    /// This file provider uses following pattern /themes/{ThemeName}/Public/*
    /// to provide static files. You will have to use following request pathes
    /// /{ThemeName}/*
    ///
    /// for example href="/FooTheme/img/foo.png" will request /Themes/FooTheme/Public/img/foo.png 
    /// </summary>
    public class ThemeFileProvider : IFileProvider
    {
        private readonly string _basePath;

        public ThemeFileProvider(string basePath)
        {
            this._basePath = basePath;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            throw new NotImplementedException();
        }

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
            if (fullPath == null)
            {
                return new NotFoundFileInfo(subpath);
            }

            return new PhysicalFileInfo(new FileInfo(fullPath));
        }

        private string GetFullPath(string path)
        {
            List<string> chunks = path.Split("/").ToList();
            chunks.Insert(1, "Public");
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

            if (!this.IsUnderneathRoot(fullPath))
            {
                return null;
            }

            return fullPath;
        }

        private bool IsUnderneathRoot(string fullPath)
        {
            return fullPath.StartsWith(this._basePath,
                StringComparison.OrdinalIgnoreCase);
        }

        public IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }
}
