using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace NNKZipper
{
    public class CompressDialogForm : Form
    {
        private string targetPath;
        private ComboBox cmbLevel;
        private ComboBox cmbFormat;
        private Button btnStart;

        public CompressDialogForm(string path)
        {
            this.targetPath = path;
            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            this.Text = "NNKZipper — Quick Compress";
            this.Size = new Size(380, 220);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblInfo = new Label() { Text = $"Target: {Path.GetFileName(targetPath)}", Left = 20, Top = 15, Width = 320, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            this.Controls.Add(lblInfo);

            Label lblLvl = new Label() { Text = "Compression:", Left = 20, Top = 55, Width = 90 };
            cmbLevel = new ComboBox() { Left = 120, Top = 52, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbLevel.Items.AddRange(new string[] { "Fast", "Normal", "Highly Compressed (Maximum)" });
            cmbLevel.SelectedIndex = 2;
            this.Controls.Add(lblLvl);
            this.Controls.Add(cmbLevel);

            Label lblFmt = new Label() { Text = "Format:", Left = 20, Top = 95, Width = 90 };
            cmbFormat = new ComboBox() { Left = 120, Top = 92, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFormat.Items.AddRange(new string[] { ".nnk Archive", ".zip Archive", ".7z Archive" });
            cmbFormat.SelectedIndex = 0;
            this.Controls.Add(lblFmt);
            this.Controls.Add(cmbFormat);

            btnStart = new Button() { Text = "Compress Now", Left = 200, Top = 135, Width = 140, Height = 32, BackColor = Color.LightGreen };
            btnStart.Click += BtnStart_Click;
            this.Controls.Add(btnStart);
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            string enginePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7zr.exe");
            if (!File.Exists(enginePath))
            {
                MessageBox.Show("7zr.exe engine not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string mx = cmbLevel.SelectedIndex == 0 ? "-mx=1" : (cmbLevel.SelectedIndex == 1 ? "-mx=5" : "-mx=9");
            string ext = cmbFormat.SelectedIndex == 1 ? ".zip" : (cmbFormat.SelectedIndex == 2 ? ".7z" : ".nnk");
            string outputPath = targetPath + ext;

            string arguments = $"a {mx} \"{outputPath}\" \"{targetPath}\"";

            try
            {
                var psi = new ProcessStartInfo { FileName = enginePath, Arguments = arguments, CreateNoWindow = true, UseShellExecute = false };
                using (var p = Process.Start(psi))
                {
                    p.WaitForExit();
                    if (p.ExitCode == 0)
                    {
                        MessageBox.Show($"Archive created successfully:\n{outputPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Compression failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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