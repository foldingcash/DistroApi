namespace StatsDownload.Core.TestHarness
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Common;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using Castle.MicroKernel.Registration;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.TestHarness.CastleWindsor;

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

        private async void CompressButton_Click(object sender, EventArgs e)
        {
            await RunActionAsync(() =>
            {
                string compressDirectory = CompressDirectoryTextBox.Text;

                if (!Directory.Exists(compressDirectory))
                {
                    Log(
                        $"The directory does not exist, provide a new directory and try again. Directory: '{compressDirectory}'");
                    return Task.CompletedTask;
                }

                string[] importFiles = Directory.GetFiles(compressDirectory, "*.txt", SearchOption.TopDirectoryOnly);

                if (importFiles.Length == 0)
                {
                    Log(
                        $"There are no text files in the top directory, provide a directory with files to compress and try again. Directory: '{compressDirectory}'");
                    return Task.CompletedTask;
                }

                int filesRemaining = importFiles.Length;

                IFileCompressionService fileCompressionService = null;
                try
                {
                    fileCompressionService = WindsorContainer.Instance.Resolve<IFileCompressionService>();

                    Log($"'{filesRemaining}' files are to be imported");

                    foreach (string importFile in importFiles)
                    {
                        fileCompressionService.CompressFile(importFile, $"{importFile}.bz2");
                        filesRemaining--;
                        Log($"File imported. '{filesRemaining}' remaining files to be imported");
                    }
                }
                finally
                {
                    WindsorContainer.Instance.Release(fileCompressionService);
                }

                return Task.CompletedTask;
            });
        }

        private async Task CreateFileDownloadServiceAndPerformAction(
            Func<IFileDownloadService, Task> fileDownloadServiceAction)
        {
            IFileDownloadService fileDownloadService = null;

            try
            {
                fileDownloadService = WindsorContainer.Instance.Resolve<IFileDownloadService>();
                Task task = fileDownloadServiceAction?.Invoke(fileDownloadService) ?? Task.CompletedTask;
                await task;
            }
            finally
            {
                WindsorContainer.Instance.Release(fileDownloadService);
            }
        }

        private void CreateSeparationInLog()
        {
            if (LoggingTextBox.Text.Length != 0)
            {
                Log(new string('-', 100));
            }
        }

        private async void DecompressButton_Click(object sender, EventArgs e)
        {
            await RunActionAsync(() =>
            {
                string decompressDirectory = DecompressDirectoryTextBox.Text;

                if (!Directory.Exists(decompressDirectory))
                {
                    Log(
                        $"The directory does not exist, provide a new directory and try again. Directory: '{decompressDirectory}'");
                    return Task.CompletedTask;
                }

                string[] importFiles = Directory.GetFiles(decompressDirectory, "*.bz2", SearchOption.TopDirectoryOnly);

                if (importFiles.Length == 0)
                {
                    Log(
                        $"There are no text files in the top directory, provide a directory with files to decompress and try again. Directory: '{decompressDirectory}'");
                    return Task.CompletedTask;
                }

                int filesRemaining = importFiles.Length;

                IFileCompressionService fileCompressionService = null;
                try
                {
                    fileCompressionService = WindsorContainer.Instance.Resolve<IFileCompressionService>();

                    Log($"'{filesRemaining}' files are to be imported");

                    foreach (string importFile in importFiles)
                    {
                        fileCompressionService.DecompressFile(importFile,
                            importFile.Substring(importFile.Length - 4, 4));
                        filesRemaining--;
                        Log($"File imported. '{filesRemaining}' remaining files to be imported");
                    }
                }
                finally
                {
                    WindsorContainer.Instance.Release(fileCompressionService);
                }

                return Task.CompletedTask;
            });
        }

        private void EnableGui(bool enable)
        {
            FileDownloadButton.Enabled = enable;

            TestEmailButton.Enabled = enable;

            ExportDirectoryTextBox.Enabled = enable;
            ExportAllRadioButton.Enabled = enable;
            ExportSubsetRadioButton.Enabled = enable;
            ExportButton.Enabled = enable;

            ImportDirectoryTextBox.Enabled = enable;
            ImportButton.Enabled = enable;

            CompressDirectoryTextBox.Enabled = enable;
            CompressButton.Enabled = enable;

            DecompressDirectoryTextBox.Enabled = enable;
            DecompressButton.Enabled = enable;

            ControlBox = enable;
        }

        private async void ExportButton_Click(object sender, EventArgs e)
        {
            await RunActionAsync(() =>
            {
                if (ExportAllRadioButton.Checked)
                {
                    MassExport(files => files);
                }
                else if (ExportSubsetRadioButton.Checked)
                {
                    MassExport(SelectSubsetOfExportFiles);
                }

                return Task.CompletedTask;
            });
        }

        private async void FileDownloadButton_Click(object sender, EventArgs e)
        {
            await RunActionAsync(async () =>
            {
                await CreateFileDownloadServiceAndPerformAction(async service =>
                {
                    await service.DownloadStatsFile();
                });
            });
        }

        private (int fileId, string fileName)[] GetAllFiles()
        {
            var downloads = new List<(int downloadId, string fileName)>();

            IStatsDownloadDatabaseService databaseService = null;

            try
            {
                databaseService = WindsorContainer.Instance.Resolve<IStatsDownloadDatabaseService>();

                databaseService.CreateDatabaseConnectionAndExecuteAction(service =>
                {
                    using (DbDataReader fileReader =
                        service.ExecuteReader("SELECT FileId, FileName, FileExtension FROM FoldingCoin.Files"))
                    {
                        if (!fileReader.HasRows)
                        {
                            return;
                        }

                        while (fileReader.Read())
                        {
                            downloads.Add((fileReader.GetInt32(0),
                                              $"{fileReader.GetString(1)}{fileReader.GetString(2)}"));
                        }
                    }
                });
            }
            finally
            {
                WindsorContainer.Instance.Release(databaseService);
            }

            return downloads.ToArray();
        }

        private async void ImportButton_Click(object sender, EventArgs e)
        {
            await RunActionAsync(async () =>
            {
                string importDirectory = ImportDirectoryTextBox.Text;

                if (!Directory.Exists(importDirectory))
                {
                    Log(
                        $"The directory does not exist, provide a new directory and try again. Directory: '{importDirectory}'");
                    return;
                }

                string[] importFiles = Directory.GetFiles(importDirectory, "*.txt", SearchOption.TopDirectoryOnly);

                var filesRemaining = 0;

                if (importFiles.Length > 0)
                {
                    Log("Importing .txt files");

                    ConfigurationManager.AppSettings["DisableMinimumWaitTime"] = "true";

                    filesRemaining = importFiles.Length;

                    Log($"'{filesRemaining}' files are to be imported");

                    IFileCompressionService fileCompressionService = null;

                    try
                    {
                        fileCompressionService = WindsorContainer.Instance.Resolve<IFileCompressionService>();

                        foreach (string importFile in importFiles)
                        {
                            var compressedFile = $"{importFile}.bz2";

                            fileCompressionService.CompressFile(importFile, compressedFile);

                            ConfigurationManager.AppSettings["DownloadUri"] = compressedFile;

                            await CreateFileDownloadServiceAndPerformAction(async service =>
                            {
                                await service.DownloadStatsFile();
                            });

                            filesRemaining--;

                            Log($"File imported. '{filesRemaining}' remaining files to be imported");
                        }
                    }
                    finally
                    {
                        WindsorContainer.Instance.Release(fileCompressionService);
                    }
                }

                ConfigurationManager.RefreshSection("appSettings");

                // TODO: Refactor dup code
                importFiles = Directory.GetFiles(importDirectory, "*.txt.bz2", SearchOption.TopDirectoryOnly);

                if (importFiles.Length == 0)
                {
                    Log(
                        $"There are no text files in the top directory, provide a directory with files to import and try again. Directory: '{importDirectory}'");
                    return;
                }

                Log("Importing .txt.bz2 files");

                ConfigurationManager.AppSettings["DisableMinimumWaitTime"] = "true";

                filesRemaining = importFiles.Length;

                Log($"'{filesRemaining}' files are to be imported");

                foreach (string importFile in importFiles)
                {
                    ConfigurationManager.AppSettings["DownloadUri"] = importFile;

                    await CreateFileDownloadServiceAndPerformAction(async service =>
                    {
                        await service.DownloadStatsFile();
                    });

                    filesRemaining--;

                    Log($"File imported. '{filesRemaining}' remaining files to be imported");
                }

                ConfigurationManager.RefreshSection("appSettings");
            });
        }

        private void MassExport(
            Func<(int fileId, string fileName)[], (int fileId, string fileName)[]> selectFilesFunction)
        {
            string exportDirectory = ExportDirectoryTextBox.Text;

            if (!Directory.Exists(exportDirectory))
            {
                Log(
                    $"The directory does not exist, provide a new directory and try again. Directory: '{exportDirectory}'");
                return;
            }

            (int fileId, string fileName)[] allFiles = GetAllFiles();

            IStatsUploadDatabaseService service = null;

            try
            {
                service = WindsorContainer.Instance.Resolve<IStatsUploadDatabaseService>();

                (int fileId, string fileName)[] subsetFiles = selectFilesFunction(allFiles);

                int filesRemaining = subsetFiles.Length;

                Log($"'{filesRemaining}' files are to be exported");

                foreach ((int fileId, string fileName) in subsetFiles)
                {
                    string path = Path.Combine(exportDirectory, fileName);
                    string fileData = service.GetFileData(fileId);

                    File.WriteAllText(path, fileData);

                    filesRemaining--;

                    Log($"File exported. '{filesRemaining}' remaining files to be exported");
                }
            }
            finally
            {
                WindsorContainer.Instance.Release(service);
            }
        }

        private async Task RunActionAsync(Func<Task> action)
        {
            try
            {
                EnableGui(false);
                await action();
            }
            catch (Exception exception)
            {
                Log(exception.ToString());
            }
            finally
            {
                CreateSeparationInLog();
                EnableGui(true);
            }
        }

        private (int fileId, string fileName)[] SelectSubsetOfExportFiles((int fileId, string fileName)[] files)
        {
            ISelectExportFilesProvider selectExportFilesForm = null;

            try
            {
                selectExportFilesForm = WindsorContainer.Instance.Resolve<ISelectExportFilesProvider>();

                selectExportFilesForm.AddFilesToList(files);

                DialogResult result = selectExportFilesForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    return selectExportFilesForm.GetSelectedFiles();
                }
            }
            finally
            {
                WindsorContainer.Instance.Release(selectExportFilesForm);
            }

            return new (int, string)[0];
        }

        private async void TestEmailButton_Click(object sender, EventArgs e)
        {
            await RunActionAsync(() =>
            {
                IStatsDownloadEmailService emailService = null;

                try
                {
                    emailService = WindsorContainer.Instance.Resolve<IStatsDownloadEmailService>();

                    emailService.SendTestEmail();
                }
                finally
                {
                    WindsorContainer.Instance.Release(emailService);
                }

                return Task.CompletedTask;
            });
        }
    }
}