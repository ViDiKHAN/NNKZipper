using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace NNKZipper
{
    public class ArchiveBrowserForm : Form
    {
        private string archivePath;
        private ListBox lstFiles;
        private Button btnAddFiles;
        private Button btnAddFolder;
        private Button btnDelete;
        private ComboBox cmbConvertFormat;
        private Button btnConvert;

        public ArchiveBrowserForm(string path)
        {
            this.archivePath = path;
            InitializeComponentCustom();
            LoadArchiveContents();
        }

        private void InitializeComponentCustom()
        {
            this.Text = $"NNKZipper Manager — {Path.GetFileName(archivePath)}";
            this.Size = new Size(620, 460);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblFiles = new Label() { Text = "Archive Contents:", Left = 20, Top = 15, Width = 200 };
            this.Controls.Add(lblFiles);

            lstFiles = new ListBox() { Left = 20, Top = 40, Width = 430, Height = 300 };
            this.Controls.Add(lstFiles);

            btnAddFiles = new Button() { Text = "Add Files", Left = 465, Top = 40, Width = 120, Height = 35 };
            btnAddFiles.Click += BtnAddFiles_Click;
            this.Controls.Add(btnAddFiles);

            btnAddFolder = new Button() { Text = "Add Folder", Left = 465, Top = 85, Width = 120, Height = 35 };
            btnAddFolder.Click += BtnAddFolder_Click;
            this.Controls.Add(btnAddFolder);

            btnDelete = new Button() { Text = "Delete Selected", Left = 465, Top = 130, Width = 120, Height = 35, BackColor = Color.MistyRose };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            Label lblConv = new Label() { Text = "Convert Archive to:", Left = 20, Top = 365, Width = 120 };
            cmbConvertFormat = new ComboBox() { Left = 140, Top = 362, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbConvertFormat.Items.AddRange(new string[] { ".zip", ".7z", ".tar", ".rar" });
            cmbConvertFormat.SelectedIndex = 0;
            this.Controls.Add(lblConv);
            this.Controls.Add(cmbConvertFormat);

            btnConvert = new Button() { Text = "Convert Format", Left = 300, Top = 360, Width = 150, Height = 30, BackColor = Color.LightCyan };
            btnConvert.Click += BtnConvert_Click;
            this.Controls.Add(btnConvert);
        }

        private void LoadArchiveContents()
        {
            lstFiles.Items.Clear();
            string enginePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7zr.exe");
            if (!File.Exists(enginePath)) return;

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = enginePath,
                    Arguments = $"l \"{archivePath}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };

                using (var p = Process.Start(psi))
                {
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    bool readingFiles = false;
                    foreach (string line in output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (line.StartsWith("-------------------"))
                        {
                            readingFiles = !readingFiles;
                            continue;
                        }
                        if (readingFiles && line.Length > 53)
                        {
                            string fileName = line.Substring(53).Trim();
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                lstFiles.Items.Add(fileName);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void BtnAddFiles_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog() { Multiselect = true })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string enginePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7zr.exe");
                    foreach (string file in ofd.FileNames)
                    {
                        Process.Start(enginePath, $"a \"{archivePath}\" \"{file}\"")?.WaitForExit();
                    }
                    LoadArchiveContents();
                    MessageBox.Show("Files added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnAddFolder_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string enginePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7zr.exe");
                    Process.Start(enginePath, $"a \"{archivePath}\" \"{fbd.SelectedPath}\\*\"")?.WaitForExit();
                    LoadArchiveContents();
                    MessageBox.Show("Folder added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItem == null) return;
            string fileToDelete = lstFiles.SelectedItem.ToString();

            if (MessageBox.Show($"Are you sure you want to delete '{fileToDelete}' from the archive?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                string enginePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7zr.exe");
                Process.Start(enginePath, $"d \"{archivePath}\" \"{fileToDelete}\"")?.WaitForExit();
                LoadArchiveContents();
            }
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {
            string ext = cmbConvertFormat.SelectedItem.ToString();
            string newPath = Path.ChangeExtension(archivePath, ext);

            string enginePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7zr.exe");
            string tempDir = Path.Combine(Path.GetTempPath(), "NNKConv_" + Guid.NewGuid());

            try
            {
                Directory.CreateDirectory(tempDir);
                Process.Start(enginePath, $"x \"{archivePath}\" -o\"{tempDir}\" -y")?.WaitForExit();
                Process.Start(enginePath, $"a \"{newPath}\" \"{tempDir}\\*\"")?.WaitForExit();

                MessageBox.Show($"Successfully converted archive to:\n{newPath}", "Conversion Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Conversion error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            }
        }
    }
}