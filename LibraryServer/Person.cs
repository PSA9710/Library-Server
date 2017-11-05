using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryServer
{
    abstract class Person
    {
        public String Name { get; private set; }
        public String LastName { get; private set; }


        public abstract void AddBooks();
        public abstract void RemoveBooks();

    }
}
