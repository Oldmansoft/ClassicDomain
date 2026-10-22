using MongoDB.Driver;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core.Commands
{
    class AddCommand<TDomain, TKey> : ICommand
    {
        private readonly TDomain Domain;

        private readonly DbSet<TDomain, TKey> Target;

        public AddCommand(TDomain domain, DbSet<TDomain, TKey> target)
        {
            Domain = domain;
            Target = target;
        }

        public bool Execute()
        {
            Target.QueueWrite(new InsertOneModel<TDomain>(Domain), Domain);
            return true;
        }
    }
}
