using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ClassicDomain.Drvier.Domain
{
    class Book
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Sex { get; set; }

        public byte[] Binary { get; set; }

        public List<Author> Authors { get; set; }

        public List<string> Tags { get; set; }

        public Dictionary<string, string> DictionaryValue { get; set; }
    }
}
