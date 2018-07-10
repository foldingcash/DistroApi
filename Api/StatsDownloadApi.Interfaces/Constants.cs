namespace StatsDownloadApi.Interfaces
{
    using System;

    public class Constants
    {
        public static class DistroErrors
        {
            public static DistroError DatabaseUnavailable => new DistroError(DistroErrorCode.DatabaseUnavailable,
                ErrorMessages.DatabaseUnavailableMessage);

            public static DistroError EndDateInvalid => new DistroError(DistroErrorCode.EndDateInvalid,
                ErrorMessages.EndDateInvalidMessage);

            public static DistroError StartDateInvalid => new DistroError(DistroErrorCode.StartDateInvalid,
                ErrorMessages.StartDateInvalidMessage);
        }

        public static class ErrorMessages
        {
            public static string DatabaseUnavailableMessage =>
                "The database is unavailable. Try again in a short period of time. If the problem continues, then contact the technical team.";

            public static string EndDateInvalidMessage =>
                $"The end date is invalid, ensure the end date was provided as a query parameter in the format MM-DD-YYYY and greater than or equal to {DateTime.MinValue:MM-dd-yyyy} and less than or equal to {DateTime.MaxValue:MM-dd-yyyy}.";

            public static string StartDateInvalidMessage =>
                $"The start date is invalid, ensure the start date was provided as a query parameter in the format MM-DD-YYYY and greater than or equal to {DateTime.MinValue:MM-dd-yyyy} and less than or equal to {DateTime.MaxValue:MM-dd-yyyy}.";
        }
    }
}