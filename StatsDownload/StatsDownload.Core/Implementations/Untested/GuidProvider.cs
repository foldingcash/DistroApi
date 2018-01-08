namespace StatsDownload.Core
{
    using System;

    public class GuidProvider : IGuidService
    {
        public Guid NextGuid()
        {
            return Guid.NewGuid();
        }
    }
}