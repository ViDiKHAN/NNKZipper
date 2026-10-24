using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace NNKZipper
{
    public partial class MainForm : Form
    {
        private TextBox txtInput;
        private TextBox txtOutput;
        private ComboBox cmbCompression;
        private TextBox txtPassword;
        private Button btnBrowseInput;
        private Button btnBrowseOutput;
        private Button btnStart;
        private ProgressBar progressBar;
        private Label lblStatus;
        private RadioButton rbCompress;
        private RadioButton rbExtract;

        public MainForm()
        {
            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            this.Text = "NNKZipper — Custom .nnk File Archive Utility";
            this.Size = new Size(600, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Mode Selection (Compress / Extract)
            rbCompress = new RadioButton() { Text = "Compress to .nnk", Left = 20, Top = 20, Width = 140, Checked = true };
            rbExtract = new RadioButton() { Text = "Extract .nnk Archive", Left = 170, Top = 20, Width = 160 };
            rbCompress.CheckedChanged += ModeChanged;
            this.Controls.Add(rbCompress);
            this.Controls.Add(rbExtract);

            // Input File/Folder
            Label lblInput = new Label() { Text = "Source Path:", Left = 20, Top = 60, Width = 100 };
            txtInput = new TextBox() { Left = 120, Top = 57, Width = 340 };
            btnBrowseInput = new Button() { Text = "Browse...", Left = 470, Top = 55, Width = 90 };
            btnBrowseInput.Click += BtnBrowseInput_Click;
            this.Controls.Add(lblInput);
            this.Controls.Add(txtInput);
            this.Controls.Add(btnBrowseInput);

            // Output File/Folder
            Label lblOutput = new Label() { Text = "Destination:", Left = 20, Top = 105, Width = 100 };
            txtOutput = new TextBox() { Left = 120, Top = 102, Width = 340 };
            btnBrowseOutput = new Button() { Text = "Browse...", Left = 470, Top = 100, Width = 90 };
            btnBrowseOutput.Click += BtnBrowseOutput_Click;
            this.Controls.Add(lblOutput);
            this.Controls.Add(txtOutput);
            this.Controls.Add(btnBrowseOutput);

            // Compression Level
            Label lblComp = new Label() { Text = "Compression:", Left = 20, Top = 150, Width = 100 };
            cmbCompression = new ComboBox() { Left = 120, Top = 147, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCompression.Items.AddRange(new string[] { "Store (No compression)", "Fast", "Normal", "Maximum" });
            cmbCompression.SelectedIndex = 3; // Default to Maximum
            this.Controls.Add(lblComp);
            this.Controls.Add(cmbCompression);

            // Password Protection
            Label lblPass = new Label() { Text = "Password (Opt):", Left = 20, Top = 195, Width = 100 };
            txtPassword = new TextBox() { Left = 120, Top = 192, Width = 200, UseSystemPasswordChar = true };
            this.Controls.Add(lblPass);
            this.Controls.Add(txtPassword);

            // Progress Bar
            progressBar = new ProgressBar() { Left = 20, Top = 250, Width = 540, Height = 25 };
            this.Controls.Add(progressBar);

            // Status Label
            lblStatus = new Label() { Text = "Ready. Place 7zr.exe in the application folder.", Left = 20, Top = 285, Width = 540 };
            this.Controls.Add(lblStatus);

            // Start Button
            btnStart = new Button() { Text = "Start Operation", Left = 390, Top = 325, Width = 170, Height = 40, BackColor = Color.LightGreen };
            btnStart.Click += BtnStart_Click;
            this.Controls.Add(btnStart);
        }

        private void ModeChanged(object sender, EventArgs e)
        {
            if (rbCompress.Checked)
            {
                btnStart.Text = "Compress to .nnk";
                cmbCompression.Enabled = true;
            }
            else
            {
                btnStart.Text = "Extract .nnk";
                cmbCompression.Enabled = false;
            }
        }

        private void BtnBrowseInput_Click(object sender, EventArgs e)
        {
            if (rbCompress.Checked)
            {
                // Can be file or folder
                using (var fbd = new FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        txtInput.Text = fbd.SelectedPath;
                        txtOutput.Text = fbd.SelectedPath + ".nnk";
                    }
                }
            }
            else
            {
                using (var ofd = new OpenFileDialog() { Filter = "NNK Archives (*.nnk)|*.nnk|All Files (*.*)|*.*" })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        txtInput.Text = ofd.FileName;
                        txtOutput.Text = Path.GetDirectoryName(ofd.FileName);
                    }
                }
            }
        }

        private void BtnBrowseOutput_Click(object sender, EventArgs e)
        {
            if (rbCompress.Checked)
            {
                using (var sfd = new SaveFileDialog() { Filter = "NNK Archive (*.nnk)|*.nnk", FileName = "Backup.nnk" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        txtOutput.Text = sfd.FileName;
                    }
                }
            }
            else
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        txtOutput.Text = fbd.SelectedPath;
                    }
                }
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            string input = txtInput.Text.Trim();
            string output = txtOutput.Text.Trim();

            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(output))
            {
                MessageBox.Show("Please specify both input and output paths.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string enginePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7zr.exe");
            if (!File.Exists(enginePath))
            {
                MessageBox.Show("7zr.exe engine not found in the application directory!\nPlease download 7zr.exe and place it alongside NNKZipper.exe.", "Missing Engine", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string arguments = "";
            if (rbCompress.Checked)
            {
                string level = "-mx=9";
                if (cmbCompression.SelectedIndex == 0) level = "-mx=0";
                else if (cmbCompression.SelectedIndex == 1) level = "-mx=1";
                else if (cmbCompression.SelectedIndex == 2) level = "-mx=5";

                string passwordArg = "";
                if (!string.IsNullOrEmpty(txtPassword.Text))
                {
                    passwordArg = $" -p\"{txtPassword.Text}\" -mhe=on";
                }

                arguments = $"a -t7z {level}{passwordArg} \"{output}\" \"{input}\"";
            }
            else
            {
                string passwordArg = "";
                if (!string.IsNullOrEmpty(txtPassword.Text))
                {
                    passwordArg = $" -p\"{txtPassword.Text}\"";  // <-- Fixed!
                }

                arguments = $"x \"{input}\" -o\"{output}\"{passwordArg} -y";
            }

            try
            {
                btnStart.Enabled = false;
                progressBar.Style = ProgressBarStyle.Marquee;
                lblStatus.Text = "Running 7-Zip engine...";

                var psi = new ProcessStartInfo
                {
                    FileName = enginePath,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = false, // Disable redirection to prevent buffer deadlock
                    RedirectStandardError = false   // Disable redirection to prevent buffer deadlock
                };

                using (var process = Process.Start(psi))
                {
                    process.WaitForExit();
                    if (process.ExitCode == 0)
                    {
                        lblStatus.Text = "Operation completed successfully!";
                        MessageBox.Show("Archive operation completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        lblStatus.Text = "Operation failed.";
                        MessageBox.Show($"Operation failed with exit code: {process.ExitCode}", "Execution Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar.Style = ProgressBarStyle.Blocks;
                btnStart.Enabled = true;
            }
        }
    }
}