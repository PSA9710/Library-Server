using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryServer
{
    public class User : Person
    {
        public bool isTeacher { get; private set; }

        User(String Nume, String Prenume, bool isteacher)
        {
            isTeacher = isteacher;
            SetLastName(Nume);
            SetName(Prenume);
            
        }

        public User()
        {

        }





        public void SetTeacher(bool isteacher)
        {
            isTeacher = isteacher;
        }

        public override void AddBooks(String cartii)
        {   if (cartii == null) return;
            Console.WriteLine("The books are:" + cartii);
            String[] carti = cartii.Split(',');
            foreach (String carte in carti)
            {
                ReservedBooks.Add(Convert.ToInt32(carte));
            }
            
        //    throw new NotImplementedException();
        }

        public override void AddBooks(int ISBN)
        {
            //ReservedBooks.Add(5);
            ReservedBooks.Add(ISBN);
            Console.WriteLine("Reserved book with ISBN" + ISBN.ToString());
            UpdateRegisteredUserBooks();
            //throw new NotImplementedException();
        }

        public override void RemoveBooks(int i )
        {
            String books = null;
            Console.WriteLine("Removing book with ISBN:" + i.ToString() + " from list");
            if (ReservedBooks.Count == 0) { Console.WriteLine("There is no book in order to create the update"); return; }
            ReservedBooks.Remove(i);
            //foreach (int book in ReservedBooks)
            //{
            //    books += book + ",";
            //}
            //if(books!="")
            //books = books.Remove(books.Length - 1);
            //Console.WriteLine("The String with registered books:" + books);

            UpdateRegisteredUserBooks();
        }

        private void UpdateRegisteredUserBooks()
        {
            String books = null;
            Console.WriteLine("Creating the string of books");
            if (ReservedBooks.Count != 0)
            {
                foreach (int book in ReservedBooks)
                {
                    books += book + ",";
                }

                books = books.Remove(books.Length - 1);
            }
            Console.WriteLine("The String with registered books:" + books);
            
                if (isTeacher)
                    UpdateTeacherBooks(books);
                else
                    UpdateStudentBooks(books);
          
         //   throw new NotImplementedException();
        }

        private void UpdateTeacherBooks(String books)
        {
            Console.WriteLine("Updating list for the teacher");
            try
            {
                using (SqlConnection con = new SqlConnection(new BOOKS().SQL_ConnectionString()))
                {
                    Console.WriteLine("Attempting to establish connection");
                    con.Open();
                    Console.WriteLine("Connection established");
                    String sql = "update Librarians Set BookList=@value where CNP=" + CNP;
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@value", (object)books ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("List updated succesfuly for the user");
                    }
                }
            }
            catch(SqlException e)
            {
                Console.WriteLine(e);
            }

        }

        private void UpdateStudentBooks(String books)
        {
            Console.WriteLine("Updating list for the student");
            try
            {
                using (SqlConnection con = new SqlConnection(new BOOKS().SQL_ConnectionString()))
                {
                    Console.WriteLine("Attempting to establish connection");
                    con.Open();
                    Console.WriteLine("Connection established");
                    String sql = "update Students Set BookList=@value where CNP=" + CNP;
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@value", (object)books?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("List updated succesfuly for the user");
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
            }
        }
            
    }
}
