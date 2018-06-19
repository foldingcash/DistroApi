namespace StatsDownload.TestHarness
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Castle.MicroKernel.Registration;
    using CastleWindsor;
    using Core.Interfaces;

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            WindsorContainer.Instance.Register(Component.For<Action<string>>().Instance(Log));
        }

        private void AppendToLog(string message)
        {
            LoggingTextBox.AppendText(message);
        }

        private void CreateFileDownloadServiceAndPerformAction(Action<IFileDownloadService> fileDownloadServiceAction)
        {
            var fileDownloadService = WindsorContainer.Instance.Resolve<IFileDownloadService>();
            fileDownloadServiceAction?.Invoke(fileDownloadService);
            CreateSeparationInLog();
            WindsorContainer.Instance.Release(fileDownloadService);
        }

        private void CreateFileUploadServiceAndPerformAction(Action<IStatsUploadService> fileUploadServiceAction)
        {
            var fileUploadService = WindsorContainer.Instance.Resolve<IStatsUploadService>();
            fileUploadServiceAction?.Invoke(fileUploadService);
            CreateSeparationInLog();
            WindsorContainer.Instance.Release(fileUploadService);
        }

        private void CreateSeparationInLog()
        {
            if (LoggingTextBox.Text.Length != 0)
            {
                Log(new string('-', 100));
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

        internal void Log(string message)
        {
            if (LoggingTextBox.InvokeRequired)
            {
                LoggingTextBox.Invoke(new Action(() => Log(message)));
            }
            else
            {
                AppendToLog(message);
                AppendToLog(Environment.NewLine);
                AppendToLog(Environment.NewLine);
            }
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
                    () => { CreateFileUploadServiceAndPerformAction(service => { service.UploadStatsFiles(); }); });
        }
    }
}