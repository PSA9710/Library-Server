using MaterialDesignThemes.Wpf;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace LibraryServer
{

    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        #region Local_Variables
        int MaximumTextCharacters = 15;//the amount of characters a user is allowed to input
        bool AcceptButtonIsPressed = false;
        int tickCount = 0; //check ticks in order to Call SnacbkarEnque Known ISSUE #738
        #endregion

        private User AppUser;

        public Home()
        {
            InitializeComponent();
            LabelTextChangeRelatedToTime();

            DispatcherTimer ActualTime = new DispatcherTimer();
            ActualTime.Interval = TimeSpan.FromSeconds(1); //set the interval when the ticks will ocur
            ActualTime.Tick += TimerTick;   //Add TimerTick to fired events
            ActualTime.Start();

            ChangeNameLabelColour(false);

        }

        public void SetUser(User usr)
        {
            AppUser = usr;
        }

        #region Time_Related_Methods
        private void LabelTextChangeRelatedToTime()
        {
            int time = DateTime.Parse(DateTime.Now.ToString("HHtt")).Hour;
            if (time < 12) LabelGreeting.Content = "Good Morning,";
            else if (time < 17) LabelGreeting.Content = "Good Afternoon,";
            else if (time < 24) LabelGreeting.Content = "Good Evening,";

        }

        //functie ce updateaza timpul in aplicatie
        private void TimerTick(Object sender, EventArgs e)
        {
            //Set content of Label to current time in a HH:MM format
            LabelTimer.Content = DateTime.Now.ToString("HH:mm");
            tickCount++;
        }
        #endregion

        #region SnackBarDisplays
        //Recieves a string and time to display, and pops a snackbarnotification with the recieved string, and time of displayment

        //There's a issue with Snackbars poping a shor period of time , so i had to set a timeinterval
        private void SnackbarMessageDisplay(string s, int _timeSpan)
        {
            if (tickCount < 2 + (_timeSpan / 1000)) return;
            ////SnackbarMaximumCharacters.IsActive = true;
            //var messageQueue = SnackbarMaximumCharacters.MessageQueue;

            //tickCount = 0;//reset tick count
            ////find out what this shit does
            //Task.Factory.StartNew(() => messageQueue.Enqueue(s));
            // var message = "Maximum character  is " + MaximumTextCharacters.ToString() + "!";
            //var messageQueue = SnackbarDialogHost.MessageQueue;
            ////find out what this shit does
            //Task.Factory.StartNew(() => messageQueue.Enqueue(message));


            var messageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(_timeSpan));
            SnackbarMaximumCharacters.MessageQueue = messageQueue;
            SnackbarMaximumCharacters.MessageQueue.Enqueue(s);
            tickCount = 0;
        }

        private void SnackBarDialogHostMessageDisplay(string s, int _timeSpan)
        {
            if (tickCount < 1 + _timeSpan / 1000) return;


            //var message = "Maximum character  ce  is " + MaximumTextCharacters.ToString() + "!";
            //var messageQueue = SnackbarDialogHost.MessageQueue;
            ////find out what this shit does
            //Task.Factory.StartNew(() => messageQueue.Enqueue(message));


            var messageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(_timeSpan));
            SnackbarDialogHost.MessageQueue = messageQueue;
            SnackbarDialogHost.MessageQueue.Enqueue(s);
            tickCount = 0;

        }
        #endregion

        #region HOME_UI
        #region TextBoxNameInput
        private void TextBoxNameInput_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (TextBoxNameInput.Text.Length == MaximumTextCharacters)
            {
                //if one of the following keys is pressed, ignore the event
                if (!GetPressedKey(e))
                {
                    e.Handled = true;
                    var message = "Maximum character limit is " + MaximumTextCharacters.ToString() + "!";
                    SnackbarMessageDisplay(message, 1500);
                }
            }
            if (e.Key == Key.Enter)
            {
                if (TextBoxNameInput.Text == "")
                {
                    SnackbarMessageDisplay("Your Forname is required in order to login", 1500);
                    e.Handled = true;
                    return;
                }
                //fire the clickbutton in order to pop DialogBox;
                DialogHostLogIn.IsOpen = true;
                //B/*uttonRaiseDialog.Command.Execute(ButtonRaiseDialog.CommandParameter);*/
            }
        }

        // if text(has changed& has more than the maximumtextcharacters, reset and display message
        private void TextBoxNameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBoxNameInput.Text.Length > MaximumTextCharacters)
            {
                TextBoxNameInput.Text = "";
                var message = "Maximum character limit is " + MaximumTextCharacters.ToString() + "! The name you tried to enter exceeds the allowed limit!";
                SnackbarMessageDisplay(message, 1500);
            }
        }
        #endregion


        #endregion

        #region DialogHost

        #region DialogHost_FiredEvents
        //when DialogHostLogIn is opened, execute the following code
        private void DialogHostLogIn_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            TextBoxUserName.Text = TextBoxNameInput.Text;
            //Keyboard.Focus(PasswordBoxUserPassword);
        }

        //When te DIalogHostLogIn is closing, execute the followiing code
        private void DialogHostLogIn_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            //if the AcceptButton was pressed, do the folowing else do the other
            if (AcceptButtonIsPressed)
            {
                LogInSuccessfull(AcceptButtonIsPressed, TextBoxUserName.Text);
                var message = "Succesfully logged in! Welcome " + TextBoxUserName.Text + "!";
                SnackbarMessageDisplay(message, 2000);
            }
            else
            {
                TextBoxNameInput.Text = ""; //reset TextBoxNameInput
                var message = "\"Cancel\" was pressed by the user";
                SnackbarMessageDisplay(message, 2000);
            }

            PasswordBoxUserPassword.Password = "";//resets password content 
            TextBoxUserName.Text = "";//resets name content
            AcceptButtonIsPressed = false; //make sure it's false, whatever it happens
        }


        #endregion

        #region DialogHost_Content

        private void LabelName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LabelName.Visibility = Visibility.Collapsed;
            TextBoxNameInput.Visibility = Visibility.Visible;
            TextBoxNameInput.Text = LabelName.Content.ToString();
        }

        private void ButtonAcceptDialogHost_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxUserName.Text == "")
            {
                var message = "Name required to login!";
                SnackBarDialogHostMessageDisplay(message, 1000);
            }

            if (PasswordBoxUserPassword.Password != "")
            {
                if (PasswordBoxUserPassword.Password.Length < 13)
                {
                    var message = "Password is too short!";
                    SnackBarDialogHostMessageDisplay(message, 1000);
                    PasswordBoxUserPassword.Focus();
                    e.Handled = true;
                }
                else
                {

                    //if userLogin fails
                    if (!SQL_Connect())
                    {
                        var message = "User does not exist!";
                        SnackBarDialogHostMessageDisplay(message, 1000);
                        PasswordBoxUserPassword.Password = "";
                        TextBoxUserName.Text = "";
                        TextBoxUserName.Focus();
                        return;
                    }

                    

                    Console.WriteLine("Closing Dialog Host");
                    AcceptButtonIsPressed = true;
                    BUTTONCLOSEDIALOG.Command.Execute(null);
                    var target = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;
                    target.MenuToggleButton.IsEnabled = true;
                    if(!AppUser.isTeacher)
                    target.ButtonBooks.Visibility = Visibility.Hidden;
                    e.Handled = true;
                }
            }
            else
            {
                var message = "CNP can not be empty!";
                SnackBarDialogHostMessageDisplay(message, 1000);
                e.Handled = true;
            }

        }

        private void ButtonCancelDialogHost_Click(object sender, RoutedEventArgs e)
        {
            AcceptButtonIsPressed = false;
            BUTTONCLOSEDIALOG.Command.Execute(null);

        }

        private void TextBoxUserName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (TextBoxUserName.Text.Length == MaximumTextCharacters)
            {
                if (!GetPressedKey(e))
                {
                    e.Handled = true;
                    SnackBarDialogHostMessageDisplay("Character limit is" + MaximumTextCharacters.ToString() + "!", 1000);

                }
            }
        }

        private void PasswordBoxUserPassword_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonAcceptDialogHost.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                e.Handled = true;
                return;
            }
            if (PasswordBoxUserPassword.Password.Length == 13)
            {
                if (GetPressedKey(e)) return;
                SnackBarDialogHostMessageDisplay("Maximum Password length is 13!", 1000);

            }
            //If the pressed buttons are not numbers or special cases , error 
            if (!GetPressedKey(e) && !(e.Key >= Key.D0 && e.Key <= Key.D9))
            {
                e.Handled = true;
                var message = "Please insert only numbers";
                SnackBarDialogHostMessageDisplay(message, 1000);

            }


        }

        private void PasswordBoxUserPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {

            if (PasswordBoxUserPassword.Password == "") return;
            string testString;
            testString = PasswordBoxUserPassword.Password;
            //if password contains characters reset password, and send error notification
            if (!testString.All(char.IsDigit))
            {
                var message = "Only numbers are allowed";
                SnackBarDialogHostMessageDisplay(message, 1000);
                PasswordBoxUserPassword.Password = "";
            }
            if (PasswordBoxUserPassword.Password.Length < 13) return;
        }
        #endregion

        #endregion

        #region ExtraFunctions
        /// <summary>
        /// This function tests if a specific Key combination has been Pressed
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool GetPressedKey(KeyEventArgs e)
        {
            if (
                    e.Key == System.Windows.Input.Key.Delete ||
                    e.Key == System.Windows.Input.Key.Back ||
                    e.Key == System.Windows.Input.Key.Left ||
                    e.Key == System.Windows.Input.Key.Right ||
                    e.Key == System.Windows.Input.Key.A && Keyboard.Modifiers == ModifierKeys.Control ||
                    e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control ||
                    e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control ||
                    e.Key == Key.X && Keyboard.Modifiers == ModifierKeys.Control ||
                    e.Key == Key.End || e.Key == Key.Home ||
                    (e.Key == Key.End && Keyboard.Modifiers == ModifierKeys.Shift ||
                    e.Key == Key.Home && Keyboard.Modifiers == ModifierKeys.Shift) ||
                    e.Key == Key.Tab ||
                    e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
                    e.Key == Key.LeftShift || e.Key == Key.RightShift || e.Key == Key.Enter
                    ) return true;
            return false;
        }
        /// <summary>
        /// If the Login attempt was succesful this function changes the window changes
        /// e.g Textbox transforms into LABEL
        /// </summary>
        /// <param name="loginResult">
        /// If true switches the textbox with a label
        /// </param>
        /// <param name="Name">Name is the name with which the user logged in</param>
        private void LogInSuccessfull(bool loginResult, string Name)
        {
            if (loginResult)
            {
                LabelName.Content = Name;
                LabelName.Visibility = Visibility.Visible;
                TextBoxNameInput.Visibility = Visibility.Collapsed;

            }
        }
        /// <summary>
        /// Sets the color of the LabelName according to the selected Mode
        /// </summary>
        /// <param name="isDark">if true, is Dark Mode</param>
        public void ChangeNameLabelColour(bool isDark)
        {

            if (isDark)
            {
                Application.Current.Resources["SwitchAccentPrimary"] = FindResource("SecondaryAccentBrush");
            }
            else
            {
                Application.Current.Resources["SwitchAccentPrimary"] = FindResource("PrimaryHueMidBrush");
            }
        }

        #endregion

        #region SQL_CONNECT
        /// <summary>
        /// Connect to the SQL_database for user login
        /// </summary>
        private bool SQL_Connect()
        {
            try
            {
                Console.WriteLine("Connecting to SQL SERVER");
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "tcp:libraryoop.database.windows.net,1433";
                builder.UserID = "Library";
                builder.Password = "Aa123456789";
                builder.InitialCatalog = "Library";

                Console.WriteLine("Conecting to SQL server.....");
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connection Succesfull");

                    Console.WriteLine("Attempting LogIN");
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT * from " + ComboBoxRank.SelectionBoxItem.ToString() + "s where Prenume = @nume AND CNP=@cnp;");
                    String sql = sb.ToString();



                    //check if user exists
                    using (SqlCommand sql_command = new SqlCommand(sql, connection))
                    {
                        sql_command.Parameters.AddWithValue("@nume", TextBoxUserName.Text.ToString());
                        sql_command.Parameters.AddWithValue("@cnp", PasswordBoxUserPassword.Password.ToString());

                        using (SqlDataReader reader = sql_command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                AppUser.SetName(reader["Prenume"] as string);
                                AppUser.SetLastName(reader["Nume"] as string);
                                AppUser.SetTeacher((ComboBoxRank.SelectionBoxItem.ToString() != "Librarian") ? false : true);
                                

                                if (AppUser.AnAbs > DateTime.Now.Year && !AppUser.isTeacher)
                                {
                                    var message = "User is no more a Student";
                                    SnackBarDialogHostMessageDisplay(message, 1000);
                                    PasswordBoxUserPassword.Password = "";
                                    TextBoxUserName.Text = "";
                                    TextBoxUserName.Focus();
                                    return false;
                                }


                                if (!AppUser.isTeacher)
                                AppUser.SetAnAbs(reader.GetInt32(reader.GetOrdinal("An_absolvire")));
                                AppUser.AddBooks(reader["BookList"] as string);
                            }
                            else
                            {
                                Console.WriteLine("LogIn failed! User does not exist! \nReseting Values...."); return false;
                            }
                        }
                    }
                    Console.WriteLine("User found");
                    //if user exists LOGIN
                    sb.Clear();
                    sb.Append("SELECT Profilepic from " + ComboBoxRank.SelectionBoxItem.ToString() + "s where Prenume = @nume AND CNP=@cnp;");
                    sql = sb.ToString();
                    //retrieve profilePicture
                    using (SqlCommand sql_command = new SqlCommand(sql, connection))
                    {
                        sql_command.Parameters.AddWithValue("@nume", TextBoxUserName.Text.ToString());
                        sql_command.Parameters.AddWithValue("@cnp", PasswordBoxUserPassword.Password.ToString());
                        using (SqlDataReader reader = sql_command.ExecuteReader())
                        {       //nu merge tui ceapa lui
                            if (reader.Read())
                            {
                                if (reader["Profilepic"] != null)
                                {
                                    Console.WriteLine("Changing profile picture");
                                    ProfilePicture.ImageSource = new BitmapImage(new Uri(reader.GetString(reader.GetOrdinal("Profilepic")), UriKind.Absolute));
                                    NoProfilePicture.Visibility = Visibility.Hidden;
                                }
                            }
                        }
                    }
                    return true;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return false;
        }
        #endregion

    }
}
