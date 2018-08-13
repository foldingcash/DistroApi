namespace StatsDownloadApi.Interfaces
{
    using System.Collections.Generic;
    using DataTransfer;

    public class GetMemberStatsResponse : ApiResponse
    {
        public GetMemberStatsResponse(IList<Member> members)
        {
            Members = members;
        }

        public GetMemberStatsResponse(IList<ApiError> errors) : base(errors)
        {
        }

        public int? MemberCount => Members?.Count;

        public IList<Member> Members { get; }
    }
}