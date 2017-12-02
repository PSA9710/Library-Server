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

        Book book;

        public BOOKS()
        {
            InitializeComponent();
            for (int i = 1; i < 11; i++)
                ComboBoxCopies.Items.Add(i);
        }

        private void PopulateBook()
        {
            book = new Book(
                TextBoxISBN.Text,
                TextBoxBookName.Text,
                TextBoxAuthor.Text,
                TextBoxPublisher.Text,
                ComboBoxCopies.SelectedValue.ToString(),
                TextBoxDescription.Text
                );
            Console.WriteLine("book instance created succesfully");
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
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9) && !(new Home().GetPressedKey(e))) e.Handled = true;
            if (e.Key == Key.Enter)
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
            builder.DataSource = "tcp:libraryoop.database.windows.net,1433";
            builder.UserID = "Library";
            builder.Password = "Aa123456789";
            builder.InitialCatalog = "Library";

            Console.WriteLine("Conecting to SQL server.....");
            return builder.ConnectionString;
        }

        /// <summary>
        /// Search in the database for book after ISBN
        /// </summary>
        private void SQL_ISBN_Search()
        {
            int idA = -1, idP = -1;
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
                if (idA != -1 && idP != -1)
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
                                TextBoxPublisher.Text = a;
                            }
                        }
                    }
                }
                con.Close();
                if (EntryFound)
                {
                    ButtonSave.Content = "Save";
                    PopulateBook();
                    TextBoxISBN.IsEnabled = false;
                }
                else
                {
                    ButtonSave.Content = "ADD";
                }
            }
        }


        private void SQL_RegisterAuthor()
        {
            Console.WriteLine("Author not found , creating entry....");
            string t = TextBoxAuthor.Text;
            string[] names = t.Split(' ');
            using (SqlConnection con = new SqlConnection(SQL_ConnectionString()))
            {

                con.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("insert into Authors (first_Name,last_Name) values ('" + names[0] + "','" + names[1] + "');");
                String sql = sb.ToString();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch(SqlException ex)
                    {
                        MessageBox.Show(ex.ToString());
                        MessageBox.Show("Error while registering author,");
                    }
                }
                con.Close();
            }
            Console.WriteLine("Entry created");
        }

        /// <summary>
        /// Get the author ID, if non existent create one
        /// </summary>
        /// <returns>returns the author_ID to be inserted into the DB</returns>
        private int SQL_GetAuthorID()
        {
            int id = -1;
            string t = TextBoxAuthor.Text;
            string[] names = t.Split(' ');
            if (names.Length > 2)
            {
                Console.WriteLine("Too many spaces in author textbox");
                MessageBox.Show("Only 1 Space allowed");
                TextBoxAuthor.Focus();
                return -1;
            }

            using (SqlConnection con = new SqlConnection(SQL_ConnectionString()))
            {

                con.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("select author_ID from Authors where first_Name=@first and last_Name=@last");
                String sql = sb.ToString();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@first", names[0]);
                    cmd.Parameters.AddWithValue("@last", names[1]);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id = reader.GetInt32(0);
                            MessageBox.Show(id.ToString());
                        }
                    }
                }
                con.Close();
            }
            if (id == -1)
            {
                SQL_RegisterAuthor();
                return -9;
            }

            return id;
        }


        /// <summary>
        /// Modifies a entry in the database
        /// </summary>
        private void SQL_BOOK_modify()
        {
            int aID = -1, pID = -1;
            if (TextBoxAuthor.Text != book.Author)
            {
                Console.WriteLine("Author changed , attempting to get new ID");
                aID = SQL_GetAuthorID();
                if (aID == -9)
                    aID = SQL_GetAuthorID();
            }
            if (TextBoxPublisher.Text != book.Publisher)
            {
                Console.WriteLine("Publisher changed, attemtping to get new PublisherID");
                pID = SQL_GetPublisherID();
                if (pID == -9)
                    pID = SQL_GetPublisherID();
            }
            if (aID == -1 || pID == -1) return;

            using (SqlConnection con = new SqlConnection(SQL_ConnectionString()))
            {
                con.Open();
                Console.WriteLine("Creating SQL Querry for Book Update");

                StringBuilder sb = new StringBuilder();
                sb.Append("update Books set " +
                    "book_name='" + TextBoxBookName.Text + "'," +
                    "author_ID=" + aID.ToString() + "," +
                    "publisher_ID=" + pID.ToString() + "," +
                    "no_of_copies=" + ComboBoxCopies.SelectedValue.ToString() + "," +
                    "description='" + TextBoxDescription.Text + "'" +
                    "where ISBN=" + TextBoxISBN.Text + ";"
                    );
                String sql = sb.ToString();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch { MessageBox.Show("Error while updating"); }
                }

            }
        }

        private void SQL_RegisterPublisher()
        {
            Console.WriteLine("Publisher not found , creating entry....");
            using (SqlConnection con = new SqlConnection(SQL_ConnectionString()))
            {

                con.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("insert into Publishers (Name) values ('" + TextBoxPublisher.Text + "');");
                String sql = sb.ToString();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        MessageBox.Show("Error while registering publisher,");
                    }
                }
                con.Close();
            }
            Console.WriteLine("Entry created");
        }

        /// <summary>
        /// Get the author ID, if non existent create one
        /// </summary>
        /// <returns>returns the author_ID to be inserted into the DB</returns>
        private int SQL_GetPublisherID()
        {
            int id = -1;

            using (SqlConnection con = new SqlConnection(SQL_ConnectionString()))
            {

                con.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("select publisher_ID from Publishers where Name=@name");
                String sql = sb.ToString();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@name", TextBoxPublisher.Text);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id = reader.GetInt32(0);
                            MessageBox.Show(id.ToString());
                        }
                    }
                }
                con.Close();
            }
            if (id == -1)
            {
                SQL_RegisterPublisher();
                return -9;
            }

            return id;
        }

        private void SQL_BOOK_insert()
        {

            int aID = -1, pID = -1;
            //if (TextBoxAuthor.Text != book.Author)
            {
                Console.WriteLine("Author  , attempting to get new ID");
                aID = SQL_GetAuthorID();
                if (aID == -9)
                    aID = SQL_GetAuthorID();
            }
            //if (TextBoxPublisher.Text != book.Publisher)
            {
                Console.WriteLine("Publisher , attemtping to get new PublisherID");
                pID = SQL_GetPublisherID();
                if (pID == -9)
                    pID = SQL_GetPublisherID();
            }
            if (aID == -1 || pID == -1) return;

            using (SqlConnection con = new SqlConnection(SQL_ConnectionString()))
            {
                con.Open();
                Console.WriteLine("Creating SQL Querry for Book insert");

                StringBuilder sb = new StringBuilder();
                sb.Append("insert into Books (ISBN,book_name,author_ID,publisher_ID,no_of_copies,description)" +
                    " values('" + TextBoxISBN.Text + "','" + TextBoxBookName.Text + "'," + aID.ToString() + "," + pID.ToString()
                    + "," + ComboBoxCopies.SelectedValue.ToString() + ",'" + TextBoxDescription.Text + "');");

                String sql = sb.ToString();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch { MessageBox.Show("Error while attempting to insert"); }
                }

            }
        }


        #endregion

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (EntryFound)
            {
                Console.WriteLine("Modifing entry in the table for ISBN " + TextBoxISBN.Text);
                SQL_BOOK_modify();
            }
            else
            {
                Console.WriteLine("Inserting new entry in the table with ISBN " + TextBoxISBN.Text);
                SQL_BOOK_insert();
            }

        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Erasing all entries...enabling ISBN textbox");
            TextBoxISBN.IsEnabled = true;
            TextBoxISBN.Text = "";
            TextBoxDescription.Text = "";
            TextBoxBookName.Text = "";
            TextBoxPublisher.Text = "";
            TextBoxAuthor.Text = "";
            ComboBoxCopies.SelectedIndex = -1;
        }
    }
}
