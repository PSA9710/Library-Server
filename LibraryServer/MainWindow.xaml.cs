using System;
using System.Collections.Generic;
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

namespace LibraryServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        //ovverride statechanged in order to get acces to window-state/style
        protected override void OnStateChanged(EventArgs e)
        {

            if (WindowState == WindowState.Maximized)
            {

                WindowStyle = WindowStyle.None;
            }
            base.OnStateChanged(e);
        }
        // If maximized==true, this buton closes the app
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
        //return to normal mode out of maximized mode

        private void WindowStateButton_Click(object sender, RoutedEventArgs e)
        {
             if(WindowState==WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                WindowStateIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.FullscreenExit;
            }
             else if(WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                WindowStateIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Fullscreen;
            }
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
