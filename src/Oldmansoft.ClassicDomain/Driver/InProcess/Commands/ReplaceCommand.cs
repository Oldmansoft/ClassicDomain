namespace Oldmansoft.ClassicDomain.Driver.InProcess.Commands
{
    class ReplaceCommand<TDomain, TKey> : ICommand
    {
        private readonly StoreManager<TDomain, TKey> Store;

        private TDomain Domain;

        public ReplaceCommand(StoreManager<TDomain, TKey> store, TDomain domain)
        {
            Store = store;
            Domain = domain;
        }

        public bool Execute()
        {
            return Store.Replace(Domain);
        }
    }
}
