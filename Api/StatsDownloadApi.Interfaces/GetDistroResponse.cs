namespace StatsDownloadApi.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using DataTransfer;

    public class GetDistroResponse : ApiResponse
    {
        public GetDistroResponse(IList<DistroUser> distro)
        {
            Distro = distro;
        }

        public GetDistroResponse(IList<ApiError> errors) : base(errors)
        {
        }

        public IList<DistroUser> Distro { get; }

        public int? DistroCount => Distro?.Count;

        public decimal? TotalDistro => Distro?.Sum(user => user.Amount);

        public long? TotalPoints => Distro?.Sum(user => user.PointsGained);

        public long? TotalWorkUnits => Distro?.Sum(user => user.WorkUnitsGained);
    }
}