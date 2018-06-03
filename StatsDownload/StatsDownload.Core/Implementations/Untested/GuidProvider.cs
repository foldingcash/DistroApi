namespace StatsDownload.Core.Implementations.Untested
{
    using System;

    using StatsDownload.Core.Interfaces;

    public class GuidProvider : IGuidService
    {
        public Guid NextGuid()
        {
            return Guid.NewGuid();
        }
    }
}