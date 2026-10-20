using System;
using System.Data.Entity;

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
            var entry = Context.Entry(Domain);
            if (entry.State != EntityState.Detached) return false;

            var key = PrimaryKeyManager.Instance.GetPrimaryKey<TDomain>(Context);
            var keyValue = key.Get(Domain);
            TDomain tracked = null;
            foreach (var local in Context.Set<TDomain>().Local)
            {
                if (Equals(key.Get(local), keyValue))
                {
                    tracked = local;
                    break;
                }
            }
            if (tracked != null)
            {
                if (!ReferenceEquals(tracked, Domain)) Domain.MapTo(tracked);
                return false;
            }

            Context.AttachForCommit(typeof(TDomain), () =>
            {
                Context.Set<TDomain>().Attach(Domain);
                var attached = Context.Entry(Domain);
                foreach (var name in attached.CurrentValues.PropertyNames)
                {
                    attached.Property(name).IsModified = true;
                }
            });
            return false;
        }
    }
}
