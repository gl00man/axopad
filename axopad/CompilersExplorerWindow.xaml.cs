using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using Microsoft.Win32;

namespace axopad
{
    public partial class CompilersExplorerWindow : Window
    {
        Dictionary<string, string> paths = new Dictionary<string, string>();

        public CompilersExplorerWindow()
        {
            InitializeComponent();

            String[] data = ReadCompilers();
            paths.Add(data[0], data[1]);
            paths.Add(data[2], data[3]);
            paths.Add(data[4], data[5]);
            paths.Add(data[6], data[7]);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveCompilers();
        }

        private void langCmb_DropDownClosed(object sender, EventArgs e)
        {
            Debug.Write(langCmb.Text);
            pathTxt.Text = paths[langCmb.Text];
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            if (langCmb.Text != "")
            {
                paths[langCmb.Text] = BrowseCompilers();
                pathTxt.Text = paths[langCmb.Text];
            }
        }

        private string BrowseCompilers()
        {
            string filePath = "";
            OpenFileDialog fDialog = new OpenFileDialog();
            fDialog.Multiselect = false;
            fDialog.Filter = "All files (*.*)|*.*";
            Nullable<bool> dialogOK = fDialog.ShowDialog();

            if (dialogOK == true)
            {
                foreach (string x in fDialog.FileNames)
                {
                    filePath += x;
                }
            }
            return filePath;
        }

        private void SaveCompilers()
        {
            File.WriteAllText(GetCompilerPath(), String.Empty);
            using (StreamWriter sw = new StreamWriter(GetCompilerPath()))
            {
                sw.Write("C#" + '|' + paths["C#"] + '|' + "C++" + '|' + paths["C++"] + '|' + "Python" + '|' + paths["Python"] + '|' + "Rust" + '|' + paths["Rust"]);
            }
        }

        private String[] ReadCompilers()
        {
            using (StreamReader sr = new StreamReader(GetCompilerPath(), true))
            {
                string line;
                String[] parameters = { };

                while ((line = sr.ReadLine()) != null)
                {
                    parameters = line.Split('|');
                }
                return parameters;
            }
        }

        private string GetCompilerPath()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("axopad.dll", @"Assets\Data\compilers.txt");
        }
    }
}
