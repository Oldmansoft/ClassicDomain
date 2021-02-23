using System;
using System.Collections.Generic;
using System.Text;

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
