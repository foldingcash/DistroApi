namespace StatsDownloadApi.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using StatsDownloadApi.Interfaces.DataTransfer;

    public class GetDistroResponse : ApiResponse
    {
        public GetDistroResponse(IList<DistroUser> distro, DateTime startDateTime, DateTime endDateTime)
        {
            Distro = distro;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
        }

        public GetDistroResponse(IList<ApiError> errors)
            : base(errors)
        {
        }

        public IList<DistroUser> Distro { get; }

        public int? DistroCount => Distro?.Count;

        public DateTime EndDateTime { get; }

        public DateTime StartDateTime { get; }

        public decimal? TotalDistro => Distro?.Sum(user => user.Amount);

        public long? TotalPoints => Distro?.Sum(user => user.PointsGained);

        public long? TotalWorkUnits => Distro?.Sum(user => user.WorkUnitsGained);
    }
}