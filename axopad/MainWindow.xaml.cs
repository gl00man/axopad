using Microsoft.Win32;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;


namespace axopad
{
    public partial class MainWindow : Window
    {
        string resetColorPhrase = "";
        string filePath = "";
        bool textChanged;

        public MainWindow()
        {
            InitializeComponent();

            saveFileBtn.IsEnabled = false;
            textChanged = false;
            ChangeProperties();
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
            textChanged = false;
        }

        private void saveFileBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFile(false, filePath);
            textChanged = false;
        }

        private void saveAsBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFile(true, GetToSavePath());
            textChanged = false;
        }

        private void toolsBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenToolsWindow();
        }

        private void optionsBtn_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw;
            sw = new SettingsWindow();
            sw.Owner = this;
            sw.Show();
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
                filePath = path;
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
            else if (saveAs && filePath == "")
            { }
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

        private void OpenToolsWindow()
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
            resetColorPhrase = find;
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
            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
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
                else if (e.Key == Key.T)
                {
                    OpenToolsWindow();
                }
                else if (e.Key == Key.H)
                {
                    SettingsWindow sw;
                    sw = new SettingsWindow();
                    sw.Owner = this;
                    sw.Show();
                }
                else if (e.Key == Key.X)
                {
                    this.Close();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (textChanged)
            {
                MessageBoxResult choice = MessageBox.Show("Do you want to save your changes?", "", MessageBoxButton.YesNo);
                if (choice == MessageBoxResult.Yes)
                {
                    if (filePath != null)
                    {
                        SaveFile(true, GetToSavePath());
                    }
                    else
                    {
                        SaveFile(false, filePath);
                    }
                }
            }
        }

        /*
         * RICHTEXTBOX EVENTS
         */

        private void mainTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            textChanged = true;
        }

        private void mainTxt_Loaded(object sender, RoutedEventArgs e)
        {
            textChanged = false;

            TextRange textRange = new TextRange(mainTxt.Document.ContentStart, mainTxt.Document.ContentEnd);
            string text = textRange.Text;

            string keywords = "(public|private|partial|static|namespace|class|using|void|foreach|in)";
            MatchCollection keywordMatches = Regex.Matches(text, keywords);

            foreach (Match m in keywordMatches)
            {
                int start = m.Index;
                int end = m.Index + m.Length;

                TextRange tx = new TextRange(textRange.Start.GetPositionAtOffset(start), textRange.Start.GetPositionAtOffset(end));
                tx.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Gray));
            }
        }

        /*
         * READING SETTINGS AND USING IT
         */

        private String[] ReadSettingsFromTxt()
        {
            using (StreamReader sr = new StreamReader("settings.txt", true))
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

        public void ChangeProperties()
        {
            String[] parameters = ReadSettingsFromTxt();

            TextRange text = new TextRange(mainTxt.Document.ContentStart, mainTxt.Document.ContentEnd);
            mainTxt.Focus();
            text.ApplyPropertyValue(RichTextBox.FontFamilyProperty, parameters[0]);
            text.ApplyPropertyValue(RichTextBox.FontSizeProperty, parameters[1]);
            mainTxt.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString(parameters[2]));
        }
    }
}
