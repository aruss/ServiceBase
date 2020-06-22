// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Mvc.Theming
{
    using System.Threading.Tasks;

    public class SimpleThemeInfoProvider : IThemeInfoProvider
    {
        private readonly ThemeInfoResult _themeResult;

        public SimpleThemeInfoProvider(string themeName)
        {
            this._themeResult = new ThemeInfoResult
            {
                ThemeName = themeName
            };
        }

        public Task<ThemeInfoResult> GetThemeInfoResultAsync()
        {
            return Task.FromResult(this._themeResult);
        }
    }
}
