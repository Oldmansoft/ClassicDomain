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

        public void SetName(string name)
        {
            Name = name;
        }

        public void CreateSub()
        {
            Sub = CopySourceSubModel.Create();
        }
    }

    class CopySourceSubModel
    {
        public string Value { get; set; }

        private CopySourceSubModel() { }

        public static CopySourceSubModel Create()
        {
            return new CopySourceSubModel();
        }
    }
}
