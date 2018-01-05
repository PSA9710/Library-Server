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
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace LibraryServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static public User AppUser;
        static public int tickCount;
        private TcpClient _client;

        private StreamReader _sReader;
        private StreamWriter _sWriter;

        private Boolean _isConnected;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            AppUser = new User();
            Home.SetUser(AppUser);
            ListBooks.SetUser(AppUser);
            _client = new TcpClient();
           // try
            {
                _client.Connect("127.0.0.1", 5555);
                Thread t = new Thread(() => HandleCommunication());
                t.Start();
            }
          //  catch (Exception e)
            {
                MessageBox.Show("Can not connect to server");
            }


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
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                UISearch.ScrollViewerDisplayCards.Margin = new Thickness(0, 0, 8, 10);
                WindowStateIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.FullscreenExit;  //change icon to fullscreenexit


                BorderChat.Margin = new Thickness(7 + BorderChat.Margin.Left, 0, BorderChat.Margin.Left + 2, BorderChat.Margin.Bottom + 7);
            }
            else if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                UISearch.ScrollViewerDisplayCards.Margin = new Thickness(0);
                WindowStateIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Fullscreen; //change icon to fullscreen
                BorderChat.Margin = new Thickness(BorderChat.Margin.Left - 7, 0, BorderChat.Margin.Left - 2, BorderChat.Margin.Bottom - 7);
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
            if (SwitchModeButton.IsChecked == true)
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
            USERs.Visibility = Visibility.Hidden;
            TextBlockWhereIAm.Text = "Library: Home";
        }

        private void ButtonBooks_Click(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Hidden;
            BOOKS.Visibility = Visibility.Visible;
            UISearch.Visibility = Visibility.Hidden;
            ListBooks.Visibility = Visibility.Hidden;
            USERs.Visibility = Visibility.Hidden;
            TextBlockWhereIAm.Text = "Library: Add or Modify Books";
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Hidden;
            BOOKS.Visibility = Visibility.Hidden;
            UISearch.Visibility = Visibility.Visible;
            ListBooks.Visibility = Visibility.Hidden;
            USERs.Visibility = Visibility.Hidden;
            TextBlockWhereIAm.Text = "Library: Search Books";
        }

        private void ButtonList_Click(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Hidden;
            BOOKS.Visibility = Visibility.Hidden;
            UISearch.Visibility = Visibility.Hidden;
            ListBooks.Visibility = Visibility.Visible;
            USERs.Visibility = Visibility.Hidden;
            TextBlockWhereIAm.Text = "Library: Booked Books";
        }

        private void ButtonUsers_Click(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Hidden;
            BOOKS.Visibility = Visibility.Hidden;
            UISearch.Visibility = Visibility.Hidden;
            ListBooks.Visibility = Visibility.Hidden;
            USERs.Visibility = Visibility.Visible;
            TextBlockWhereIAm.Text = "Library: Add or Modify Users";
        }

        public void ToggleButtonEnabled()
        {
            MenuToggleButton.IsEnabled = !MenuToggleButton.IsEnabled;
        }

        public void AddBookToUser(int ISBN)
        {
            //MessageBox.Show(ISBN.ToString());
            AppUser.AddBooks(ISBN);

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


        public void HandleCommunication()
        {
            _sReader = new StreamReader(_client.GetStream(), Encoding.ASCII);
            _isConnected = true;
            String sData = null;
            ChatBox cb;
            //detele this if u on Winforms/wpf
            //Console.WriteLine(_sReader.ReadLine().ToString());

            while (_isConnected)
            {
                // if you want to receive anything
                String sDataIncomming = _sReader.ReadLine();
                Console.WriteLine(sDataIncomming);
                //  MethodInvoker m = new MethodInvoker(() => clientForm.textBox1.Text += ("Server: " + sDataIncomming + Environment.NewLine));
                //Dispatcher.Invoke(new Action(() => chat.Text += sData));


                //split string for easier work
                String[] stuff = sDataIncomming.Split(new string[] { "#$#$" }, StringSplitOptions.None);
                String Nume = stuff[0];
                String ProfilePic = stuff[1];
                String Message = stuff[2];
                Console.WriteLine(Nume + "   " + Message);
                Console.WriteLine("Spawning...");
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    cb = new ChatBox(true, Message, Nume, ProfilePic);
                    cb.HorizontalAlignment = HorizontalAlignment.Left;
                    cb.Margin = new Thickness(0, 0, 0, 3);
                    StackPanelChat.Children.Add(cb);
                    Console.WriteLine("Am spawnat");
                    int i;
                    if (BadgeChat.Badge.ToString() == "")
                    { i = 0; }
                    else
                    {
                        i = Convert.ToInt32(BadgeChat.Badge.ToString());
                    }
                    if (ToggleButtonMenu.IsChecked == false)
                        BadgeChat.Badge = ++i;
                }));
            }
        }

        private void ButtonSendChat_Click(object sender, RoutedEventArgs e)
        {
            String stringSend = null;
            String AppUserName = AppUser.Name + " " + AppUser.LastName;
            String ProfilePic = AppUser.ProfilePic;
            stringSend = AppUserName + "#$#$" + ProfilePic + "#$#$" + TextBoxChat.Text;
            string s = TextBoxChat.Text;
            TextBoxChat.Clear();

            if (!_isConnected) { MessageBox.Show("not Connected to the Server"); return; }

            ChatBox cb = new ChatBox(false, s, "YOU", ProfilePic);
            cb.HorizontalAlignment = HorizontalAlignment.Right;
            cb.Margin = new Thickness(0, 0, 0, 3);
            StackPanelChat.Children.Add(cb);

            _sWriter = new StreamWriter(_client.GetStream(), Encoding.ASCII);
            _sWriter.WriteLine(stringSend);
            _sWriter.Flush();
        }

        private void TextBoxChat_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ButtonSendChat_Click(this, new RoutedEventArgs());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isConnected)
            {
                _sWriter = new StreamWriter(_client.GetStream(), Encoding.ASCII);
                _sWriter.WriteLine("#I am OuT#");
                _sWriter.Flush();
            }

            _isConnected = false;

            Environment.Exit(Environment.ExitCode);
        }

        private void MenuToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            BadgeChat.Badge = "";
        }
    }
}
