namespace StatsDownload.Core.Interfaces
{
    using System.Collections.Generic;

    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Enums;

    public interface IErrorMessageService
    {
        string GetErrorMessage(FailedReason failedReason, FilePayload filePayload, StatsDownloadService service);

        string GetErrorMessage(FailedReason failedReason, StatsDownloadService service);

        string GetErrorMessage(IEnumerable<FailedUserData> failedUsersData);

        string GetErrorMessage(FailedUserData failedUserData);
    }
}