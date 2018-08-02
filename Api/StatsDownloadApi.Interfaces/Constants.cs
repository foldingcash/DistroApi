namespace StatsDownloadApi.Interfaces
{
    public class Constants
    {
        public static class ApiErrors
        {
            public static ApiError DatabaseUnavailable => new ApiError(ApiErrorCode.DatabaseUnavailable,
                ErrorMessages.DatabaseUnavailableMessage);

            public static ApiError EndDateUnsearchable => new ApiError(ApiErrorCode.EndDateUnsearchable,
                ErrorMessages.EndDateUnsearchableMessage);

            public static ApiError InvalidDateRange => new ApiError(ApiErrorCode.InvalidDateRange,
                ErrorMessages.InvalidDateRangeMessage);

            public static ApiError NegativeAmount =>
                new ApiError(ApiErrorCode.NegativeAmount, ErrorMessages.NegativeAmountMessage);

            public static ApiError NoAmount =>
                new ApiError(ApiErrorCode.NoAmount, ErrorMessages.NoAmountMessage);

            public static ApiError NoEndDate => new ApiError(ApiErrorCode.NoEndDate,
                ErrorMessages.NoEndDateMessage);

            public static ApiError NoStartDate => new ApiError(ApiErrorCode.NoStartDate,
                ErrorMessages.NoStartDateMessage);

            public static ApiError StartDateUnsearchable => new ApiError(ApiErrorCode.StartDateUnsearchable,
                ErrorMessages.StartDateUnsearchableMessage);

            public static ApiError UnexpectedException => new ApiError(ApiErrorCode.UnexpectedException,
                ErrorMessages.UnexpectedExceptionMessage);

            public static ApiError ZeroAmount =>
                new ApiError(ApiErrorCode.ZeroAmount, ErrorMessages.ZeroAmountMessage);
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