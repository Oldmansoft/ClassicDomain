namespace Oldmansoft.ClassicDomain.Driver.Mongo.Core
{
    interface IFlushWrites
    {
        int FlushWrites();

        void AbandonWrites();
    }
}
