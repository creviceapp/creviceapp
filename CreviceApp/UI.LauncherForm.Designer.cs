namespace Crevice.UI
{
    partial class LauncherForm
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
            if (disposing)
            {
                components?.Dispose();
                tooltip1?.Dispose();
                MainForm?.Dispose();
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
            this.components = new System.ComponentModel.Container();
            this.tooltip1 = new TooltipNotifier(this);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = Properties.Resources.CreviceIcon;
            this.notifyIcon1.Text = "Crevice is loading";
            this.notifyIcon1.BalloonTipClicked += this.NotifyIcon1_BalloonTipClick;
            this.notifyIcon1.MouseUp += this.NotifyIcon1_MouseUp;
            this.notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            //
            // contextMenu1
            //
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenu1_Opening);
            // 
            // LauncherForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "LauncherForm";
            this.Text = "Crevice 4";
            this.Icon = Properties.Resources.CreviceIcon;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Opacity = 0;
            this.ResumeLayout(false);
        }
        #endregion

        private TooltipNotifier tooltip1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}