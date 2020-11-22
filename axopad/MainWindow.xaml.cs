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
            ClearData();
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

        private void optionsBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void helpBtn_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow hw;
            hw = new HelpWindow();
            hw.Show();
        }

        /*
         * TOOLBAR EVENTS
         */


        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void fullScreenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState != WindowState.Maximized)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void minimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void toolBarGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch
            { }
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
            titleBlockTxt.Text = path;
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
                titleBlockTxt.Text = path;
            }
            else if(!File.Exists(path))
            {
                MessageBox.Show("Can't find file!");
            }
        }

        private void ClearData()
        {
            filePath = "";
            titleBlockTxt.Text = "Untitled.txt";
            mainTxt.Document.Blocks.Clear();
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
        
        /*
         * MAIN WINDOW EVENTS & SHORTCUTS
         */

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.S)
            {
                SaveFile(true, GetToSavePath());
            }
            else if(Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Key == Key.N)
                {
                    ClearData();
                }
                else if (e.Key == Key.O)
                {
                    filePath = GetFilePath();
                    OpenFile(filePath);
                }
                else if (e.Key == Key.S)
                {
                    SaveFile(false, filePath);
                }
                else if(e.Key == Key.F)
                {
                    //finding
                }
                else if(e.Key == Key.H)
                {
                    //options
                }
                else if (e.Key == Key.X)
                {
                    this.Close();
                }
            }
        }
    }
}
