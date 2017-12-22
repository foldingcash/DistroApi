namespace StatsDownload.Core
{
    using System;
    using System.IO;

    public class FileNameProvider : IFileNameService
    {
        private const string FileName = "daily_user_summary.txt.bz2";

        private readonly IGuidService guidService;

        public FileNameProvider(IGuidService guidService)
        {
            this.guidService = guidService;
        }

        public string GetRandomFileNamePath(string directory)
        {
            Guid guid = NextGuid();
            string fileName = $"{guid}.{FileName}";
            return Path.Combine(directory, fileName);
        }

        private Guid NextGuid()
        {
            return guidService.NextGuid();
        }
    }
}