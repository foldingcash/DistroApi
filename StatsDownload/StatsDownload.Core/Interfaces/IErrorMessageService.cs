namespace StatsDownload.Core
{
    using System.Collections.Generic;
    using Interfaces.DataTransfer;
    using Interfaces.Enums;

    public interface IErrorMessageService
    {
        string GetErrorMessage(FailedReason failedReason, FilePayload filePayload);

        string GetErrorMessage(FailedReason failedReason);

        string GetErrorMessage(IEnumerable<FailedUserData> failedUsersData);

        string GetErrorMessage(FailedUserData failedUserData);
    }
}