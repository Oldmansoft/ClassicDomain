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

        public string Name { get; private set; }

        private Person() { }

        public static Person Create(string name)
        {
            var result = new Person();
            result.Name = name;
            return result;
        }

        public void Change(string name)
        {
            Name = name;
        }
    }
}
