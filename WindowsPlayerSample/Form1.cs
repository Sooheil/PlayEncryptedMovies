using System;
using System.Net.Http;
using System.Windows.Forms;

namespace WindowsPlayerSample
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

        private void OpenFileButton_Click(object sender, EventArgs e)
        {
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "All Movie Type|*.mp4|*.mov|*.wmv|*.avi|*.mkv";
			if (openFileDialog.ShowDialog() == DialogResult.OK) 
			{
				PathTextBox.Text = openFileDialog.FileName;
				if (System.IO.File.Exists(PathTextBox.Text))
				{
					HttpClient client = new HttpClient();
					axWindowsMediaPlayer1.URL = WindowsPlayerSample.Program.BaseAddress + $"api/media/play?file={PathTextBox.Text}";
					MessageBox.Show("Your file is ready. For watching it, press play button.", "information", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else 
				{
                    MessageBox.Show("The file does not exist.","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
			}

			
		}
    }
}
