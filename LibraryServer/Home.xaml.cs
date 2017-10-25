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
        }

        //Recieves a string, and pops a snackbarnotification with the recieved string
        private void SnackbarMessageDisplay(string s)
        {
            //SnackbarMaximumCharacters.IsActive = true;
            var messageQueue = SnackbarMaximumCharacters.MessageQueue;


            //find out what this shit does
            Task.Factory.StartNew(() => messageQueue.Enqueue(s));
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
                    e.Key == Key.End || e.Key == Key.Home
                    ))
                {
                    e.Handled = true;
                    var message = "Maximum character limit is " + MaximumTextCharacters.ToString() + "!";
                    SnackbarMessageDisplay(message);
                }
            }
            if (e.Key == Key.Enter)
            {
                if (TextBoxNameInput.Text == "")
                {
                    SnackbarMessageDisplay("A username is required in order to login");
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
                SnackbarMessageDisplay(message);
            }
        }
    }
}
