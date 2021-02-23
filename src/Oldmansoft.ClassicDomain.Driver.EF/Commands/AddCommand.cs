using System;

namespace Oldmansoft.ClassicDomain.Driver.EF.Commands
{
    class AddCommand<TDomain> : ICommand
        where TDomain : class
    {
        private readonly Context Context;

        private readonly TDomain Domain;

        public AddCommand(Context context, TDomain domain)
        {
            Context = context;
            Domain = domain;
        }

        public bool Execute()
        {
            Context.Set<TDomain>().Add(Domain);
            return Context.SaveChanges(typeof(TDomain)) > 0;
        }
    }
}
