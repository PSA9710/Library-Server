using System;
using System.Collections.Generic;
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
            //throw new NotImplementedException();
        }

        public override void RemoveBooks(int i )
        {
            ReservedBooks.Remove(i);
        }
        
    }
}
