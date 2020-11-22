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

            saveFileBtn.IsEnabled = false;
        }

        /*
         * BUTTONS EVENTS
         */

        private void newFileBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearData(false);
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

        private void toolsBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenToolsWindow();
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
            if (path != null && saveAs)
            {
                TextRange range;
                FileStream fStream;

                range = new TextRange(mainTxt.Document.ContentStart, mainTxt.Document.ContentEnd);

                fStream = new FileStream(path, FileMode.Create);
                range.Save(fStream, DataFormats.Text);
                fStream.Close();
                saveFileBtn.IsEnabled = true;
                titleBlockTxt.Text = path;
            }
            else if (filePath != "" && !saveAs)
            {
                TextRange range;
                FileStream fStream;

                range = new TextRange(mainTxt.Document.ContentStart, mainTxt.Document.ContentEnd);

                fStream = new FileStream(path, FileMode.Create);
                range.Save(fStream, DataFormats.Text);
                fStream.Close();
                titleBlockTxt.Text = path;
            }
            else
            {
                MessageBox.Show("Can't find file!");
                ClearData(true);
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

                saveFileBtn.IsEnabled = true;
                titleBlockTxt.Text = path;
            }
            else if (!File.Exists(path) && path != null)
            {
                MessageBox.Show("Can't find file!");
            }
        }

        private void ClearData(bool isDeleted)
        {
            if (!isDeleted)
            {
                filePath = "";
                titleBlockTxt.Text = "Untitled.txt";
                saveFileBtn.IsEnabled = false;
                mainTxt.Document.Blocks.Clear();
            }
            else
            {
                filePath = "";
                titleBlockTxt.Text = "Untitled.txt";
                saveFileBtn.IsEnabled = false;
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
            Nullable<bool> dialogOk = sfd.ShowDialog();

            if (dialogOk == true)
            {
                System.IO.Path.GetFileName(sfd.FileName);
                return System.IO.Path.GetFullPath(sfd.FileName);
            }
            return null;
        }

        /*
         * SEARCH AND REPLACE
         */

        private void OpenToolsWindow ()
        {
            ToolsWindow tw = new ToolsWindow();
            tw.Owner = this;
            tw.Show();
        }

        public void ReplaceText(string find, string replaceText)
        {
            string text = new TextRange(mainTxt.Document.ContentStart, mainTxt.Document.ContentEnd).Text;
            text = text.Replace(find, replaceText);
            if (text != new TextRange(mainTxt.Document.ContentStart, mainTxt.Document.ContentEnd).Text)
            {
                new TextRange(mainTxt.Document.ContentStart, mainTxt.Document.ContentEnd).Text = text;
            }
        }

        public void FindText(string find)
        {
            FindTextInRange(find).ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Red));
        }

        private TextRange FindTextInRange(string find)
        {
            TextRange searchRange = new TextRange(mainTxt.Document.ContentStart, mainTxt.Document.ContentEnd);

            int offset = searchRange.Text.IndexOf(find, StringComparison.OrdinalIgnoreCase);
            if (offset < 0)
                return null; 

            var start = GetTextPositionAtOffset(searchRange.Start, offset);
            TextRange result = new TextRange(start, GetTextPositionAtOffset(start, find.Length));

            return result;
        }

        private TextPointer GetTextPositionAtOffset(TextPointer position, int characterCount)
        {
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    int count = position.GetTextRunLength(LogicalDirection.Forward);
                    if (characterCount <= count)
                    {
                        return position.GetPositionAtOffset(characterCount);
                    }

                    characterCount -= count;
                }

                TextPointer nextContextPosition = position.GetNextContextPosition(LogicalDirection.Forward);
                if (nextContextPosition == null)
                    return position;

                position = nextContextPosition;
            }
            return position;
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
                    ClearData(false);
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
