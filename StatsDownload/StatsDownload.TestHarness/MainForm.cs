namespace StatsDownload.TestHarness
{
    using System;
    using System.Configuration;
    using System.IO;
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

        private void EnableGui(bool enable)
        {
            FileDownloadButton.Enabled = enable;
            UploadStatsButton.Enabled = enable;
            ImportDirectoryTextBox.Enabled = enable;
            ImportButton.Enabled = enable;
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
        }

        private async void FileDownloadButton_Click(object sender, EventArgs e)
        {
            await
                RunActionAsync(
                    () => { CreateFileDownloadServiceAndPerformAction(service => { service.DownloadStatsFile(); }); });
        }

        private async void ImportButton_Click(object sender, EventArgs e)
        {
            await RunActionAsync(() =>
            {
                string importDirectory = ImportDirectoryTextBox.Text;

                if (!Directory.Exists(importDirectory))
                {
                    Log(
                        $"The directory does not exist, provide a new directory and try again. Directory: '{importDirectory}'");
                    CreateSeparationInLog();
                    return;
                }

                string[] importFiles = Directory.GetFiles(importDirectory, "*.txt", SearchOption.TopDirectoryOnly);

                if (importFiles.Length == 0)
                {
                    Log(
                        $"There are no text files in the top directory, provide a directory with files to import and try again. Directory: '{importDirectory}'");
                    CreateSeparationInLog();
                    return;
                }

                ConfigurationManager.AppSettings["FileCompressionDisabled"] = "true";
                ConfigurationManager.AppSettings["DisableMinimumWaitTime"] = "true";

                foreach (string importFile in importFiles)
                {
                    ConfigurationManager.AppSettings["DownloadUri"] = importFile;

                    CreateFileDownloadServiceAndPerformAction(service => { service.DownloadStatsFile(); });
                }

                CreateFileUploadServiceAndPerformAction(service => { service.UploadStatsFiles(); });

                ConfigurationManager.RefreshSection("appSettings");
            });
        }

        private async Task RunActionAsync(Action action)
        {
            try
            {
                EnableGui(false);
                await Task.Run(action);
            }
            catch (Exception exception)
            {
                Log(exception.ToString());
            }
            finally
            {
                EnableGui(true);
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