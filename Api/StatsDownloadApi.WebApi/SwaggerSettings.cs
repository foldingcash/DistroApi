namespace StatsDownloadApi.WebApi
{
    public class SwaggerSettings
    {
        public string JsonUrl { get; set; }

        public string Name => Title + " " + Version;

        public string SwaggerUrl { get; set; }

        public string Title { get; set; }

        public string Version { get; set; }
    }
}