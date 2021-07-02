using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Squirrel;

namespace SquirrelApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            lblVersion.Content = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            lblExePath.Content = Assembly.GetExecutingAssembly().Location;            
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var updater = (Updater)this.FindResource("updater");
            updater.UpdateApp();
        }
       
        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            var updater = (Updater)this.FindResource("updater");
            updater.RestartApp();
        }
    }
}
