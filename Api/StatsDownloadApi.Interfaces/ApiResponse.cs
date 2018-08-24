namespace StatsDownloadApi.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;

    public class ApiResponse
    {
        public ApiResponse()
        {
            Success = true;
        }

        public ApiResponse(IList<ApiError> errors)
        {
            Success = false;
            Errors = errors;
        }

        public int? ErrorCount => Errors?.Count;

        public IList<ApiError> Errors { get; }

        public ApiErrorCode FirstErrorCode => Errors?.First().ErrorCode ?? ApiErrorCode.None;

        public bool Success { get; }
    }
}