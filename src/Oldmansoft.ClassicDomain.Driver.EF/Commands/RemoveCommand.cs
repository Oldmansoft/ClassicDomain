using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.EF.Commands
{
    class RemoveCommand<TDomain> : ICommand
        where TDomain : class
    {
        private Context Context;

        private TDomain Domain;

        public RemoveCommand(Context context, TDomain domain)
        {
            Context = context;
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
            Context.Set<TDomain>().Remove(Domain);
            return Context.SaveChanges(Type) > 0;
        }
    }
}
