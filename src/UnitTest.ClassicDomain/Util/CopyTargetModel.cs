using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ClassicDomain.Util
{
    class CopyTargetModel
    {
        public string Name { get; set; }

        public TargetType Type { get; set; }
        
        public CopyTargetSubModel[] SubArray { get; set; }

        public List<CopyTargetSubModel> SubList { get; set; }

        public Dictionary<int, CopyTargetSubModel> SubDictionary { get; set; }
    }

    enum TargetType : short
    {
        Normal1,
        Importance1
    }

    class CopyTargetSubModel
    {
        public string Value { get; set; }

        private CopyTargetSubModel() { }

        public static CopyTargetSubModel Create()
        {
            return new CopyTargetSubModel();
        }
    }
}
