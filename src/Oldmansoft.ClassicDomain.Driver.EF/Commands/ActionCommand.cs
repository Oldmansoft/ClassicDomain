using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oldmansoft.ClassicDomain.Driver.EF.Commands
{
    class ActionCommand<TDomain> : ICommand
        where TDomain : class
    {
        private Context Context;

        private Func<Context, bool> Action;

        public ActionCommand(Context context, Func<Context, bool> action)
        {
            Context = context;
            Action = action;
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
            return Action(Context);
        }
    }
}
