namespace StatsDownload.TestHarness
{
    partial class SelectExportFilesForm
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
            this.ExportFilesCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.FinishedButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ExportFilesCheckedListBox
            // 
            this.ExportFilesCheckedListBox.FormattingEnabled = true;
            this.ExportFilesCheckedListBox.Location = new System.Drawing.Point(122, 12);
            this.ExportFilesCheckedListBox.Name = "ExportFilesCheckedListBox";
            this.ExportFilesCheckedListBox.Size = new System.Drawing.Size(401, 463);
            this.ExportFilesCheckedListBox.Sorted = true;
            this.ExportFilesCheckedListBox.TabIndex = 0;
            // 
            // FinishedButton
            // 
            this.FinishedButton.Location = new System.Drawing.Point(12, 193);
            this.FinishedButton.Name = "FinishedButton";
            this.FinishedButton.Size = new System.Drawing.Size(104, 41);
            this.FinishedButton.TabIndex = 1;
            this.FinishedButton.Text = "Finished";
            this.FinishedButton.UseVisualStyleBackColor = true;
            this.FinishedButton.Click += new System.EventHandler(this.FinishedButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(12, 240);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(104, 41);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // SelectExportFilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 486);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.FinishedButton);
            this.Controls.Add(this.ExportFilesCheckedListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectExportFilesForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Select Files to Export";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.CheckedListBox ExportFilesCheckedListBox;
        private System.Windows.Forms.Button FinishedButton;
        private System.Windows.Forms.Button CancelButton;
    }
}