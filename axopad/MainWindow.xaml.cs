using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Threading;

namespace axopad
{
    public partial class MainWindow : Window
    {
        string[] args;
        string resetColorPhrase = "";
        string filePath = "";
        string extension = "";
        bool textChanged;

        public MainWindow(string[] arguments)
        {
            InitializeComponent();

            args = arguments;
        }

        /*
         * BUTTONS EVENTS
         */

        private void newFileBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearData(false);
            GetSyntax();
        }

        private void openFileBtn_Click(object sender, RoutedEventArgs e)
        {
            CheckIfTextChangedToExit(true);
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
            CheckIfTextChangedToExit(false);
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
                FileStream fStream;

                fStream = new FileStream(path, FileMode.Create);
                mainTxt.Save(fStream);
                fStream.Close();
                saveFileBtn.IsEnabled = false;
                titleBlockTxt.Text = path;
                filePath = path;
                GetExtension();
                GetSyntax();
            }
            else if (filePath != "" && !saveAs)
            {
                FileStream fStream;

                fStream = new FileStream(path, FileMode.Create);
                mainTxt.Save(fStream);
                fStream.Close();
                saveFileBtn.IsEnabled = false;
                titleBlockTxt.Text = path;
            }
            else if (saveAs && path == null)
            { }
            else if (!saveAs && filePath == "")
            {
                SaveFile(true, GetToSavePath());
            }
            else
            {
                MessageBox.Show("Can't find file!");
                ClearData(true);
            }
        }

        private void OpenFile(string path)
        {
            FileStream fStream;

            if (File.Exists(path))
            {
                fStream = new FileStream(path, FileMode.OpenOrCreate);
                mainTxt.Load(fStream);
                fStream.Close();

                saveFileBtn.IsEnabled = false;
                filePath = path;
                titleBlockTxt.Text = path;
                GetExtension();
                GetSyntax();
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
                mainTxt.Clear();
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
            fileDialog.Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt|Python File (*.py)|*.py|C# Source File (*.cs)|*.cs|C++ Source File (*.cpp)|*.cpp|HTML Document (*.html)|*.html|JavaScript File (*.js)|*.js|Cascading Style Sheet (*.css)|*.css";
            Nullable<bool> dialogOK = fileDialog.ShowDialog();

            if (dialogOK == true)
            {
                string filePath = "";
                foreach (string x in fileDialog.FileNames)
                {
                    filePath += x;
                }
                String[] extensionArr = filePath.Split('.');
                extension = "." + extensionArr[extensionArr.Length - 1];
                return filePath;
            }
            return null;
        }

        private string GetToSavePath()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "axofile";
            sfd.Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt|Python File (*.py)|*.py|JavaScript File (*.js)|*.js|C# Source File (*.cs)|*.cs|C++ Source File (*.cpp)|*.cpp|HTML Document (*.html)|*.html|Cascading Style Sheet (*.css)|*.css";
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
            string text = mainTxt.Text;
            text = text.Replace(find, replaceText);
            if (text != mainTxt.Text)
            {
                mainTxt.Text = text;
            }
        }

        public void FindText(string find)
        {
            //FindTextInRange(find).ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Red));
            resetColorPhrase = find;
        }

        //private TextRange FindTextInRange(string find)
        //{
        //TextRange searchRange = new TextRange(mainTxt.Document.ContentStart, mainTxt.Document.ContentEnd);

        //int offset = searchRange.Text.IndexOf(find, StringComparison.OrdinalIgnoreCase);
        //if (offset < 0)
        //return null;

        //var start = GetTextPositionAtOffset(searchRange.Start, offset);
        //TextRange result = new TextRange(start, GetTextPositionAtOffset(start, find.Length));

        //return result;
        //}

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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            saveFileBtn.IsEnabled = false;
            textChanged = false;
            ChangeProperties();
            if (args.Length > 0)
            {
                OpenFile(args[0]);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.S)
            {
                SaveFile(true, GetToSavePath());
                textChanged = false;
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftShift) && e.Key == Key.X)
            {
                CheckIfTextChangedToExit(false);
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Key == Key.N)
                {
                    ClearData(false);
                }
                else if (e.Key == Key.O)
                {
                    CheckIfTextChangedToExit(true);
                    filePath = GetFilePath();
                    OpenFile(filePath);
                    textChanged = false;
                }
                else if (e.Key == Key.S)
                {
                    SaveFile(false, filePath);
                    textChanged = false;
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
            }
        }

        /*
         * AVALON EVENTS
         */

        private void mainTxt_TextChanged(object sender, EventArgs e)
        {
            textChanged = true;
            saveFileBtn.IsEnabled = true;
        }

        private void mainTxt_Loaded(object sender, RoutedEventArgs e)
        {
            textChanged = false;
        }

        /*
         * READING SETTINGS AND USING IT
         */

        private String[] ReadSettingsFromTxt()
        {
            using (StreamReader sr = new StreamReader(GetSettingsPath(), true))
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

            mainTxt.Focus();
            mainTxt.FontFamily = new FontFamily(parameters[0]);
            mainTxt.FontSize = double.Parse(parameters[1]);
            mainTxt.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString(parameters[2]));
            mainTxt.ShowLineNumbers = bool.Parse(parameters[3]);
        }

        private void GetSyntax()
        {
            if (filePath != "" && extension == ".py")
            {
                ReadXshd(@"Assets\Python-Mode.xshd");
            }
            else if (filePath != "" && extension == ".cs")
            {
                ReadXshd(@"Assets\CSharp-Mode.xshd");
            }
            else if (filePath != "" && extension == ".cpp")
            {
                ReadXshd(@"Assets\CPP-Mode.xshd");
            }
            else if (filePath != "" && extension == ".js")
            {
                ReadXshd(@"Assets\JavaScript-Mode.xshd");
            }
            else if (filePath != "" && extension == ".html")
            {
                ReadXshd(@"Assets\HTML-Mode.xshd");
            }
            else if (filePath != "" && extension == ".css")
            {
                ReadXshd(@"Assets\CSS-Mode.xshd");
            }
            else
            {
                ReadXshd(@"Assets\Patch-Mode.xshd");
            }
        }

        private void ReadXshd(string xshdPath)
        {
            try
            {
                using (StreamReader s = new StreamReader(GetXshdPath(xshdPath)))
                {
                    using (XmlReader reader = XmlReader.Create(s))
                    {
                        mainTxt.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(
                        reader,
                        HighlightingManager.Instance);
                    }
                }
            }
            catch
            { }
        }

        private void CheckIfTextChangedToExit(bool isOpening)
        {
            if (textChanged)
            {
                MessageBoxResult choice = MessageBox.Show("Do you want to save your changes?", "", MessageBoxButton.YesNoCancel);
                if (choice == MessageBoxResult.Yes)
                {
                    if(isOpening)
                    {
                        ClearData(false);
                    }

                    if (filePath != null)
                    {
                        SaveFile(true, GetToSavePath());
                    }
                    else
                    {
                        SaveFile(false, filePath);
                    }
                }
                else if (choice == MessageBoxResult.No && !isOpening)
                {
                    this.Close();
                }
                else if (choice == MessageBoxResult.Cancel)
                {

                }
                else if(!isOpening)
                {
                    this.Close();
                }
            }
            else if(!isOpening)
            {
                this.Close();
            }
        }

        private string GetSettingsPath()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("axopad.dll", @"Assets\settings.txt");
        }

        private string GetXshdPath(string pathPart)
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("axopad.dll", pathPart);
        }
        
        private void GetExtension()
        {
            String[] extensionArr = filePath.Split('.');
            extension = "." + extensionArr[extensionArr.Length - 1];
        }
    }
}
