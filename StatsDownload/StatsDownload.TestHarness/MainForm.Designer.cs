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
            this.ImportDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.ImportDirectoryLabel = new System.Windows.Forms.Label();
            this.MassImportGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // FileDownloadButton
            // 
            this.FileDownloadButton.Location = new System.Drawing.Point(16, 15);
            this.FileDownloadButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.LoggingTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.LoggingTextBox.Multiline = true;
            this.LoggingTextBox.Name = "LoggingTextBox";
            this.LoggingTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LoggingTextBox.Size = new System.Drawing.Size(673, 483);
            this.LoggingTextBox.TabIndex = 1;
            // 
            // UploadStatsButton
            // 
            this.UploadStatsButton.Location = new System.Drawing.Point(16, 71);
            this.UploadStatsButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            // ImportDirectoryTextBox
            // 
            this.ImportDirectoryTextBox.Location = new System.Drawing.Point(6, 49);
            this.ImportDirectoryTextBox.Name = "ImportDirectoryTextBox";
            this.ImportDirectoryTextBox.Size = new System.Drawing.Size(251, 22);
            this.ImportDirectoryTextBox.TabIndex = 4;
            this.ImportDirectoryTextBox.Text = "C:\\Path\\ImportDirectory";
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(977, 513);
            this.Controls.Add(this.MassImportGroupBox);
            this.Controls.Add(this.UploadStatsButton);
            this.Controls.Add(this.LoggingTextBox);
            this.Controls.Add(this.FileDownloadButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Stats Download Test Harness";
            this.MassImportGroupBox.ResumeLayout(false);
            this.MassImportGroupBox.PerformLayout();
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
    }
}

