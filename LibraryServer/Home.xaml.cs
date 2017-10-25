using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace LibraryServer
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
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
        { int time = DateTime.Parse(DateTime.Now.ToString("HHtt")).Hour;
            if (time < 12) LabelGreeting.Content = "Good Morning,";
            else if (time < 17) LabelGreeting.Content = "Good Afternoon,";
            else if (time < 24) LabelGreeting.Content = "Good Evening,";
            
        }

        //functie ce updateaza timpul in aplicatie
        private void TimerTick(Object sender, EventArgs e)
        {
            //Set content of Label to current time in a HH:MM format
            LabelTimer.Content = DateTime.Now.ToString("hh:mm tt");
        }

        private void TextBoxNameInput_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (TextBoxNameInput.Text.Length == 15)
            {
                //if one of the following keys is pressed, ignore the event
                if (!(e.Key == System.Windows.Input.Key.Delete ||
                    e.Key == System.Windows.Input.Key.Back ||
                    e.Key == System.Windows.Input.Key.Left ||
                    e.Key == System.Windows.Input.Key.Right)
                    )
                {
                    e.Handled = true;
                    //SnackbarMaximumCharacters.IsActive = true;
                    var messageQueue = SnackbarMaximumCharacters.MessageQueue;
                    var message = "Maximum character limit is 15!";

                    //find out what this shit does
                    Task.Factory.StartNew(() => messageQueue.Enqueue(message));
                }
            }
        }


    }
}
