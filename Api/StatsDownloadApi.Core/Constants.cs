namespace StatsDownloadApi.Core
{
    public class Constants
    {
        public static class DistroErrors
        {
            public static DistroError DatabaseUnavailable => new DistroError(DistroErrorCode.DatabaseUnavailable,
                ErrorMessages.DatabaseUnavailableMessage);
        }

        public static class ErrorMessages
        {
            public static string DatabaseUnavailableMessage =>
                "The database is unavailable. Try again in a short period of time. If the problem continues, then contact the technical team.";
        }
    }
}