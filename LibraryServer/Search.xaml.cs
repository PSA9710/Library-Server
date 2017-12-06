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
           // SQL_Querry();
        }

        private void SQL_Querry()
        {
            using (SqlConnection con = new SqlConnection(new BOOKS().SQL_ConnectionString()))
            {
                Console.WriteLine("Connecting to the database for querrying....initializing");
                con.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("select * from Books where ");
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
    }
}
