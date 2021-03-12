namespace StatsDownload.Email
{
    using System.Collections.Generic;

    public class EmailSettings
    {
        public string Address { get; set; }

        public string DisplayName { get; set; }

        public string Host { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

        public ICollection<string> Receivers { get; set; }
    }
}