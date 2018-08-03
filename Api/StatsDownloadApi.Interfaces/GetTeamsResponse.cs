namespace StatsDownloadApi.Interfaces
{
    using System.Collections.Generic;
    using DataTransfer;

    public class GetTeamsResponse : ApiResponse
    {
        public GetTeamsResponse(IList<Team> teams)
        {
            Teams = teams;
        }

        public GetTeamsResponse(IList<ApiError> errors) : base(errors)
        {
        }

        public int? TeamCount => Teams?.Count;

        public IList<Team> Teams { get; set; }
    }
}