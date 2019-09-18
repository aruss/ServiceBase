// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Mvc.Razor
{
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Mvc.Razor;

    /// <summary>
    /// Specifies the contracts for a view location expander that is used by 
    /// <see cref="Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine"/> instances 
    /// to determine search paths for a view.
    /// </summary>
    public class ActionViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(
             ViewLocationExpanderContext context,
             IEnumerable<string> viewLocations)
        {
            string uiCultureName = context.Values["UiCultureName"];

            if (uiCultureName != null)
            {
                yield return $"~/Actions/{{1}}/Views/{{0}}.{uiCultureName}.cshtml";
            }

            yield return "~/Actions/{1}/Views/{0}.cshtml";

            if (uiCultureName != null)
            {
                yield return $"~/Actions/Shared/{{0}}.{uiCultureName}.cshtml";
            }

            yield return "~/Actions/Shared/{0}.cshtml";
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            IRequestCultureFeature requestCultureFeature =
                context.ActionContext.HttpContext.Features.Get<IRequestCultureFeature>();

            if (requestCultureFeature != null)
            {
                CultureInfo culture = requestCultureFeature.RequestCulture.UICulture;
                context.Values["UiCultureName"] = culture.Name;
            }
            else
            {
                context.Values["UiCultureName"] = null; 
            }
        }
    }
}
