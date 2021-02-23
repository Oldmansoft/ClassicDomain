using System;

namespace Oldmansoft.ClassicDomain.Driver.EF.Commands
{
    class ReplaceCommand<TDomain> : ICommand
        where TDomain : class
    {
        private readonly Context Context;

        private readonly TDomain Domain;

        public ReplaceCommand(Context context, TDomain domain)
        {
            Context = context;
            Domain = domain;
        }

        public bool Execute()
        {
            if (Context.Entry(Domain).State != System.Data.Entity.EntityState.Detached) return true;
            var domainToReplace = Context.Set<TDomain>().Find(PrimaryKeyManager.Instance.GetPrimaryKey<TDomain>(Context).Get(Domain));
            Domain.MapTo(domainToReplace);
            return Context.SaveChanges(typeof(TDomain)) > 0;
        }
    }
}
