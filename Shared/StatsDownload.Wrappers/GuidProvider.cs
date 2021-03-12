namespace StatsDownload.Wrappers
{
    using System;

    using StatsDownload.Core.Interfaces;

    public class GuidProvider : IGuidService
    {
        public Guid NewGuid()
        {
            return Guid.NewGuid();
        }
    }
}