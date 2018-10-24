namespace StatsDownload.TestHarness
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    public partial class SelectExportFilesForm : Form, ISelectExportFilesProvider
    {
        public SelectExportFilesForm()
        {
            InitializeComponent();
        }

        public void AddFilesToList((int fileId, string fileName)[] files)
        {
            ExportFilesCheckedListBox.Items.Clear();

            foreach ((int fileId, string fileName) file in files)
            {
                ExportFilesCheckedListBox.Items.Add(file, CheckState.Unchecked);
            }
        }

        public (int fileId, string fileName)[] GetSelectedFiles()
        {
            return ExportFilesCheckedListBox.CheckedItems.Cast<(int, string)>().ToArray();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void FinishedButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}