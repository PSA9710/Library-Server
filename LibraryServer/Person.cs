using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryServer
{
    public abstract class Person
    {
        public String CNP { get; private set; }
        public String Name { get; private set; }
        public String LastName { get; private set; }
        public int AnAbs { get; private set; }
        public List<int> ReservedBooks { get; private set; } = new List<int>();
        public String ProfilePic{get;private set;}
        public void SetName(String name)
        {
            Name = name;
        }

        public void SetLastName(String name)
        {
            LastName = name;
        }

        public void SetCNP(String cnp)
        {
            CNP = cnp;
        }

        public void SetAnAbs(int i)
        {
            AnAbs = i;
        }
        public void SetProfilePic(String s)
        {
            ProfilePic = s;
        }

        public abstract void AddBooks(String carti);
        public abstract void AddBooks(int ISBN);
        public abstract void RemoveBooks(int i);
        //public abstract void UpdateRegisteredUserBooks();

    }
}
