using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var id = new Applications.Persons().Add(new Data.PersonData() { Name = "Oldman" });
            new Applications.Persons().Edit(new Data.PersonData() { Id = id, Name = "Oldmansoft" });
            new Applications.Persons().Remove(id);
        }
    }
}
