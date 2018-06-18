namespace StatsDownload.Core.Interfaces
{
    using System.Collections.Generic;
    using DataTransfer;
    using Enums;

    public interface IErrorMessageService
    {
        string GetErrorMessage(FailedReason failedReason, FilePayload filePayload);

        string GetErrorMessage(FailedReason failedReason);

        string GetErrorMessage(IEnumerable<FailedUserData> failedUsersData);

        string GetErrorMessage(FailedUserData failedUserData);
    }
}