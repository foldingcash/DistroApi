namespace StatsDownload.Core.Interfaces
{
    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IAdditionalUserDataParserService
    {
        void Parse(UserData userData);
    }
}