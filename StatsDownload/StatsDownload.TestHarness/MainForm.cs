namespace StatsDownload.TestHarness
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using Castle.MicroKernel.Registration;

    using StatsDownload.Core;

    public partial class MainForm : Form
    {
        private ServiceHost host;

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

        private void CreateSeparationInLog()
        {
            if (LoggingTextBox.Text.Length != 0)
            {
                Log(new string('-', 147) + Environment.NewLine);
            }
        }

        private async void FileDownloadButton_Click(object sender, EventArgs e)
        {
            await RunActionAsync(
                () =>
                    {
                        var fileDownloadService = WindsorContainer.Instance.Resolve<IFileDownloadService>();
                        fileDownloadService.DownloadStatsFile();
                        CreateSeparationInLog();
                    });
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

        private void StartServerButton_Click(object sender, EventArgs e)
        {
            var baseAddress = "http://127.0.0.1";
            host = new ServiceHost(typeof(TestHarnessFileServer), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(ITestHarnessFileServer), new WebHttpBinding(), "")
                .Behaviors.Add(new WebHttpBehavior());
            var smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);
            host.Open();
            StartServerButton.Enabled = false;
            StopServerButton.Enabled = true;
        }

        private void StopServerButton_Click(object sender, EventArgs e)
        {
            host?.Close();
            StartServerButton.Enabled = true;
            StopServerButton.Enabled = false;
        }
    }
}