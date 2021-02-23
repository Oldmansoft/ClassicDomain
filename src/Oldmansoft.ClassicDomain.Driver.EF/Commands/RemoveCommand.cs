using System;

namespace Oldmansoft.ClassicDomain.Driver.EF.Commands
{
    class RemoveCommand<TDomain> : ICommand
        where TDomain : class
    {
        private readonly Context Context;

        private readonly TDomain Domain;

        public RemoveCommand(Context context, TDomain domain)
        {
            Context = context;
            Domain = domain;
        }

        public bool Execute()
        {
            Context.Set<TDomain>().Remove(Domain);
            return Context.SaveChanges(typeof(TDomain)) > 0;
        }
    }
}
