using System;
using System.Collections.Generic;
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
    /// Interaction logic for BookCard.xaml
    /// </summary>
    public partial class BookCard : UserControl
    {

        Book book;

        public BookCard()
        {
            InitializeComponent();
        }

        public BookCard(Book b)
        {
            book = b;
            InitializeComponent();
            TextBlockBookTitle.Text = book.BookName;
            TextBlockAuthor.Text = "written by " + book.Author;
            TextBlockDescription.Text = "   " + book.Description;
        }

        private void ButtonFindOutMore_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this.Name.ToString());
        }
    }
}
