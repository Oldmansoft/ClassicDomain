using System;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    internal class MongoServer : MongoDB.Driver.MongoServer
    {
        public MongoServer(MongoDB.Driver.MongoServerSettings settings)
            : base(settings)
        {
        }

        public override MongoDB.Driver.MongoDatabase GetDatabase(string databaseName, MongoDB.Driver.MongoDatabaseSettings databaseSettings)
        {
            if (databaseName == null)
            {
                throw new ArgumentNullException("databaseName");
            }
            if (databaseSettings == null)
            {
                throw new ArgumentNullException("databaseSettings");
            }
            return new MongoDatabase(this, databaseName, databaseSettings);
        }
    }
}
