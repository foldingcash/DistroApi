namespace StatsDownloadApi.Interfaces
{
    using System.Collections.Generic;

    public class GetMemberStatsResponse : ApiResponse
    {
        public GetMemberStatsResponse(IList<MemberStats> members)
        {
            Members = members;
        }

        public GetMemberStatsResponse(IList<ApiError> errors) : base(errors)
        {
        }

        public IList<MemberStats> Members { get; }

        public int? MemberStatsCount => Members?.Count;
    }
}