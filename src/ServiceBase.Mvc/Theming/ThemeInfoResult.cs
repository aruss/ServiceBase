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
        public string RequestTheme { get; set; }

        /// <summary>
        /// Default fallback theme name.
        /// </summary>
        public string DefaultTheme { get; set; }
    }
}
