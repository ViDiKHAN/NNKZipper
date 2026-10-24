using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace NNKZipper
{
    public class ActionForm : Form
    {
        private string archivePath;
        private Label lblTitle;
        private Button btnExtractHere;
        private Button btnExtractFolder;
        private Button btnExtractTo;
        private Button btnOpenBrowser;

        public ActionForm(string path)
        {
            this.archivePath = path;
            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            this.Text = "NNKZipper Archive Handler";
            this.Size = new Size(420, 310);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            lblTitle = new Label()
            {
                Text = $"Archive: {Path.GetFileName(archivePath)}",
                Left = 20,
                Top = 20,
                Width = 360,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            this.Controls.Add(lblTitle);

            btnExtractHere = CreateActionButton("1. Extract Here", 60);
            btnExtractHere.Click += (s, e) => ExecuteExtraction(Path.GetDirectoryName(archivePath));

            string folderName = Path.GetFileNameWithoutExtension(archivePath);
            btnExtractFolder = CreateActionButton($"2. Extract in '{folderName}'", 110);
            btnExtractFolder.Click += (s, e) =>
            {
                string targetDir = Path.Combine(Path.GetDirectoryName(archivePath), folderName);
                Directory.CreateDirectory(targetDir);
                ExecuteExtraction(targetDir);
            };

            btnExtractTo = CreateActionButton("3. Extract to... (Browse)", 160);
            btnExtractTo.Click += (s, e) =>
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        ExecuteExtraction(fbd.SelectedPath);
                    }
                }
            };

            btnOpenBrowser = CreateActionButton("4. Open in Archive Manager (Manage/Convert)", 210);
            btnOpenBrowser.Click += (s, e) =>
            {
                this.Hide();
                var browser = new ArchiveBrowserForm(archivePath);
                browser.ShowDialog();
                this.Close();
            };
        }

        private Button CreateActionButton(string text, int top)
        {
            var btn = new Button()
            {
                Text = text,
                Left = 20,
                Top = top,
                Width = 360,
                Height = 38,
                TextAlign = ContentAlignment.MiddleLeft,
                UseVisualStyleBackColor = true,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };
            this.Controls.Add(btn);
            return btn;
        }

        private void ExecuteExtraction(string destinationDir)
        {
            string enginePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7zr.exe");
            Directory.CreateDirectory(destinationDir);
            string arguments = $"x \"{archivePath}\" -o\"{destinationDir}\" -y";

            try
            {
                var psi = new ProcessStartInfo { FileName = enginePath, Arguments = arguments, CreateNoWindow = true, UseShellExecute = false };
                using (var process = Process.Start(psi))
                {
                    process.WaitForExit();
                    if (process.ExitCode == 0)
                    {
                        MessageBox.Show($"Successfully extracted to:\n{destinationDir}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Extraction failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}