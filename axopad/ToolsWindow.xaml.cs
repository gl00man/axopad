using System;
using System.Collections.Generic;
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
    public partial class ToolsWindow : Window
    {
        public ToolsWindow()
        {
            InitializeComponent();
        }

        private void findBtn_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)this.Owner).FindText(findPhraseTxt.Text);
            this.Close();
        }

        private void replaceBtn_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)this.Owner).ReplaceText(findForReplaceTxt.Text, textToReplaceTxt.Text);
            this.Close();
        }
    }
}
