namespace StatsDownload.Core.TestHarness
{
    using System.Windows.Forms;

    public interface ISelectExportFilesProvider
    {
        void AddFilesToList((int fileId, string fileName)[] files);

        (int fileId, string fileName)[] GetSelectedFiles();

        DialogResult ShowDialog();
    }
}