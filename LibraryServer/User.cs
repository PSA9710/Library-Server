using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryServer
{
    class User : Person
    {
        public bool isTeacher { get; private set; }

        User(String Nume,String Prenume,bool isteacher)
        {
            isTeacher = isteacher;
            setLastName(Nume);
            setName(Prenume);
        }




        public void setTeacher(bool isteacher)
        {
            isTeacher = isteacher;
        }

        public override void AddBooks()
        {
            throw new NotImplementedException();
        }

        public override void RemoveBooks()
        {
            throw new NotImplementedException();
        }
    }
}
