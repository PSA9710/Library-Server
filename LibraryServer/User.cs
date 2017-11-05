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
