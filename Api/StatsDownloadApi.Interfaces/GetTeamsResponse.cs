namespace StatsDownloadApi.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using DataTransfer;

    public class GetTeamsResponse
    {
        public GetTeamsResponse(IList<Team> teams)
        {
            Success = true;
            Teams = teams;
        }

        public GetTeamsResponse(IList<ApiError> errors)
        {
            Success = false;
            Errors = errors;
        }

        public int? ErrorCount => Errors?.Count;

        public IList<ApiError> Errors { get; }

        public ApiErrorCode FirstErrorCode => Errors?.First().ErrorCode ?? ApiErrorCode.None;

        public bool Success { get; }

        public int? TeamCount => Teams?.Count;

        public IList<Team> Teams { get; set; }
    }
}