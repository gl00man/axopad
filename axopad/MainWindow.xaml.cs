using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Document;
using Ookii.Dialogs.Wpf;
using System.Windows.Controls;
using System.Collections.Generic;

namespace axopad
{
    public partial class MainWindow : Window
    {
        string[] args;
        string compilerPath = "";
        string lang = "";
        string filePath = "";
        string extension = "";
        bool xshdLoaded = false;
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
            OpenFile(GetFilePath());
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
            OpenSettingsWindow();
        }

        private void helpBtn_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow hw;
            hw = new HelpWindow();
            hw.Show();
        }

        private void runBtn_Click(object sender, RoutedEventArgs e)
        {
            if (compilerPath != "" && compilerPath != null  && compilerPath != "null")
            {
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo()
                {
                    FileName = compilerPath,
                    Arguments = filePath,
                    UseShellExecute = false,
                };
                p.Start();
            }
        }

        private void compilersBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenCompilersWindow();
        }

        private void openFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new VistaFolderBrowserDialog();

            if (folderDialog.ShowDialog() == true)
            {
                OpenFolder(folderDialog.SelectedPath);
            }

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
         * TREEVIEW EVENTS
         */

        private void foldersTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var element = foldersTreeView.SelectedItem as TreeViewItem;

            try
            {
                if (element.IsSelected)
                {
                    CheckIfTextChangedToExit(true);
                    OpenFile(element.Tag.ToString());
                    textChanged = false;
                }
            }
            catch { }
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

        private void OpenFolder(string path)
        {
            DirectoryInfo rootDirectoryInfo = new DirectoryInfo(@path);
            foldersTreeView.Items.Clear();
            foldersTreeView.Items.Add(CreateDirectoryNode(rootDirectoryInfo));
        }

        private void ClearData(bool isDeleted)
        {
            if (!isDeleted)
            {
                filePath = "";
                titleBlockTxt.Text = "new file";
                saveFileBtn.IsEnabled = false;
                xshdLoaded = false;
                mainTxt.Clear();
            }
            else
            {
                filePath = "";
                titleBlockTxt.Text = "new file";
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

        private void OpenSettingsWindow()
        {
            SettingsWindow sw;
            sw = new SettingsWindow();
            sw.Owner = this;
            sw.Show();
        }
        
        private void OpenCompilersWindow()
        {
            CompilersExplorerWindow cew;
            cew = new CompilersExplorerWindow();
            cew.Owner = this;
            cew.Show();
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

        public bool FindText(string find)
        {
            Regex regex = GetRegex(find);
            int start = regex.Options.HasFlag(RegexOptions.RightToLeft) ?
            mainTxt.SelectionStart : mainTxt.SelectionStart + mainTxt.SelectionLength;
            Match match = regex.Match(mainTxt.Text, start);

            if (!match.Success)
            {
                if (regex.Options.HasFlag(RegexOptions.RightToLeft))
                    match = regex.Match(mainTxt.Text, mainTxt.Text.Length);
                else
                    match = regex.Match(mainTxt.Text, 0);
            }

            if (match.Success)
            {
                mainTxt.Select(match.Index, match.Length);
                TextLocation loc = mainTxt.Document.GetLocation(match.Index);
                mainTxt.ScrollTo(loc.Line, loc.Column);
            }
            return match.Success;
        }

        private Regex GetRegex(string textToFind)
        {
           return new Regex(textToFind);
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
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
            if (xshdLoaded == true)
            {
                CheckForDuplicateSymbol(e);
            }

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
                    if (filePath != null)
                    {
                        OpenFile(filePath);
                        textChanged = false;
                    }
                }
                else if (e.Key == Key.S)
                {
                    SaveFile(false, filePath);
                    textChanged = false;
                }
                else if (e.Key == Key.T)
                {
                    OpenSettingsWindow();
                }
                else if (e.Key == Key.H)
                { 

                    OpenToolsWindow();
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
            lineNumTxt.Text = mainTxt.Document.GetLocation(mainTxt.CaretOffset).Line.ToString() + ',';
            columnNumTxt.Text = mainTxt.Document.GetLocation(mainTxt.CaretOffset).Column.ToString();
            Debug.Write(mainTxt.FontFamily);
        }

        private void mainTxt_Loaded(object sender, RoutedEventArgs e)
        {
            textChanged = false;
        }

        private void mainTxt_GotMouseCapture(object sender, MouseEventArgs e)
        {
            lineNumTxt.Text = mainTxt.Document.GetLocation(mainTxt.CaretOffset).Line.ToString() + ',';
            columnNumTxt.Text = mainTxt.Document.GetLocation(mainTxt.CaretOffset).Column.ToString();
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
                ReadXshd(@"Assets\Highlighting\Python-Mode.xshd");
                xshdLoaded = true;
                lang = "Python";
                langTxt.Text = lang;
                SetCompilerPath();
            }
            else if (filePath != "" && extension == ".cs")
            {
                ReadXshd(@"Assets\Highlighting\CSharp-Mode.xshd");
                xshdLoaded = true;
                lang = "C#";
                langTxt.Text = lang;
                SetCompilerPath();
            }
            else if (filePath != "" && extension == ".cpp")
            {
                ReadXshd(@"Assets\Highlighting\CPP-Mode.xshd");
                xshdLoaded = true;
                lang = "C++";
                langTxt.Text = lang;
                SetCompilerPath();
            }
            else if (filePath != "" && extension == ".js")
            {
                ReadXshd(@"Assets\Highlighting\JavaScript-Mode.xshd");
                xshdLoaded = true;
                lang = "Java Script";
                langTxt.Text = lang;
                compilerPath = "";
            }
            else if (filePath != "" && extension == ".html")
            {
                ReadXshd(@"Assets\Highlighting\HTML-Mode.xshd");
                xshdLoaded = true;
                lang = "HTML";
                langTxt.Text = lang;
                compilerPath = "";
            }
            else if (filePath != "" && extension == ".css")
            {
                ReadXshd(@"Assets\Highlighting\CSS-Mode.xshd");
                xshdLoaded = true;
                lang = "CSS";
                langTxt.Text = lang;
                compilerPath = "";
            }
            else if (filePath != "" && extension == ".rs")
            {
                ReadXshd(@"Assets\Highlighting\Rust-Mode.xshd");
                xshdLoaded = true;
                lang = "Rust";
                langTxt.Text = lang;
                SetCompilerPath();
            }
            else
            {
                ReadXshd(@"Assets\Highlighting\Patch-Mode.xshd");
                xshdLoaded = false;
                lang = "Notes";
                langTxt.Text = lang;
                compilerPath = "";
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
            return System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("axopad.dll", @"Assets\Data\settings.txt");
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

        private void CheckForDuplicateSymbol(KeyEventArgs e)
        {
            Debug.Write(mainTxt.CaretOffset);
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (e.Key == Key.OemQuotes)
                {
                    mainTxt.Document.Insert(mainTxt.CaretOffset, "\"");
                    try { mainTxt.CaretOffset = mainTxt.CaretOffset - 1; }
                    catch { }
                }
                else if(e.Key == Key.OemOpenBrackets)
                {
                    mainTxt.Document.Insert(mainTxt.CaretOffset, "}");
                    try { mainTxt.CaretOffset = mainTxt.CaretOffset - 1; }
                    catch { }
                }
                else if (e.Key == Key.D9)
                {
                    mainTxt.Document.Insert(mainTxt.CaretOffset, ")");
                    try { mainTxt.CaretOffset = mainTxt.CaretOffset - 1; }
                    catch { }
                }
            }
            else if (e.Key == Key.OemQuotes)
            {
                mainTxt.Document.Insert(mainTxt.CaretOffset, "\'");
                try { mainTxt.CaretOffset = mainTxt.CaretOffset - 1; }
                catch { }
            }
        }
        private static TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeViewItem { Header = directoryInfo.Name, Tag = directoryInfo.FullName };
            foreach (var directory in directoryInfo.GetDirectories())
                directoryNode.Items.Add(CreateDirectoryNode(directory));

            foreach (var file in directoryInfo.GetFiles())
                directoryNode.Items.Add(new TreeViewItem { Header = file.Name, Tag = file.FullName });

            return directoryNode;
        }

        public void SetCompilerPath()
        {
            Dictionary<string, string> paths = new Dictionary<string, string>();

            String[] data = ReadCompilers();
            paths.Add(data[0], data[1]);
            paths.Add(data[2], data[3]);
            paths.Add(data[4], data[5]);
            paths.Add(data[6], data[7]);

            compilerPath = paths[lang];
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
