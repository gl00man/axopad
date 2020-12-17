using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace axopad
{
    public partial class SettingsWindow : Window
    {
        bool optionChanged;
        bool saveButtonPressed;
        public SettingsWindow()
        {
            InitializeComponent();
            try
            {
                String[] parameters = ReadSettingsFromTxt();
                changeFontCmb.Text = parameters[0];
                fontSizeCmb.Text = parameters[1];
                fontColorTxt.Text = parameters[2];
                if (parameters[3] == "true")
                {
                    showLineNumsChck.IsChecked = true;
                }
                else
                {
                    showLineNumsChck.IsChecked = false;
                }
                optionChanged = false;
                saveButtonPressed = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ComboBoxes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            optionChanged = true;
        }

        private void fontColorTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            optionChanged = true;
        }

        private void SaveSettingsToTxt()
        {
            string check = "true";

            File.WriteAllText(GetSettingsPath(), String.Empty);

            if (showLineNumsChck.IsChecked == false)
            {
                check = "false";
            }


            using (StreamWriter sw = new StreamWriter(GetSettingsPath(), true))
            {
                sw.WriteLine("{0}|{1}|{2}|{3}", changeFontCmb.Text, fontSizeCmb.Text, fontColorTxt.Text, check);
                sw.Close();
            }
        }

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

        private void saveSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            saveButtonPressed = true;
            if (changeFontCmb.Text != "" && fontColorTxt.Text != "" && fontColorTxt.Text != "" && fontColorTxt.Text != " ")
            {
                try
                {
                    SaveSettingsToTxt();
                    ((MainWindow)this.Owner).ChangeProperties();
                }
                catch
                { }
                this.Close();
            }
        }

        private string GetSettingsPath()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("axopad.dll", @"Assets\Data\settings.txt");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (changeFontCmb.Text != "" && fontColorTxt.Text != "" && fontColorTxt.Text != "" && fontColorTxt.Text != " " && saveButtonPressed == false)
            {
                if (!optionChanged)
                { }
                else
                {
                    MessageBoxResult choice = MessageBox.Show("Apply your changes?", "", MessageBoxButton.YesNoCancel);
                    if (choice == MessageBoxResult.Yes)
                    {
                        SaveSettingsToTxt();
                        ((MainWindow)this.Owner).ChangeProperties();
                    }
                    else
                    {
                        this.Close();
                    }

                }
            }
        }
    }
}
