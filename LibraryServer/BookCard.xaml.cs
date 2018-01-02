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
        public BookCard(Book b, bool remove)
        {
            book = b;
            InitializeComponent();
            TextBlockBookTitle.Text = book.BookName;
            TextBlockAuthor.Text = "written by " + book.Author;
            TextBlockDescription.Text = "   " + book.Description;
            ButtonFindOutMore.Height = 0;
            ButtonFindOutMore.Visibility = Visibility.Hidden;
            ButtonRemove.Height = 32;
            ButtonRemove.Visibility = Visibility.Visible;
        }

        private void ButtonFindOutMore_Click(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show(this.Name.ToString());
         //   MessageBox.Show(book.ISBN.ToString());
            var target = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;
            Console.WriteLine("Taking Book with ISBN:" + book.ISBN.ToString());
            target.AddBookToUser(book.ISBN);
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            var target = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;
            Console.WriteLine("Taking Book with ISBN:" + book.ISBN.ToString());
            target.RemoveBookFromUser(book.ISBN);
        }
    }
}
