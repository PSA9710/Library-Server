using System;
using System.Windows.Controls;
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
            DispatcherTimer ActualTime = new DispatcherTimer();
            ActualTime.Interval = TimeSpan.FromSeconds(1); //set the interval when the ticks will ocur
            ActualTime.Tick += TimerTick;   //Add TimerTick to fired events
            ActualTime.Start();

            
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
                e.Handled = true;
                SnackbarMaximumCharacters.IsActive = true;
            }
        }

        private void SnackbarMessage_ActionClick(object sender, System.Windows.RoutedEventArgs e)
        {
            SnackbarMaximumCharacters.IsActive = false;
        }
    }
}
