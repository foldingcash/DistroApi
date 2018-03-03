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

        private void CreateFileUploadServiceAndPerformAction(Action<IStatsUploadService> fileUploadServiceAction)
        {
            var fileUploadService = WindsorContainer.Instance.Resolve<IStatsUploadService>();
            fileUploadServiceAction?.Invoke(fileUploadService);
            CreateSeparationInLog();
        }

        private void CreateSeparationInLog()
        {
            if (LoggingTextBox.Text.Length != 0)
            {
                Log(new string('-', 147));
            }
        }

        private void EnableButtons(bool enable)
        {
            FileDownloadButton.Enabled = enable;
            UploadStatsButton.Enabled = enable;
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
                EnableButtons(false);
                await Task.Run(action);
                EnableButtons(true);
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
                    () => { CreateFileUploadServiceAndPerformAction(service => { service.UploadStatsFile(); }); });
        }
    }
}