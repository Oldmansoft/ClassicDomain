namespace Oldmansoft.ClassicDomain.Driver.InProcess.Commands
{
    class AddCommand<TDomain, TKey> : ICommand
    {
        private readonly StoreManager<TDomain, TKey> Store;

        private readonly TDomain Domain;

        public AddCommand(StoreManager<TDomain, TKey> store, TDomain domain)
        {
            Store = store;
            Domain = domain;
        }

        public bool Execute()
        {
            return Store.Add(Domain);
        }
    }
}
