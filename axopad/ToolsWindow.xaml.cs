using System.Windows;

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
