namespace StatsDownload.Core.TestHarness
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectExportFilesForm));
            this.ExportFilesCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.FinishedButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ExportFilesCheckedListBox
            // 
            this.ExportFilesCheckedListBox.FormattingEnabled = true;
            this.ExportFilesCheckedListBox.Location = new System.Drawing.Point(107, 11);
            this.ExportFilesCheckedListBox.Name = "ExportFilesCheckedListBox";
            this.ExportFilesCheckedListBox.Size = new System.Drawing.Size(351, 418);
            this.ExportFilesCheckedListBox.TabIndex = 0;
            // 
            // FinishedButton
            // 
            this.FinishedButton.Location = new System.Drawing.Point(10, 181);
            this.FinishedButton.Name = "FinishedButton";
            this.FinishedButton.Size = new System.Drawing.Size(91, 38);
            this.FinishedButton.TabIndex = 1;
            this.FinishedButton.Text = "Finished";
            this.FinishedButton.UseVisualStyleBackColor = true;
            this.FinishedButton.Click += new System.EventHandler(this.FinishedButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(10, 225);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(91, 38);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // SelectExportFilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 456);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.FinishedButton);
            this.Controls.Add(this.ExportFilesCheckedListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectExportFilesForm";
            this.ShowInTaskbar = false;
            this.Text = "Select Files to Export";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button FinishedButton;
        private new System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.CheckedListBox ExportFilesCheckedListBox;
    }
}