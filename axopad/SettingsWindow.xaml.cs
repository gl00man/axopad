using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace axopad
{
    /// <summary>
    /// Logika interakcji dla klasy SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            String[] parameters = ReadSettingsFromTxt();
            changeFontCmb.Text = parameters[0];
            fontSizeCmb.Text = parameters[1];
            fontColorTxt.Text = parameters[2];
        }

        private void ComboBoxes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(changeFontCmb.Text != "" && fontColorTxt.Text != "")
            {
                SaveSettingsToTxt();
                ((MainWindow)this.Owner).ChangeProperties();
            }
        }

        private void fontColorTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(fontColorTxt.Text != "" && fontColorTxt.Text != " ")
            {
                try
                {
                    SaveSettingsToTxt();
                    ((MainWindow)this.Owner).ChangeProperties();
                }
                catch
                { }
            }
        }

        private void SaveSettingsToTxt()
        {
            using (StreamWriter sw = new StreamWriter("settings.txt", true))
            {
                sw.WriteLine("{0}|{1}|{2}", changeFontCmb.Text, fontSizeCmb.Text, fontColorTxt.Text);
                sw.Close();
            }
        }

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
    }
}
