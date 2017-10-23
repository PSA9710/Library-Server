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
using MaterialDesignThemes.Wpf;

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
        //switch between maximized and normalized mode
        private void WindowStateButton_Click(object sender, RoutedEventArgs e)
        {
             if(WindowState==WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                WindowStateIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.FullscreenExit;  //change icon to fullscreenexit
            }
             else if(WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                WindowStateIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Fullscreen; //change icon to fullscreen
            }
        }
        // If the tilebar was clicked, allow drag across the screen
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        // If Switchmode has been checked , switch to dark mode
        private void SwitchModeButton_Checked(object sender, RoutedEventArgs e)
        {
            if(SwitchModeButton.IsChecked==true)
            {
                SwitchMode.Content = "Switch to Light Mode";
                new PaletteHelper().SetLightDark(true); //function to switch between light ui and dark ui
            }
        }

        // If Switchmode has been checked , switch to light mode
        private void SwitchModeButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (SwitchModeButton.IsChecked == false)
            {
                SwitchMode.Content = "Switch to Dark Mode";
                new PaletteHelper().SetLightDark(false); //function to switch between light ui and dark ui
            }
        }

       
    }
}
