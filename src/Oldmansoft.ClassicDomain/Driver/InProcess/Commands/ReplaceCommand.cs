using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.InProcess.Commands
{
    class ReplaceCommand<TDomain, TKey> : ICommand
    {
        private StoreManager<TDomain, TKey> Store;

        private TDomain Domain;

        public ReplaceCommand(StoreManager<TDomain, TKey> store, TDomain domain)
        {
            Store = store;
            Domain = domain;
        }

        public Type Type
        {
            get
            {
                return typeof(TDomain);
            }
        }

        public bool Execute()
        {
            return Store.Replace(Domain);
        }
    }
}
