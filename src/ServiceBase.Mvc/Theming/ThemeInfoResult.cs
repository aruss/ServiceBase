// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase.Mvc.Theming
{
    /// <summary>
    /// Theme information for current request.
    /// </summary>
    public class ThemeInfoResult
    {
        /// <summary>
        /// Theme name for current request.
        /// </summary>
        public string ThemeName { get; set; }
    }
}
