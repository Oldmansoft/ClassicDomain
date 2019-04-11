using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ClassicDomain.Util
{
    class ItemObject
    {
        public Dictionary<string, string> Dic { get; set; }

        public string[] Array { get; set; }

        public List<string> List { get; set; }

        public CopySourceModel Model { get; set; }

        public E? NullEnum { get; set; }

        public E Enum { get; set; }

        public enum E
        {
            A,
            B
        }
    }

    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
}
