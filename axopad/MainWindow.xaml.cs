using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace axopad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filePath = "";
        public MainWindow()
        {
            InitializeComponent();
        }

        /*
         * BUTTONS EVENTS
         */

        private void newFileBtn_Click(object sender, RoutedEventArgs e)
        {
            filePath = "";
            mainTxt.Document.Blocks.Clear();
        }

        private void openFileBtn_Click(object sender, RoutedEventArgs e)
        {
            filePath = GetFilePath();
            OpenFile(filePath);
        }

        private void saveFileBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFile(false, filePath);
        }

        private void saveAsBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFile(true, GetToSavePath());
        }

        private void findPhraseBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void helpBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        /*
         * SAVING AND READING DATA
         */

        private void SaveFile(bool saveAs, string path)
        {
            if(saveAs)
            {
                TextRange range;
                FileStream fStream;

                range = new TextRange(mainTxt.Document.ContentStart, mainTxt.Document.ContentEnd);

                fStream = new FileStream(path, FileMode.Create);
                range.Save(fStream, DataFormats.Text);
                fStream.Close();
            }
            else if(filePath != "" && !saveAs)
            {
                TextRange range;
                FileStream fStream;

                range = new TextRange(mainTxt.Document.ContentStart, mainTxt.Document.ContentEnd);

                fStream = new FileStream(path, FileMode.Create);
                range.Save(fStream, DataFormats.Text);
                fStream.Close();
            }
        }

        private void OpenFile(string path)
        {
            TextRange range;

            FileStream fStream;

            if (File.Exists(path))
            {

                range = new TextRange(mainTxt.Document.ContentStart, mainTxt.Document.ContentEnd);

                fStream = new FileStream(path, FileMode.OpenOrCreate);
                range.Load(fStream, DataFormats.Text);
                fStream.Close();
            }
            else
            {
                MessageBox.Show("Can't find file!");
            }
        }
        
        /*
         * GETTING FILES PATH
         */

        private string GetFilePath()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            fileDialog.DefaultExt = ".txt";
            Nullable<bool> dialogOK = fileDialog.ShowDialog();

            if (dialogOK == true)
            {
                string filePath = "";
                foreach (string x in fileDialog.FileNames)
                {
                    filePath += x;
                }
                return filePath;
            }
            return null;
        }

        private string GetToSavePath()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "txt";
            sfd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            string sfdname = sfd.FileName;
            if (sfd.ShowDialog() == (DialogResult == true))
            {
                System.IO.Path.GetFileName(sfd.FileName);
            }
            return System.IO.Path.GetFullPath(sfd.FileName);
        }

        private void optionsBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
