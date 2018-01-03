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
using System.Windows.Threading;

namespace LibraryServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static public User AppUser;
        static public int tickCount;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            AppUser = new User();
            Home.SetUser(AppUser);
            ListBooks.SetUser(AppUser);
        }

        

        private void InitializeTimer()
        {
            DispatcherTimer ActualTime = new DispatcherTimer();
            ActualTime.Interval = TimeSpan.FromSeconds(1); //set the interval when the ticks will ocur
            ActualTime.Tick += TimerTick;   //Add TimerTick to fired events
            ActualTime.Start();
        }
        
        private void TimerTick(object sender, EventArgs e)
        {
            tickCount++;
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
                UISearch.ScrollViewerDisplayCards.Margin = new Thickness(0,0,8,10);
                WindowStateIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.FullscreenExit;  //change icon to fullscreenexit
            }
             else if(WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                UISearch.ScrollViewerDisplayCards.Margin = new Thickness(0);
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
                new Home().ChangeNameLabelColour(true);
            }
        }

        // If Switchmode has been checked , switch to light mode
        private void SwitchModeButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (SwitchModeButton.IsChecked == false)
            {
                SwitchMode.Content = "Switch to Dark Mode";
                new PaletteHelper().SetLightDark(false); //function to switch between light ui and dark ui
                new Home().ChangeNameLabelColour(false);
            }
        }

 
        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Visible;
            BOOKS.Visibility = Visibility.Hidden;
            UISearch.Visibility = Visibility.Hidden;
            ListBooks.Visibility = Visibility.Hidden;
            TextBlockWhereIAm.Text = "Library: Home";
        }

        private void ButtonBooks_Click(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Hidden;
            BOOKS.Visibility = Visibility.Visible;
            UISearch.Visibility = Visibility.Hidden;
            ListBooks.Visibility = Visibility.Hidden;
            TextBlockWhereIAm.Text = "Library: Add or Remove Books";
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Hidden;
            BOOKS.Visibility = Visibility.Hidden;
            UISearch.Visibility = Visibility.Visible;
            ListBooks.Visibility = Visibility.Hidden;
            TextBlockWhereIAm.Text = "Library: Search Books";
        }

        private void ButtonList_Click(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Hidden;
            BOOKS.Visibility = Visibility.Hidden;
            UISearch.Visibility = Visibility.Hidden;
            ListBooks.Visibility = Visibility.Visible;
            TextBlockWhereIAm.Text = "Library: Booked Books";
        }

        public  void ToggleButtonEnabled()
        {
            MenuToggleButton.IsEnabled = !MenuToggleButton.IsEnabled;
        }

        public void AddBookToUser(int ISBN)
        {
            //MessageBox.Show(ISBN.ToString());
            AppUser.AddBooks(ISBN);
            foreach (int i in AppUser.ReservedBooks)
                MessageBox.Show(i.ToString());
        }

        public void RemoveBookFromUser(int ISBN)
        {
            Console.WriteLine("Removing Book from User with ISBN=" + ISBN.ToString());
            AppUser.RemoveBooks(ISBN);
        }
        
        public void ListBookUpdateAfterRemove()
        {
            ListBooks.UpdateDisplayAfterDelete();
        }
    }
}
