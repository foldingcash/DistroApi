namespace StatsDownload.Core.Interfaces
{
    using DataTransfer;

    public interface IAdditionalUserDataParserService
    {
        void Parse(UserData userData);
    }
}