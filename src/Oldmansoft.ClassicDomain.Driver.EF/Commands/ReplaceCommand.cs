using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.EF.Commands
{
    class ReplaceCommand<TDomain> : ICommand
        where TDomain : class
    {
        private Context Context;

        private TDomain Domain;

        public ReplaceCommand(Context context, TDomain domain)
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
            if (Context.Entry(Domain).State != System.Data.Entity.EntityState.Detached) return true;
            var domainToReplace = Context.Set<TDomain>().Find(PrimaryKeyManager.Instance.GetPrimaryKey<TDomain>(Context).Get(Domain));
            Domain.MapTo(domainToReplace);
            return Context.SaveChanges(Type) > 0;
        }
    }
}
