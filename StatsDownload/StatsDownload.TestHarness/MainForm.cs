namespace StatsDownload.TestHarness
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using Castle.MicroKernel.Registration;

    using StatsDownload.Core;

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            WindsorContainer.Instance.Register(Component.For<MainForm>().Instance(this));
        }

        internal void Log(string message)
        {
            if (LoggingTextBox.InvokeRequired)
            {
                LoggingTextBox.Invoke(new Action((() => Log(message))));
            }
            else
            {
                LoggingTextBox.Text += message;

                if (!LoggingTextBox.Text.EndsWith(Environment.NewLine))
                {
                    LoggingTextBox.Text += Environment.NewLine;
                }

                LoggingTextBox.Text += Environment.NewLine;
            }
        }

        private void CreateFileDownloadServiceAndPerformAction(Action<IFileDownloadService> fileDownloadServiceAction)
        {
            var fileDownloadService = WindsorContainer.Instance.Resolve<IFileDownloadService>();
            fileDownloadServiceAction?.Invoke(fileDownloadService);
            CreateSeparationInLog();
        }

        private void CreateSeparationInLog()
        {
            if (LoggingTextBox.Text.Length != 0)
            {
                Log(new string('-', 147));
            }
        }

        private async void FileDownloadButton_Click(object sender, EventArgs e)
        {
            await
                RunActionAsync(
                    () => { CreateFileDownloadServiceAndPerformAction(service => { service.DownloadStatsFile(); }); });
        }

        private async Task RunActionAsync(Action action)
        {
            try
            {
                FileDownloadButton.Enabled = false;
                await Task.Run(action);
                FileDownloadButton.Enabled = true;
            }
            catch (Exception exception)
            {
                Log(exception.ToString());
            }
        }

        private async void UploadStatsButton_Click(object sender, EventArgs e)
        {
            await
                RunActionAsync(
                    () => { CreateFileDownloadServiceAndPerformAction(service => { service.UploadStatsFile(); }); });
        }
    }
}