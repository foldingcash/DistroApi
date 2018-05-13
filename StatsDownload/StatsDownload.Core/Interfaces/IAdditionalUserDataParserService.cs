namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.DataTransfer;

    public interface IAdditionalUserDataParserService
    {
        void Parse(UserData userData);
    }
}