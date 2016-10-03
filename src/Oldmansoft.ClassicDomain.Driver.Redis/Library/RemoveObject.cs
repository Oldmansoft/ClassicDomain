using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.Redis.Library
{
    class RemoveObject
    {
        public string _specail { get; set; }

        private RemoveObject()
        {
            _specail = "remove";
        }

        public static readonly string Instance = Serializer.Serialize(new RemoveObject());
    }
}