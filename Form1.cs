namespace SC2_Multi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Text = $"SC2 Multi-Instance v{version!.Major}.{version.Minor}.{version.Build}";
        }

        private async void btnCloseHandles_Click(object sender, EventArgs e)
        {
            btnCloseHandles.Enabled = false;
            txtLog.Clear();
            try
            {
                var results = await HandleCloser.CloseHandlesAsync();
                txtLog.Text = string.Join(Environment.NewLine, results);
            }
            catch (Exception ex)
            {
                txtLog.Text = $"Error: {ex.Message}";
            }
            finally
            {
                btnCloseHandles.Enabled = true;
            }
        }

        private void lnkGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://github.com/NoxRTS",
                UseShellExecute = true
            });
        }
    }
}
