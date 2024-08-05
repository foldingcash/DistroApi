namespace StatsDownloadApi.Interfaces
{
    using System.Collections.Generic;

    using StatsDownloadApi.Interfaces.DataTransfer;

    public class GetMembersResponse : ApiResponse
    {
        public GetMembersResponse(IList<Member> members)
        {
            Members = members;
        }

        public GetMembersResponse(IList<ApiError> errors)
            : base(errors)
        {
        }

        public int? MemberCount => Members?.Count;

        public IList<Member> Members { get; }
    }
}