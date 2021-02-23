using System;

namespace Oldmansoft.ClassicDomain.Driver.EF.Commands
{
    class ActionCommand<TDomain> : ICommand
        where TDomain : class
    {
        private readonly Context Context;

        private readonly Func<Context, bool> Action;

        public ActionCommand(Context context, Func<Context, bool> action)
        {
            Context = context;
            Action = action;
        }

        public bool Execute()
        {
            return Action(Context);
        }
    }
}
