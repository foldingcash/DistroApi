namespace StatsDownload.TestHarness
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.FileDownloadButton = new System.Windows.Forms.Button();
            this.LoggingTextBox = new System.Windows.Forms.TextBox();
            this.UploadStatsButton = new System.Windows.Forms.Button();
            this.ImportButton = new System.Windows.Forms.Button();
            this.MassImportGroupBox = new System.Windows.Forms.GroupBox();
            this.ImportDirectoryLabel = new System.Windows.Forms.Label();
            this.ImportDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.MassExportGroupBox = new System.Windows.Forms.GroupBox();
            this.ExportDirectoryLabel = new System.Windows.Forms.Label();
            this.ExportDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.ExportButton = new System.Windows.Forms.Button();
            this.ExportAllRadioButton = new System.Windows.Forms.RadioButton();
            this.ExportSubsetRadioButton = new System.Windows.Forms.RadioButton();
            this.MassImportGroupBox.SuspendLayout();
            this.MassExportGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // FileDownloadButton
            // 
            this.FileDownloadButton.Location = new System.Drawing.Point(16, 15);
            this.FileDownloadButton.Margin = new System.Windows.Forms.Padding(4);
            this.FileDownloadButton.Name = "FileDownloadButton";
            this.FileDownloadButton.Size = new System.Drawing.Size(263, 49);
            this.FileDownloadButton.TabIndex = 0;
            this.FileDownloadButton.Text = "File Download";
            this.FileDownloadButton.UseVisualStyleBackColor = true;
            this.FileDownloadButton.Click += new System.EventHandler(this.FileDownloadButton_Click);
            // 
            // LoggingTextBox
            // 
            this.LoggingTextBox.Location = new System.Drawing.Point(287, 15);
            this.LoggingTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.LoggingTextBox.Multiline = true;
            this.LoggingTextBox.Name = "LoggingTextBox";
            this.LoggingTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LoggingTextBox.Size = new System.Drawing.Size(673, 483);
            this.LoggingTextBox.TabIndex = 1;
            // 
            // UploadStatsButton
            // 
            this.UploadStatsButton.Location = new System.Drawing.Point(16, 71);
            this.UploadStatsButton.Margin = new System.Windows.Forms.Padding(4);
            this.UploadStatsButton.Name = "UploadStatsButton";
            this.UploadStatsButton.Size = new System.Drawing.Size(263, 49);
            this.UploadStatsButton.TabIndex = 2;
            this.UploadStatsButton.Text = "Stats Upload";
            this.UploadStatsButton.UseVisualStyleBackColor = true;
            this.UploadStatsButton.Click += new System.EventHandler(this.UploadStatsButton_Click);
            // 
            // ImportButton
            // 
            this.ImportButton.Location = new System.Drawing.Point(6, 86);
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(251, 49);
            this.ImportButton.TabIndex = 3;
            this.ImportButton.Text = "Import";
            this.ImportButton.UseVisualStyleBackColor = true;
            this.ImportButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // MassImportGroupBox
            // 
            this.MassImportGroupBox.Controls.Add(this.ImportDirectoryLabel);
            this.MassImportGroupBox.Controls.Add(this.ImportDirectoryTextBox);
            this.MassImportGroupBox.Controls.Add(this.ImportButton);
            this.MassImportGroupBox.Location = new System.Drawing.Point(16, 357);
            this.MassImportGroupBox.Name = "MassImportGroupBox";
            this.MassImportGroupBox.Size = new System.Drawing.Size(263, 141);
            this.MassImportGroupBox.TabIndex = 4;
            this.MassImportGroupBox.TabStop = false;
            this.MassImportGroupBox.Text = "Mass Import";
            // 
            // ImportDirectoryLabel
            // 
            this.ImportDirectoryLabel.AutoSize = true;
            this.ImportDirectoryLabel.Location = new System.Drawing.Point(6, 29);
            this.ImportDirectoryLabel.Name = "ImportDirectoryLabel";
            this.ImportDirectoryLabel.Size = new System.Drawing.Size(108, 17);
            this.ImportDirectoryLabel.TabIndex = 5;
            this.ImportDirectoryLabel.Text = "Import Directory";
            // 
            // ImportDirectoryTextBox
            // 
            this.ImportDirectoryTextBox.Location = new System.Drawing.Point(6, 49);
            this.ImportDirectoryTextBox.Name = "ImportDirectoryTextBox";
            this.ImportDirectoryTextBox.Size = new System.Drawing.Size(251, 22);
            this.ImportDirectoryTextBox.TabIndex = 4;
            this.ImportDirectoryTextBox.Text = "C:\\Path\\ImportDirectory";
            // 
            // MassExportGroupBox
            // 
            this.MassExportGroupBox.Controls.Add(this.ExportSubsetRadioButton);
            this.MassExportGroupBox.Controls.Add(this.ExportAllRadioButton);
            this.MassExportGroupBox.Controls.Add(this.ExportDirectoryLabel);
            this.MassExportGroupBox.Controls.Add(this.ExportDirectoryTextBox);
            this.MassExportGroupBox.Controls.Add(this.ExportButton);
            this.MassExportGroupBox.Location = new System.Drawing.Point(17, 189);
            this.MassExportGroupBox.Name = "MassExportGroupBox";
            this.MassExportGroupBox.Size = new System.Drawing.Size(263, 162);
            this.MassExportGroupBox.TabIndex = 5;
            this.MassExportGroupBox.TabStop = false;
            this.MassExportGroupBox.Text = "Mass Export";
            // 
            // ExportDirectoryLabel
            // 
            this.ExportDirectoryLabel.AutoSize = true;
            this.ExportDirectoryLabel.Location = new System.Drawing.Point(6, 29);
            this.ExportDirectoryLabel.Name = "ExportDirectoryLabel";
            this.ExportDirectoryLabel.Size = new System.Drawing.Size(109, 17);
            this.ExportDirectoryLabel.TabIndex = 5;
            this.ExportDirectoryLabel.Text = "Export Directory";
            // 
            // ExportDirectoryTextBox
            // 
            this.ExportDirectoryTextBox.Location = new System.Drawing.Point(6, 49);
            this.ExportDirectoryTextBox.Name = "ExportDirectoryTextBox";
            this.ExportDirectoryTextBox.Size = new System.Drawing.Size(251, 22);
            this.ExportDirectoryTextBox.TabIndex = 4;
            this.ExportDirectoryTextBox.Text = "C:\\Path\\ExportDirectory";
            // 
            // ExportButton
            // 
            this.ExportButton.Location = new System.Drawing.Point(5, 104);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(251, 49);
            this.ExportButton.TabIndex = 3;
            this.ExportButton.Text = "Export";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // ExportAllRadioButton
            // 
            this.ExportAllRadioButton.AutoSize = true;
            this.ExportAllRadioButton.Checked = true;
            this.ExportAllRadioButton.Location = new System.Drawing.Point(8, 77);
            this.ExportAllRadioButton.Name = "ExportAllRadioButton";
            this.ExportAllRadioButton.Size = new System.Drawing.Size(44, 21);
            this.ExportAllRadioButton.TabIndex = 6;
            this.ExportAllRadioButton.TabStop = true;
            this.ExportAllRadioButton.Text = "All";
            this.ExportAllRadioButton.UseVisualStyleBackColor = true;
            // 
            // ExportSubsetRadioButton
            // 
            this.ExportSubsetRadioButton.AutoSize = true;
            this.ExportSubsetRadioButton.Location = new System.Drawing.Point(58, 77);
            this.ExportSubsetRadioButton.Name = "ExportSubsetRadioButton";
            this.ExportSubsetRadioButton.Size = new System.Drawing.Size(73, 21);
            this.ExportSubsetRadioButton.TabIndex = 7;
            this.ExportSubsetRadioButton.Text = "Subset";
            this.ExportSubsetRadioButton.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(977, 513);
            this.Controls.Add(this.MassExportGroupBox);
            this.Controls.Add(this.MassImportGroupBox);
            this.Controls.Add(this.UploadStatsButton);
            this.Controls.Add(this.LoggingTextBox);
            this.Controls.Add(this.FileDownloadButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Stats Download Test Harness";
            this.MassImportGroupBox.ResumeLayout(false);
            this.MassImportGroupBox.PerformLayout();
            this.MassExportGroupBox.ResumeLayout(false);
            this.MassExportGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button FileDownloadButton;
        private System.Windows.Forms.TextBox LoggingTextBox;
        private System.Windows.Forms.Button UploadStatsButton;
        private System.Windows.Forms.Button ImportButton;
        private System.Windows.Forms.GroupBox MassImportGroupBox;
        private System.Windows.Forms.Label ImportDirectoryLabel;
        private System.Windows.Forms.TextBox ImportDirectoryTextBox;
        private System.Windows.Forms.GroupBox MassExportGroupBox;
        private System.Windows.Forms.Label ExportDirectoryLabel;
        private System.Windows.Forms.TextBox ExportDirectoryTextBox;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.RadioButton ExportSubsetRadioButton;
        private System.Windows.Forms.RadioButton ExportAllRadioButton;
    }
}

