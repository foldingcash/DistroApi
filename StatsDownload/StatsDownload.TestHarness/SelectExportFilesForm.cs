namespace StatsDownload.TestHarness
{
    using System;
    using System.Windows.Forms;

    public partial class SelectExportFilesForm : Form
    {
        public SelectExportFilesForm()
        {
            InitializeComponent();
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