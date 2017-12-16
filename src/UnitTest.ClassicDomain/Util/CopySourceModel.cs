using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ClassicDomain.Util
{
    class CopySourceModel
    {
        public string Name { get; private set; }

        public CopySourceSubModel Sub { get; set; }

        public SourceType Type { get; set; }

        public CopySourceSubModel[] SubArray { get; set; }

        public List<CopySourceSubModel> SubList { get; set; }

        public Dictionary<int , CopySourceSubModel> SubDictionary { get; set; }

        public void SetName(string name)
        {
            Name = name;
        }

        public void CreateSub()
        {
            Sub = CopySourceSubModel.Create(string.Empty);
        }
    }

    class CopySourceSubModel
    {
        public string Value { get; set; }

        private CopySourceSubModel() { }

        public static CopySourceSubModel Create(string value)
        {
            var result = new CopySourceSubModel();
            result.Value = value;
            return result;
        }
    }

    enum SourceType : byte
    {
        Normal,
        Importance
    }
}
