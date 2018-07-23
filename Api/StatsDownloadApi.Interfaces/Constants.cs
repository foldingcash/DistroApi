namespace StatsDownloadApi.Interfaces
{
    public class Constants
    {
        public static class DistroErrors
        {
            public static DistroError DatabaseUnavailable => new DistroError(DistroErrorCode.DatabaseUnavailable,
                ErrorMessages.DatabaseUnavailableMessage);

            public static DistroError EndDateUnsearchable => new DistroError(DistroErrorCode.EndDateUnsearchable,
                ErrorMessages.EndDateUnsearchableMessage);

            public static DistroError InvalidDateRange => new DistroError(DistroErrorCode.InvalidDateRange,
                ErrorMessages.InvalidDateRangeMessage);

            public static DistroError NegativeAmount =>
                new DistroError(DistroErrorCode.NegativeAmount, ErrorMessages.NegativeAmountMessage);

            public static DistroError NoAmount =>
                new DistroError(DistroErrorCode.NoAmount, ErrorMessages.NoAmountMessage);

            public static DistroError NoEndDate => new DistroError(DistroErrorCode.NoEndDate,
                ErrorMessages.NoEndDateMessage);

            public static DistroError NoStartDate => new DistroError(DistroErrorCode.NoStartDate,
                ErrorMessages.NoStartDateMessage);

            public static DistroError StartDateUnsearchable => new DistroError(DistroErrorCode.StartDateUnsearchable,
                ErrorMessages.StartDateUnsearchableMessage);

            public static DistroError UnexpectedException => new DistroError(DistroErrorCode.UnexpectedException,
                ErrorMessages.UnexpectedExceptionMessage);

            public static DistroError ZeroAmount =>
                new DistroError(DistroErrorCode.ZeroAmount, ErrorMessages.ZeroAmountMessage);
        }

        public static class ErrorMessages
        {
            public static string DatabaseUnavailableMessage =>
                "The database is unavailable. Try again in a short period of time. If the problem continues, then contact the technical team about the problem.";

            public static string EndDateUnsearchableMessage =>
                "The end date provided is unsearchable. The end date must not be today or a future day's date. Provide a new end date and try again.";

            public static string InvalidDateRangeMessage =>
                "The date range provided is invalid. The end date must not be a date prior to the start date. Provide a new date range and try again.";

            public static string NegativeAmountMessage =>
                "The amount provided was negative. Provide an amount greater than zero and try again.";

            public static string NoAmountMessage =>
                "No amount was provided; ensure the distribution amount was provided as a query paramter and try again.";

            public static string NoEndDateMessage =>
                "No end date was provided; ensure the end date was provided as a query parameter in the format MM-DD-YYYY and try again.";

            public static string NoStartDateMessage =>
                "No start date was provided; ensure the start date was provided as a query parameter in the format MM-DD-YYYY and try again.";

            public static string StartDateUnsearchableMessage =>
                "The start date provided is unsearchable. The start date must not be today or a future day's date. Provide a new start date and try again.";

            public static string UnexpectedExceptionMessage =>
                "There was an unexpected error. Try again and if the problem continues, then contact the technical team about the problem.";

            public static string ZeroAmountMessage =>
                "The amount provided was zero. Provide an amount greater than zero and try again.";
        }
    }
}