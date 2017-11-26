using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryServer
{
    class Book
    {
        public int ISBN { get; private set; }
        public String BookName { get; private set; }
        public String Author { get; private set; }
        public String Publisher { get; private set; }
        public int No_Copies { get; private set; }
        public String Description { get; private set; }

        public Book(string isbn, string Name, string author, string publisher, string copies, string description)
        {
            ISBN = Convert.ToInt32(isbn);
            BookName = Name;
            Author = author;
            Publisher = publisher;
            No_Copies = Convert.ToInt32(copies);
            Description = description;
        }
    }
}
