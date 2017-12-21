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
            this.FileDownloadButton = new System.Windows.Forms.Button();
            this.LoggingTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // FileDownloadButton
            // 
            this.FileDownloadButton.Location = new System.Drawing.Point(12, 12);
            this.FileDownloadButton.Name = "FileDownloadButton";
            this.FileDownloadButton.Size = new System.Drawing.Size(197, 40);
            this.FileDownloadButton.TabIndex = 0;
            this.FileDownloadButton.Text = "Download Stats File";
            this.FileDownloadButton.UseVisualStyleBackColor = true;
            this.FileDownloadButton.Click += new System.EventHandler(this.FileDownloadButton_Click);
            // 
            // LoggingTextBox
            // 
            this.LoggingTextBox.Location = new System.Drawing.Point(215, 12);
            this.LoggingTextBox.Multiline = true;
            this.LoggingTextBox.Name = "LoggingTextBox";
            this.LoggingTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LoggingTextBox.Size = new System.Drawing.Size(506, 393);
            this.LoggingTextBox.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 417);
            this.Controls.Add(this.LoggingTextBox);
            this.Controls.Add(this.FileDownloadButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Stats Download Test Harness";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button FileDownloadButton;
        private System.Windows.Forms.TextBox LoggingTextBox;
    }
}

