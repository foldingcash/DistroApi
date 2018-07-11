namespace StatsDownloadApi.Interfaces
{
    public enum DistroErrorCode
    {
        None = 0000,

        NoStartDate = 1000,

        NoEndDate = 1010,

        StartDateUnsearchable = 1020,

        EndDateUnsearchable = 1030,

        DatabaseUnavailable = 8000
    }
}