namespace Oldmansoft.ClassicDomain.Driver.Redis.Core
{
    interface IRedisFlush
    {
        void Enqueue(Library.RedisPendingWrite write);

        void Send();
    }
}
