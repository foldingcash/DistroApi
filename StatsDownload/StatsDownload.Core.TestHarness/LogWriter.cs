namespace StatsDownload.Core.TestHarness
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    public class LogWriter : TextWriter
    {
        private readonly TextBox control;

        public LogWriter(TextBox control)
        {
            this.control = control;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            CallSafely(value.ToString());
        }

        public override void Write(string value)
        {
            CallSafely(value);
        }

        private void CallSafely(string value)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action(() => { CallSafely(value); }));
            }
            else
            {
                control.AppendText(value);
                control.SelectionStart = control.TextLength;
                control.ScrollToCaret();
            }
        }
    }
}