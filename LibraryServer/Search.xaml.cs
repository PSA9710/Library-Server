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
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : UserControl
    {
        public Search()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Search Button was pressed");
            if (TextBoxSearch.Text == "")
            {
                Console.WriteLine("TextBoxSearch is empty ... canceling search");
                e.Handled = true;
                return;
            }
            SpawnCard();
            SQL_Querry();
        }

        /// <summary>
        /// Returns the name of the Author
        /// </summary>
        /// <param name="i">the authorsID to search after</param>
        /// <returns>The Name of the Author</returns>
        private String SQL_GetAuthor(int i)
        {
            using (SqlConnection con = new SqlConnection(new BOOKS().SQL_ConnectionString()))
            {
                con.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("select * from Authors where author_ID=@ID");
                String sql = sb.ToString();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@ID", i.ToString());
                    SqlDataReader reader = cmd.ExecuteReader();
                    if(reader.Read())
                    {
                        String S = reader["first_Name"] as String;
                        string p = reader["last_Name"] as string;
                        return S + " " + p;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Gets the Publisher 
        /// </summary>
        /// <param name="i">the id of the publisher</param>
        /// <returns>Returns the Publisher as a string</returns>
        private String SQL_GetPublisher(int i)
        {

        }
        /// <summary>
        /// Search the DB for entries that have author or publisher or bookname in it
        /// </summary>
        private void SQL_Querry()
        {
            using (SqlConnection con = new SqlConnection(new BOOKS().SQL_ConnectionString()))
            {
                Console.WriteLine("Connecting to the database for querrying....initializing");
                con.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("select * from Books where book_name like @name or " +
                    "author_ID in (select author_ID from Authors where first_Name like @name or last_Name like @name)" +
                    " or publisher_ID in (select publisher_ID from Publishers where Name like @name)");
                string sql = sb.ToString();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@name", "%" + TextBoxSearch.Text + "%");
                    try
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            Book bk = new Book();
                            String ISBN = reader.GetInt32(reader.GetOrdinal("ISBN")).ToString();
                            String name = reader["book_name"] as string;
                            String Author = SQL_GetAuthor(reader.GetInt32(reader.GetOrdinal("author_ID")));
                            String Publisher = SQL_GetPublisher(reader.GetInt32(reader.GetOrdinal("publisher_ID")));
                            String Description = reader["description"] as string;
                            String NoCopies = reader.GetInt32(reader.GetOrdinal("no_of_copies")).ToString();
                            bk = new Book(ISBN, name, Author, Publisher, NoCopies, Description);

                        }
                    }
                    catch(SqlException ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                con.Close();
            }

        }
        BookCard bk;
        /// <summary>
        /// Represents the amount of cards it has to spawn //helpfull for identyfing the object
        /// </summary>
        static int numberOfCard = 1;
        private void SpawnCard()
        {
            bk = new BookCard();
            Console.WriteLine("Inserting new Card...");
            bk.Name = "BookCard" + numberOfCard.ToString();
            numberOfCard++;
            bk.Margin = new Thickness(20);
            WrapPanelDisplayCards.Children.Add(bk);
        }

        private void TextBoxSearch_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (TextBoxSearch.Text == "")
                {
                    Console.WriteLine("Enter was pressed, but search text box is null.....aborting....");
                    e.Handled = true;
                    return;
                }
                Button_Click(new object(), new RoutedEventArgs());
            }
        }
    }
}
