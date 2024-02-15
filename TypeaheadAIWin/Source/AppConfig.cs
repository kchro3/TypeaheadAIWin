namespace TypeaheadAIWin.Source
{
    public static class AppConfig
    {
        public static string GetApiBaseUrl()
        {
#if DEBUG
            // URL for development (127.0.0.1 resolves faster than localhost)
            return "http://127.0.0.1:8787";
#else
            // URL for production
            return "https://api.typeahead.ai";
#endif
        }
    }
}
