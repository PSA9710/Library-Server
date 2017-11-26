using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for BOOKS.xaml
    /// </summary>
    public partial class BOOKS : UserControl
    {
        public BOOKS()
        {
            InitializeComponent();
            for (int i = 1; i < 11; i++)
                ComboBoxCopies.Items.Add(i);
        }

        #region SnackBar
        /// <summary>
        /// Display various messages on Snackbar
        /// </summary>
        /// <param name="message">What message to be displayed</param>
        /// <param name="timeSpan">the time to be displayed in milliseconds</param>
        private void SnackBarDisplay(string message, int timeSpan)
        {
            if (MainWindow.tickCount < 1 + timeSpan / 1000) return;
            SnackbarDisplay.MessageQueue = new MaterialDesignThemes.Wpf.SnackbarMessageQueue(TimeSpan.FromMilliseconds(timeSpan));
            SnackbarDisplay.MessageQueue.Enqueue(message);
            MainWindow.tickCount = 0;
        }
        #endregion

        #region TextBoxISBN

        private void TextBoxISBN_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!CheckStringOnlyNumbers(TextBoxISBN.Text))
            {
                TextBoxISBN.Text = "";
                var message = "ISBN must contain only numbers!";
                SnackBarDisplay(message, 1000);
                e.Handled = true;
                TextBoxISBN.Focus();
            }

        }

        private void TextBoxISBN_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9)&&!(new Home().GetPressedKey(e))) e.Handled = true;
            if(e.Key==Key.Enter)
            {
                if (!CheckStringOnlyNumbers(TextBoxISBN.Text))
                {
                    TextBoxISBN.Text = "";
                    var message = "ISBN must contain only numbers!";
                    SnackBarDisplay(message, 1000);
                    e.Handled = true;
                }
                else
                {
                    SQL_ISBN_Search();
                }
            }

        }
        /// <summary>
        /// Checks if the Text is made only out of numers
        /// </summary>
        /// <param name="text">A string must be passed for checking</param>
        /// <returns>True for numbers, false if it contains other characters</returns>
        private bool CheckStringOnlyNumbers(string text)
        {
            if (text == "") return false;
            //if password contains characters reset password, and send error notification
            if (text.All(char.IsDigit)) return true;
            return false;
        }
        #endregion

        #region SQL_Functions
        /// <summary>
        /// Variable that stores if a book is in the database
        /// </summary>
        private bool EntryFound = false;


        private String SQL_ConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "localhost";
            builder.UserID = "Library";
            builder.Password = "1234";
            builder.InitialCatalog = "Library";

            Console.WriteLine("Conecting to SQL server.....");
            return builder.ConnectionString;
        }

        /// <summary>
        /// Search in the database for book after ISBN
        /// </summary>
        private void SQL_ISBN_Search()
        {
            int idA=-1, idP=-1;
            using (SqlConnection con = new SqlConnection(SQL_ConnectionString()))
            {
                con.Open();
                Console.WriteLine("Connection Succesful...Attempting ISBN query for " + TextBoxISBN.Text.ToString());


                StringBuilder sb = new StringBuilder();
                sb.Append("select * from Books where ISBN=" + TextBoxISBN.Text.ToString());
                String sql = sb.ToString();


                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {//populare ca lumea
                            Console.WriteLine("Entry found...populating");
                            EntryFound = true;
                            TextBoxBookName.Text = reader["book_name"] as string;
                            int i = reader.GetInt32(reader.GetOrdinal("no_of_copies"));
                            ComboBoxCopies.SelectedIndex = i - 1;
                            TextBoxDescription.Text = reader["description"] as string;
                            idA = reader.GetInt32(reader.GetOrdinal("author_ID"));
                            idP = reader.GetInt32(reader.GetOrdinal("publisher_ID"));
                        }
                    }
                }
                if (idA !=-1 && idP!=-1)
                {
                    Console.WriteLine("Initializing querry for Author");
                    sb.Clear();
                    sb.Append("select * from Authors where author_ID=" + idA.ToString());
                    sql = sb.ToString();

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string a, b;
                                a = reader["first_Name"] as string;
                                b = reader["last_Name"] as string;
                                MessageBox.Show(a + b);
                                TextBoxAuthor.Text = a + " " + b;
                            }
                        }
                    }

                    Console.WriteLine("Initializing querry for Publisher");
                    sb.Clear();
                    sb.Append("select * from Publishers where publisher_ID=" + idP.ToString());
                    sql = sb.ToString();

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string a;
                                a = reader["Name"] as string;
                                TextBoxPublisher.Text = a ;
                            }
                        }
                    }
                }

                if(EntryFound)
                {
                    ButtonSave.Content = "Save";
                }
                else
                {
                    ButtonSave.Content = "ADD";
                }
            }
        }

        /// <summary>
        /// Modifies a entry in the database
        /// </summary>
        private void SQL_BOOK_modify()
        {

        }

        #endregion

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if(EntryFound)

        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
