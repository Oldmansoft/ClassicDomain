using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.ConsoleApplication.Domain
{
    public class Person
    {
        public Guid Id { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        private Person() { }

        public static Person Create(string firstName, string lastName)
        {
            var result = new Person();
            result.FirstName = firstName;
            result.LastName = lastName;
            return result;
        }

        public void Change(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
