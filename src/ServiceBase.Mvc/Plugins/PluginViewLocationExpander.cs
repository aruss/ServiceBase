namespace ServiceBase.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.AspNetCore.Mvc.Razor;
    using ServiceBase.Extensions;

    /// <summary>
    /// Specifies the contracts for a view location expander that is used by 
    /// <see cref="RazorViewEngine"/> instances to determine search paths for
    /// a view.
    /// NOTE: FileWatcher will not track changes if absolute path is provided
    /// due to #2546 bug <see href="https://github.com/aspnet/Home/issues/2546"/>
    /// </summary>
    public class PluginViewLocationExpander : IViewLocationExpander
    {
        private readonly string _basePath;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="SimpleThemeViewLocationExpander"/> class.
        /// </summary>
        /// <param name="basePath">Views base path.</param>
        public PluginViewLocationExpander(string basePath)
        {
            if (Path.IsPathRooted(basePath))
            {
                //this._basePath = Path
                //    .GetFullPath(basePath)
                //    .RemoveTrailingSlash();

                throw new NotImplementedException(
                    "Support for absolute pathes is not yet implemeted"); 
            }
            else
            {
                this._basePath = basePath
                    .Replace("./", "~/")
                    .RemoveTrailingSlash();
            }
        }
        
        /// <summary>
        /// Invoked by a <see cref="RazorViewEngine"/> to determine potential
        /// locations for a view.
        /// </summary>
        /// <param name="context">
        /// The <see cref="ViewLocationExpanderContext"/> for the current view
        /// location expansion operation.
        /// </param>
        /// <param name="viewLocations">
        /// The sequence of view locations to expand.
        /// </param>
        /// <returns>A list of expanded view locations.</returns>
        public IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            // TODO: Handle absolute pathes...

            yield return $"{this._basePath}/Views/{{1}}/{{0}}.cshtml";
            yield return $"{this._basePath}/Views/Shared/{{0}}.cshtml";
        }

        /// <summary>
        /// Invoked by a <see cref="RazorViewEngine"/> to determine the values
        /// that would be consumed by this instance of
        /// <see cref="IViewLocationExpander"/>. The calculated values are used
        /// to determine if the view location has changed since the last
        /// time it was located.
        /// </summary>
        /// <param name="context">
        /// The <see cref="ViewLocationExpanderContext"/> for the current view
        /// location expansion operation.
        /// </param>
        public void PopulateValues(ViewLocationExpanderContext context)
        {

        }
    }
}
