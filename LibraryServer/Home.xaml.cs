using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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



        public Home()
        {
            InitializeComponent();
            LabelTextChangeRelatedToTime();
            DispatcherTimer ActualTime = new DispatcherTimer();
            ActualTime.Interval = TimeSpan.FromSeconds(1); //set the interval when the ticks will ocur
            ActualTime.Tick += TimerTick;   //Add TimerTick to fired events
            ActualTime.Start();


        }

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
            lbl.Content = tickCount;
        }

        //Recieves a string, and pops a snackbarnotification with the recieved string, and time of displayment
        private void SnackbarMessageDisplay(string s,int _timeSpan)
        {   
            if (tickCount < 2+(_timeSpan/1000)) return;
            ////SnackbarMaximumCharacters.IsActive = true;
            //var messageQueue = SnackbarMaximumCharacters.MessageQueue;

            //tickCount = 0;//reset tick count
            ////find out what this shit does
            //Task.Factory.StartNew(() => messageQueue.Enqueue(s));
            var message = "Maximum character  is " + MaximumTextCharacters.ToString() + "!";
            //var messageQueue = SnackbarDialogHost.MessageQueue;
            ////find out what this shit does
            //Task.Factory.StartNew(() => messageQueue.Enqueue(message));


            var messageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(_timeSpan));
            SnackbarMaximumCharacters.MessageQueue = messageQueue;
            SnackbarMaximumCharacters.MessageQueue.Enqueue(message);
            tickCount = 0;
        }

        private void TextBoxNameInput_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (TextBoxNameInput.Text.Length == MaximumTextCharacters)
            {
                //if one of the following keys is pressed, ignore the event
                if (!(e.Key == System.Windows.Input.Key.Delete ||
                    e.Key == System.Windows.Input.Key.Back ||
                    e.Key == System.Windows.Input.Key.Left ||
                    e.Key == System.Windows.Input.Key.Right ||
                    e.Key == System.Windows.Input.Key.A && Keyboard.Modifiers == ModifierKeys.Control ||
                    e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control ||
                    e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control ||
                    e.Key == Key.X && Keyboard.Modifiers == ModifierKeys.Control ||
                    e.Key == Key.End || e.Key == Key.Home || e.Key == Key.Tab || e.Key == Key.Enter
                    ))
                {
                    e.Handled = true;
                    var message = "Maximum character limit is " + MaximumTextCharacters.ToString() + "!";
                    SnackbarMessageDisplay(message,1500);
                }
            }
            if (e.Key == Key.Enter)
            {
                if (TextBoxNameInput.Text == "")
                {
                    SnackbarMessageDisplay("A username is required in order to login",1500);
                    e.Handled = true;
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
                SnackbarMessageDisplay(message,1500);
            }
        }
        //when DialogHostLogIn is opened, execute the following code
        private void DialogHostLogIn_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            TextBoxUserName.Text = TextBoxNameInput.Text;
            //Keyboard.Focus(PasswordBoxUserPassword);
        }

        private void ButtonCancelDialogHost_Click(object sender, RoutedEventArgs e)
        {
            AcceptButtonIsPressed = false;
            ButtonCancelDialogHost.Command.Execute(null);

        }


        private void ButtonAcceptDialogHost_Click(object sender, RoutedEventArgs e)
        {
            AcceptButtonIsPressed = true;
            ButtonAcceptDialogHost.Command.Execute(null);

        }

        //When te DIalogHostLogIn is closing, execute the followiing code
        private void DialogHostLogIn_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            //if the AcceptButton was pressed, do the folowing else do the other
            if (AcceptButtonIsPressed)
            {
                var message = "Succesfully logged in!";
                SnackbarMessageDisplay(message,2000);
            }
            else
            {
                TextBoxNameInput.Text = ""; //reset TextBoxNameInput
                var message = "\"Cancel\" was pressed by the user";
                SnackbarMessageDisplay(message,2000);
            }
            AcceptButtonIsPressed = false; //make sure it's false, whatever it happens
        }

        private void TextBoxUserName_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (TextBoxUserName.Text.Length > MaximumTextCharacters)
            {
                if (!(e.Key == System.Windows.Input.Key.Delete ||
                    e.Key == System.Windows.Input.Key.Back ||
                    e.Key == System.Windows.Input.Key.Left ||
                    e.Key == System.Windows.Input.Key.Right ||
                    e.Key == System.Windows.Input.Key.A && Keyboard.Modifiers == ModifierKeys.Control ||
                    e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control ||
                    e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control ||
                    e.Key == Key.X && Keyboard.Modifiers == ModifierKeys.Control ||
                    e.Key == Key.End || e.Key == Key.Home || e.Key == Key.Tab || e.Key == Key.Enter
                    ))
                {
                    e.Handled = true;
                    //Because of knwon Issue, can not be called multiple times in short timeSpan
                    if (tickCount>2)
                    {
                        
                        var message = "Maximum character  is " + MaximumTextCharacters.ToString() + "!";
                        //var messageQueue = SnackbarDialogHost.MessageQueue;
                        ////find out what this shit does
                        //Task.Factory.StartNew(() => messageQueue.Enqueue(message));


                        var messageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(1000));
                        SnackbarDialogHost.MessageQueue = messageQueue;
                        SnackbarDialogHost.MessageQueue.Enqueue(message);
                        tickCount = 0;
                    }
                }
            }
        }

        private void PasswordBoxUserPassword_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
