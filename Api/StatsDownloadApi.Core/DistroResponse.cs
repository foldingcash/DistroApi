namespace StatsDownloadApi.Core
{
    using System.Collections.Generic;
    using System.Linq;

    public class DistroResponse
    {
        public DistroResponse()
        {
            Success = true;
        }

        public DistroResponse(IList<DistroError> errors)
        {
            Success = false;
            Errors = errors;
        }

        public int? ErrorCount => Errors?.Count;

        public IList<DistroError> Errors { get; }

        public DistroErrorCode FirstErrorCode => Errors?.First().ErrorCode ?? DistroErrorCode.None;

        public bool Success { get; }
    }
}