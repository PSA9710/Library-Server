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
    /// Interaction logic for ListBooks.xaml
    /// </summary>
    public partial class ListBooks : UserControl
    {
        public ListBooks()
        {
            InitializeComponent();
        }

        private User AppUser;
        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Console.WriteLine("ListBooks UserControl is Visible..");
            if (this.Visibility == Visibility.Visible)
            {
                if (AppUser.ReservedBooks.Count == 0)
                {
                    Console.WriteLine("User has no booked books");
                    return;
                }
                WrapPanelDisplayCards.Children.Clear();
                Console.WriteLine("Listing Books...");
                SQL_Querry(BuildBooksString());
            }
        }

        public void UpdateDisplayAfterDelete()
        {
            WrapPanelDisplayCards.Children.Clear();
            SQL_Querry(BuildBooksString());
        }

        public void SetUser(User A)
        {
            AppUser = A;

        }

        private string BuildBooksString()
        {
            string s = null;

            s = "(";
            foreach(int book in AppUser.ReservedBooks)
            {
                s += "'" + book + "',";
            }
            s = s.Remove(s.Length - 1);
            s += ")";
            Console.WriteLine("The string of books to show:" + s);
            return s;
        }

        /// <summary>
        /// Search the DB for entries that have author or publisher or bookname in it
        /// </summary>
        private void SQL_Querry(String BOOKS)
        {

            using (SqlConnection con = new SqlConnection(new BOOKS().SQL_ConnectionString()))
            {
                Console.WriteLine("Connecting to the database for querrying....initializing");
                con.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("select * from Books where ISBN in"+BOOKS) ;
                string sql = sb.ToString();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    //cmd.Parameters.AddWithValue("@name", "%" + TextBoxSearch.Text + "%");
                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                            while (reader.Read())
                            {
                                Book bk = new Book();
                                int ISBN = reader.GetInt32(reader.GetOrdinal("ISBN"));
                                String name = reader["book_name"] as string;
                                String Author = SQL_GetAuthor(reader.GetInt32(reader.GetOrdinal("author_ID")));
                                //String Publisher = SQL_GetPublisher(reader.GetInt32(reader.GetOrdinal("publisher_ID")));
                                String Description = reader["description"] as string;
                                String NoCopies = reader.GetInt32(reader.GetOrdinal("no_of_copies")).ToString();
                                if (Convert.ToInt32(NoCopies) >= 0)
                                {
                                    bk = new Book(ISBN.ToString(), name, Author, null, NoCopies, Description);
                                    SpawnCard(bk);
                                }
                            }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                con.Close();

            }
        }
        BookCard bk1;
        /// <summary>
        /// Represents the amount of cards it has to spawn //helpfull for identyfing the object
        /// </summary>
        static int numberOfCard = 1;
        private void SpawnCard(Book bk)
        {
            bk1 = new BookCard(bk,true);
            Console.WriteLine("Inserting new Card...");
            bk1.Name = "BookCard" + numberOfCard.ToString();
            numberOfCard++;
            bk1.Margin = new Thickness(20);
            WrapPanelDisplayCards.Children.Add(bk1);
        }

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
                    if (reader.Read())
                    {
                        String S = reader["first_Name"] as String;
                        string p = reader["last_Name"] as string;
                        return S + " " + p;
                    }
                }
            }
            return null;
        }
    }
}
