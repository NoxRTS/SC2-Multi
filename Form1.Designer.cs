namespace SC2_Multi
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            btnCloseHandles = new Button();
            txtLog = new TextBox();
            SuspendLayout();
            // 
            // btnCloseHandles
            // 
            btnCloseHandles.BackColor = Color.FromArgb(0, 120, 215);
            btnCloseHandles.Cursor = Cursors.Hand;
            btnCloseHandles.Dock = DockStyle.Top;
            btnCloseHandles.FlatAppearance.BorderSize = 0;
            btnCloseHandles.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 150, 255);
            btnCloseHandles.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 90, 170);
            btnCloseHandles.FlatStyle = FlatStyle.Flat;
            btnCloseHandles.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnCloseHandles.ForeColor = Color.White;
            btnCloseHandles.Height = 52;
            btnCloseHandles.Text = "⚡  Close SC2 Handles";
            btnCloseHandles.Click += btnCloseHandles_Click;
            // 
            // txtLog
            // 
            txtLog.Dock = DockStyle.Fill;
            txtLog.Font = new Font("Consolas", 9.75F);
            txtLog.Multiline = true;
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            // 
            // lblAbout
            // 
            lblAbout = new Label();
            lblAbout.Dock = DockStyle.Bottom;
            lblAbout.Font = new Font("Segoe UI", 8.25F);
            lblAbout.ForeColor = Color.Gray;
            lblAbout.Height = 24;
            lblAbout.Text = "Created by Nox";
            lblAbout.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lnkGitHub
            // 
            lnkGitHub = new LinkLabel();
            lnkGitHub.Dock = DockStyle.Bottom;
            lnkGitHub.Font = new Font("Segoe UI", 8.25F);
            lnkGitHub.Height = 20;
            lnkGitHub.Text = "github.com/NoxRTS";
            lnkGitHub.TextAlign = ContentAlignment.MiddleCenter;
            lnkGitHub.LinkColor = Color.FromArgb(0, 120, 215);
            lnkGitHub.LinkClicked += lnkGitHub_LinkClicked;
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(620, 300);
            Controls.Add(txtLog);
            Controls.Add(lnkGitHub);
            Controls.Add(lblAbout);
            Controls.Add(btnCloseHandles);
            Text = "SC2 Multi-Instance";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCloseHandles;
        private TextBox txtLog;
        private Label lblAbout;
        private LinkLabel lnkGitHub;
    }
}
