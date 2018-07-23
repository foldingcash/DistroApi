﻿namespace StatsDownloadApi.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using DataTransfer;

    public class DistroResponse
    {
        public DistroResponse(IList<DistroUser> distro)
        {
            Success = true;
            Distro = distro;
        }

        public DistroResponse(IList<DistroError> errors)
        {
            Success = false;
            Errors = errors;
        }

        public IList<DistroUser> Distro { get; }

        public int? DistroCount => Distro?.Count;

        public int? ErrorCount => Errors?.Count;

        public IList<DistroError> Errors { get; }

        public DistroErrorCode FirstErrorCode => Errors?.First().ErrorCode ?? DistroErrorCode.None;

        public bool Success { get; }

        public decimal? TotalDistro => Distro?.Sum(user => user.Amount);

        public long? TotalPoints => Distro?.Sum(user => user.PointsGained);

        public long? TotalWorkUnits => Distro?.Sum(user => user.WorkUnitsGained);
    }
}