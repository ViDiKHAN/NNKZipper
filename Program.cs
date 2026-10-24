using System;
using System.IO;
using System.Windows.Forms;

namespace NNKZipper
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Handle Context Menu Compression Action: NNKZipper.exe -compress "C:\TargetFolder"
            if (args.Length >= 2 && args[0].Equals("-compress", StringComparison.OrdinalIgnoreCase))
            {
                string targetPath = args[1];
                Application.Run(new CompressDialogForm(targetPath));
                return;
            }

            // Handle opening a .nnk file (Handler Popup or Archive Browser)
            if (args.Length > 0 && File.Exists(args[0]))
            {
                string filePath = args[0];
                if (Path.GetExtension(filePath).Equals(".nnk", StringComparison.OrdinalIgnoreCase))
                {
                    Application.Run(new ActionForm(filePath));
                    return;
                }
            }

            // Default: Open main compressor UI
            Application.Run(new MainForm());
        }
    }
}