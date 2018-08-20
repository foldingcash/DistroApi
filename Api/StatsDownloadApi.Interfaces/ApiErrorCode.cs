namespace StatsDownloadApi.Interfaces
{
    public enum ApiErrorCode
    {
        None = 0000,

        NoStartDate = 1000,

        NoEndDate = 1010,

        StartDateUnsearchable = 1020,

        EndDateUnsearchable = 1030,

        InvalidDateRange = 1040,

        NoAmount = 1050,

        ZeroAmount = 1060,

        NegativeAmount = 1070,

        DatabaseUnavailable = 8000,

        DatabaseMissingRequiredObjects = 8005,

        UnexpectedException = 9000
    }
}