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
            this.SuspendLayout();
            // 
            // ExportFilesCheckedListBox
            // 
            this.ExportFilesCheckedListBox.FormattingEnabled = true;
            this.ExportFilesCheckedListBox.Location = new System.Drawing.Point(12, 12);
            this.ExportFilesCheckedListBox.Name = "ExportFilesCheckedListBox";
            this.ExportFilesCheckedListBox.Size = new System.Drawing.Size(315, 463);
            this.ExportFilesCheckedListBox.TabIndex = 0;
            // 
            // SelectExportFilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 486);
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
    }
}