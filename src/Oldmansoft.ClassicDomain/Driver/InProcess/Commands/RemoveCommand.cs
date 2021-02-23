namespace Oldmansoft.ClassicDomain.Driver.InProcess.Commands
{
    class RemoveCommand<TDomain, TKey> : ICommand
    {
        private readonly StoreManager<TDomain, TKey> Store;

        private readonly TDomain Domain;

        public RemoveCommand(StoreManager<TDomain, TKey> store, TDomain domain)
        {
            Store = store;
            Domain = domain;
        }

        public bool Execute()
        {
            return Store.Remove(Domain);
        }
    }
}
