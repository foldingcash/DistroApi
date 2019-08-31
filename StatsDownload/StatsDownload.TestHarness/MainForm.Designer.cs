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
            this.ImportButton = new System.Windows.Forms.Button();
            this.MassImportGroupBox = new System.Windows.Forms.GroupBox();
            this.ImportDirectoryLabel = new System.Windows.Forms.Label();
            this.ImportDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.MassExportGroupBox = new System.Windows.Forms.GroupBox();
            this.ExportSubsetRadioButton = new System.Windows.Forms.RadioButton();
            this.ExportAllRadioButton = new System.Windows.Forms.RadioButton();
            this.ExportDirectoryLabel = new System.Windows.Forms.Label();
            this.ExportDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.ExportButton = new System.Windows.Forms.Button();
            this.EmailGroupBox = new System.Windows.Forms.GroupBox();
            this.TestEmailButton = new System.Windows.Forms.Button();
            this.StatsDownloadGroupBox = new System.Windows.Forms.GroupBox();
            this.MassImportGroupBox.SuspendLayout();
            this.MassExportGroupBox.SuspendLayout();
            this.EmailGroupBox.SuspendLayout();
            this.StatsDownloadGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // FileDownloadButton
            // 
            this.FileDownloadButton.Location = new System.Drawing.Point(4, 18);
            this.FileDownloadButton.Name = "FileDownloadButton";
            this.FileDownloadButton.Size = new System.Drawing.Size(188, 40);
            this.FileDownloadButton.TabIndex = 0;
            this.FileDownloadButton.Text = "File Download";
            this.FileDownloadButton.UseVisualStyleBackColor = true;
            this.FileDownloadButton.Click += new System.EventHandler(this.FileDownloadButton_Click);
            // 
            // LoggingTextBox
            // 
            this.LoggingTextBox.Location = new System.Drawing.Point(212, 11);
            this.LoggingTextBox.Multiline = true;
            this.LoggingTextBox.Name = "LoggingTextBox";
            this.LoggingTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LoggingTextBox.Size = new System.Drawing.Size(512, 436);
            this.LoggingTextBox.TabIndex = 1;
            // 
            // ImportButton
            // 
            this.ImportButton.Location = new System.Drawing.Point(4, 70);
            this.ImportButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(188, 40);
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
            this.MassImportGroupBox.Location = new System.Drawing.Point(9, 332);
            this.MassImportGroupBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MassImportGroupBox.Name = "MassImportGroupBox";
            this.MassImportGroupBox.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MassImportGroupBox.Size = new System.Drawing.Size(197, 115);
            this.MassImportGroupBox.TabIndex = 4;
            this.MassImportGroupBox.TabStop = false;
            this.MassImportGroupBox.Text = "Mass Import";
            // 
            // ImportDirectoryLabel
            // 
            this.ImportDirectoryLabel.AutoSize = true;
            this.ImportDirectoryLabel.Location = new System.Drawing.Point(4, 24);
            this.ImportDirectoryLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ImportDirectoryLabel.Name = "ImportDirectoryLabel";
            this.ImportDirectoryLabel.Size = new System.Drawing.Size(81, 13);
            this.ImportDirectoryLabel.TabIndex = 5;
            this.ImportDirectoryLabel.Text = "Import Directory";
            // 
            // ImportDirectoryTextBox
            // 
            this.ImportDirectoryTextBox.Location = new System.Drawing.Point(4, 40);
            this.ImportDirectoryTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ImportDirectoryTextBox.Name = "ImportDirectoryTextBox";
            this.ImportDirectoryTextBox.Size = new System.Drawing.Size(189, 20);
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
            this.MassExportGroupBox.Location = new System.Drawing.Point(9, 195);
            this.MassExportGroupBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MassExportGroupBox.Name = "MassExportGroupBox";
            this.MassExportGroupBox.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MassExportGroupBox.Size = new System.Drawing.Size(197, 132);
            this.MassExportGroupBox.TabIndex = 5;
            this.MassExportGroupBox.TabStop = false;
            this.MassExportGroupBox.Text = "Mass Export";
            // 
            // ExportSubsetRadioButton
            // 
            this.ExportSubsetRadioButton.AutoSize = true;
            this.ExportSubsetRadioButton.Location = new System.Drawing.Point(44, 63);
            this.ExportSubsetRadioButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ExportSubsetRadioButton.Name = "ExportSubsetRadioButton";
            this.ExportSubsetRadioButton.Size = new System.Drawing.Size(58, 17);
            this.ExportSubsetRadioButton.TabIndex = 7;
            this.ExportSubsetRadioButton.Text = "Subset";
            this.ExportSubsetRadioButton.UseVisualStyleBackColor = true;
            // 
            // ExportAllRadioButton
            // 
            this.ExportAllRadioButton.AutoSize = true;
            this.ExportAllRadioButton.Checked = true;
            this.ExportAllRadioButton.Location = new System.Drawing.Point(6, 63);
            this.ExportAllRadioButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ExportAllRadioButton.Name = "ExportAllRadioButton";
            this.ExportAllRadioButton.Size = new System.Drawing.Size(36, 17);
            this.ExportAllRadioButton.TabIndex = 6;
            this.ExportAllRadioButton.TabStop = true;
            this.ExportAllRadioButton.Text = "All";
            this.ExportAllRadioButton.UseVisualStyleBackColor = true;
            // 
            // ExportDirectoryLabel
            // 
            this.ExportDirectoryLabel.AutoSize = true;
            this.ExportDirectoryLabel.Location = new System.Drawing.Point(4, 24);
            this.ExportDirectoryLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ExportDirectoryLabel.Name = "ExportDirectoryLabel";
            this.ExportDirectoryLabel.Size = new System.Drawing.Size(82, 13);
            this.ExportDirectoryLabel.TabIndex = 5;
            this.ExportDirectoryLabel.Text = "Export Directory";
            // 
            // ExportDirectoryTextBox
            // 
            this.ExportDirectoryTextBox.Location = new System.Drawing.Point(4, 40);
            this.ExportDirectoryTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ExportDirectoryTextBox.Name = "ExportDirectoryTextBox";
            this.ExportDirectoryTextBox.Size = new System.Drawing.Size(189, 20);
            this.ExportDirectoryTextBox.TabIndex = 4;
            this.ExportDirectoryTextBox.Text = "C:\\Path\\ExportDirectory";
            // 
            // ExportButton
            // 
            this.ExportButton.Location = new System.Drawing.Point(4, 84);
            this.ExportButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(188, 40);
            this.ExportButton.TabIndex = 3;
            this.ExportButton.Text = "Export";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // EmailGroupBox
            // 
            this.EmailGroupBox.Controls.Add(this.TestEmailButton);
            this.EmailGroupBox.Location = new System.Drawing.Point(9, 79);
            this.EmailGroupBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.EmailGroupBox.Name = "EmailGroupBox";
            this.EmailGroupBox.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.EmailGroupBox.Size = new System.Drawing.Size(197, 64);
            this.EmailGroupBox.TabIndex = 6;
            this.EmailGroupBox.TabStop = false;
            this.EmailGroupBox.Text = "Email";
            // 
            // TestEmailButton
            // 
            this.TestEmailButton.Location = new System.Drawing.Point(4, 17);
            this.TestEmailButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TestEmailButton.Name = "TestEmailButton";
            this.TestEmailButton.Size = new System.Drawing.Size(188, 40);
            this.TestEmailButton.TabIndex = 0;
            this.TestEmailButton.Text = "Test Email";
            this.TestEmailButton.UseVisualStyleBackColor = true;
            this.TestEmailButton.Click += new System.EventHandler(this.TestEmailButton_Click);
            // 
            // StatsDownloadGroupBox
            // 
            this.StatsDownloadGroupBox.Controls.Add(this.FileDownloadButton);
            this.StatsDownloadGroupBox.Location = new System.Drawing.Point(9, 10);
            this.StatsDownloadGroupBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.StatsDownloadGroupBox.Name = "StatsDownloadGroupBox";
            this.StatsDownloadGroupBox.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.StatsDownloadGroupBox.Size = new System.Drawing.Size(197, 65);
            this.StatsDownloadGroupBox.TabIndex = 7;
            this.StatsDownloadGroupBox.TabStop = false;
            this.StatsDownloadGroupBox.Text = "Stats Download";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 455);
            this.Controls.Add(this.StatsDownloadGroupBox);
            this.Controls.Add(this.EmailGroupBox);
            this.Controls.Add(this.MassExportGroupBox);
            this.Controls.Add(this.MassImportGroupBox);
            this.Controls.Add(this.LoggingTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Stats Download Test Harness";
            this.MassImportGroupBox.ResumeLayout(false);
            this.MassImportGroupBox.PerformLayout();
            this.MassExportGroupBox.ResumeLayout(false);
            this.MassExportGroupBox.PerformLayout();
            this.EmailGroupBox.ResumeLayout(false);
            this.StatsDownloadGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button FileDownloadButton;
        private System.Windows.Forms.TextBox LoggingTextBox;
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
        private System.Windows.Forms.GroupBox EmailGroupBox;
        private System.Windows.Forms.Button TestEmailButton;
        private System.Windows.Forms.GroupBox StatsDownloadGroupBox;
    }
}

