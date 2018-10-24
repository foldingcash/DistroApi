namespace StatsDownload.TestHarness
{
    using System;
    using System.Windows.Forms;

    public interface ISelectExportFilesProvider : IDisposable
    {
        void AddFilesToList((int fileId, string fileName)[] files);

        (int fileId, string fileName)[] GetSelectedFiles();

        DialogResult ShowDialog();
    }
}