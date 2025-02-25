using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Search;

namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    class CollectionProxy<TDocument> : IMongoCollection<TDocument>
    {
        private readonly IMongoCollection<TDocument> Source;

        public CollectionProxy(IMongoCollection<TDocument> source)
        {
            Source = source;
        }

        CollectionNamespace IMongoCollection<TDocument>.CollectionNamespace => Source.CollectionNamespace;

        IMongoDatabase IMongoCollection<TDocument>.Database => Source.Database;

        IBsonSerializer<TDocument> IMongoCollection<TDocument>.DocumentSerializer => Source.DocumentSerializer;

        IMongoIndexManager<TDocument> IMongoCollection<TDocument>.Indexes => Source.Indexes;

        IMongoSearchIndexManager IMongoCollection<TDocument>.SearchIndexes => Source.SearchIndexes;

        MongoCollectionSettings IMongoCollection<TDocument>.Settings => Source.Settings;

        IAsyncCursor<TResult> IMongoCollection<TDocument>.Aggregate<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options, CancellationToken cancellationToken)
        {
            return new CursorProxy<TResult>(Source.Aggregate(pipeline, options, cancellationToken));
        }

        IAsyncCursor<TResult> IMongoCollection<TDocument>.Aggregate<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options, CancellationToken cancellationToken)
        {
            return Source.Aggregate(session, pipeline, options, cancellationToken);
        }

        Task<IAsyncCursor<TResult>> IMongoCollection<TDocument>.AggregateAsync<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options, CancellationToken cancellationToken)
        {
            return Source.AggregateAsync(pipeline, options, cancellationToken);
        }

        Task<IAsyncCursor<TResult>> IMongoCollection<TDocument>.AggregateAsync<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options, CancellationToken cancellationToken)
        {
            return Source.AggregateAsync(session, pipeline, options, cancellationToken);
        }

        void IMongoCollection<TDocument>.AggregateToCollection<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options, CancellationToken cancellationToken)
        {
            Source.AggregateToCollection(pipeline, options, cancellationToken);
        }

        void IMongoCollection<TDocument>.AggregateToCollection<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options, CancellationToken cancellationToken)
        {
            Source.AggregateToCollection(session, pipeline, options, cancellationToken);
        }

        Task IMongoCollection<TDocument>.AggregateToCollectionAsync<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options, CancellationToken cancellationToken)
        {
            return Source.AggregateToCollectionAsync(pipeline, options, cancellationToken);
        }

        Task IMongoCollection<TDocument>.AggregateToCollectionAsync<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options, CancellationToken cancellationToken)
        {
            return Source.AggregateToCollectionAsync(session, pipeline, options, cancellationToken);
        }

        BulkWriteResult<TDocument> IMongoCollection<TDocument>.BulkWrite(IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options, CancellationToken cancellationToken)
        {
            return Source.BulkWrite(requests, options, cancellationToken);
        }

        BulkWriteResult<TDocument> IMongoCollection<TDocument>.BulkWrite(IClientSessionHandle session, IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options, CancellationToken cancellationToken)
        {
            return Source.BulkWrite(session, requests, options, cancellationToken);
        }

        Task<BulkWriteResult<TDocument>> IMongoCollection<TDocument>.BulkWriteAsync(IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options, CancellationToken cancellationToken)
        {
            return Source.BulkWriteAsync(requests, options, cancellationToken);
        }

        Task<BulkWriteResult<TDocument>> IMongoCollection<TDocument>.BulkWriteAsync(IClientSessionHandle session, IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options, CancellationToken cancellationToken)
        {
            return Source.BulkWriteAsync(session, requests, options, cancellationToken);
        }

        [Obsolete]
        long IMongoCollection<TDocument>.Count(FilterDefinition<TDocument> filter, CountOptions options, CancellationToken cancellationToken)
        {
            return Source.Count(filter, options, cancellationToken);
        }

        [Obsolete]
        long IMongoCollection<TDocument>.Count(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options, CancellationToken cancellationToken)
        {
            return Source.Count(session, filter, options, cancellationToken);
        }

        [Obsolete]
        Task<long> IMongoCollection<TDocument>.CountAsync(FilterDefinition<TDocument> filter, CountOptions options, CancellationToken cancellationToken)
        {
            return Source.CountAsync(filter, options, cancellationToken);
        }

        [Obsolete]
        Task<long> IMongoCollection<TDocument>.CountAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options, CancellationToken cancellationToken)
        {
            return Source.CountAsync(session, filter, options, cancellationToken);
        }

        long IMongoCollection<TDocument>.CountDocuments(FilterDefinition<TDocument> filter, CountOptions options, CancellationToken cancellationToken)
        {
            return Source.CountDocuments(filter, options, cancellationToken);
        }

        long IMongoCollection<TDocument>.CountDocuments(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options, CancellationToken cancellationToken)
        {
            return Source.CountDocuments(session, filter, options, cancellationToken);
        }

        Task<long> IMongoCollection<TDocument>.CountDocumentsAsync(FilterDefinition<TDocument> filter, CountOptions options, CancellationToken cancellationToken)
        {
            return Source.CountDocumentsAsync(filter, options, cancellationToken);
        }

        Task<long> IMongoCollection<TDocument>.CountDocumentsAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options, CancellationToken cancellationToken)
        {
            return Source.CountDocumentsAsync(session, filter, options, cancellationToken);
        }

        DeleteResult IMongoCollection<TDocument>.DeleteMany(FilterDefinition<TDocument> filter, CancellationToken cancellationToken)
        {
            return Source.DeleteMany(filter, cancellationToken);
        }

        DeleteResult IMongoCollection<TDocument>.DeleteMany(FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken)
        {
            return Source.DeleteMany(filter, options, cancellationToken);
        }

        DeleteResult IMongoCollection<TDocument>.DeleteMany(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken)
        {
            return Source.DeleteMany(session, filter, options, cancellationToken);
        }

        Task<DeleteResult> IMongoCollection<TDocument>.DeleteManyAsync(FilterDefinition<TDocument> filter, CancellationToken cancellationToken)
        {
            return Source.DeleteManyAsync(filter, cancellationToken);
        }

        Task<DeleteResult> IMongoCollection<TDocument>.DeleteManyAsync(FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken)
        {
            return Source.DeleteManyAsync(filter, options, cancellationToken);
        }

        Task<DeleteResult> IMongoCollection<TDocument>.DeleteManyAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken)
        {
            return Source.DeleteManyAsync(session, filter, options, cancellationToken);
        }

        DeleteResult IMongoCollection<TDocument>.DeleteOne(FilterDefinition<TDocument> filter, CancellationToken cancellationToken)
        {
            return Source.DeleteOne(filter, cancellationToken);
        }

        DeleteResult IMongoCollection<TDocument>.DeleteOne(FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken)
        {
            return Source.DeleteOne(filter, options, cancellationToken);
        }

        DeleteResult IMongoCollection<TDocument>.DeleteOne(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken)
        {
            return Source.DeleteOne(session, filter, options, cancellationToken);
        }

        Task<DeleteResult> IMongoCollection<TDocument>.DeleteOneAsync(FilterDefinition<TDocument> filter, CancellationToken cancellationToken)
        {
            return Source.DeleteOneAsync(filter, cancellationToken);
        }

        Task<DeleteResult> IMongoCollection<TDocument>.DeleteOneAsync(FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken)
        {
            return Source.DeleteOneAsync(filter, options, cancellationToken);
        }

        Task<DeleteResult> IMongoCollection<TDocument>.DeleteOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken)
        {
            return Source.DeleteOneAsync(session, filter, options, cancellationToken);
        }

        IAsyncCursor<TField> IMongoCollection<TDocument>.Distinct<TField>(FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options, CancellationToken cancellationToken)
        {
            return Source.Distinct(field, filter, options, cancellationToken);
        }

        IAsyncCursor<TField> IMongoCollection<TDocument>.Distinct<TField>(IClientSessionHandle session, FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options, CancellationToken cancellationToken)
        {
            return Source.Distinct(session, field, filter, options, cancellationToken);
        }

        Task<IAsyncCursor<TField>> IMongoCollection<TDocument>.DistinctAsync<TField>(FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options, CancellationToken cancellationToken)
        {
            return Source.DistinctAsync(field, filter, options, cancellationToken);
        }

        Task<IAsyncCursor<TField>> IMongoCollection<TDocument>.DistinctAsync<TField>(IClientSessionHandle session, FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options, CancellationToken cancellationToken)
        {
            return Source.DistinctAsync(session, field, filter, options, cancellationToken);
        }

        IAsyncCursor<TItem> IMongoCollection<TDocument>.DistinctMany<TItem>(FieldDefinition<TDocument, IEnumerable<TItem>> field, FilterDefinition<TDocument> filter, DistinctOptions options, CancellationToken cancellationToken)
        {
            return Source.DistinctMany(field, filter, options, cancellationToken);
        }

        IAsyncCursor<TItem> IMongoCollection<TDocument>.DistinctMany<TItem>(IClientSessionHandle session, FieldDefinition<TDocument, IEnumerable<TItem>> field, FilterDefinition<TDocument> filter, DistinctOptions options, CancellationToken cancellationToken)
        {
            return Source.DistinctMany(session, field, filter, options, cancellationToken);
        }

        Task<IAsyncCursor<TItem>> IMongoCollection<TDocument>.DistinctManyAsync<TItem>(FieldDefinition<TDocument, IEnumerable<TItem>> field, FilterDefinition<TDocument> filter, DistinctOptions options, CancellationToken cancellationToken)
        {
            return Source.DistinctManyAsync(field, filter, options, cancellationToken);
        }

        Task<IAsyncCursor<TItem>> IMongoCollection<TDocument>.DistinctManyAsync<TItem>(IClientSessionHandle session, FieldDefinition<TDocument, IEnumerable<TItem>> field, FilterDefinition<TDocument> filter, DistinctOptions options, CancellationToken cancellationToken)
        {
            return Source.DistinctManyAsync(session, field, filter, options, cancellationToken);
        }

        long IMongoCollection<TDocument>.EstimatedDocumentCount(EstimatedDocumentCountOptions options, CancellationToken cancellationToken)
        {
            return Source.EstimatedDocumentCount(options, cancellationToken);
        }

        Task<long> IMongoCollection<TDocument>.EstimatedDocumentCountAsync(EstimatedDocumentCountOptions options, CancellationToken cancellationToken)
        {
            return Source.EstimatedDocumentCountAsync(options, cancellationToken);
        }

        Task<IAsyncCursor<TProjection>> IMongoCollection<TDocument>.FindAsync<TProjection>(FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options, CancellationToken cancellationToken)
        {
            return Source.FindAsync(filter, options, cancellationToken);
        }

        Task<IAsyncCursor<TProjection>> IMongoCollection<TDocument>.FindAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options, CancellationToken cancellationToken)
        {
            return Source.FindAsync(session, filter, options, cancellationToken);
        }

        TProjection IMongoCollection<TDocument>.FindOneAndDelete<TProjection>(FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options, CancellationToken cancellationToken)
        {
            return Source.FindOneAndDelete(filter, options, cancellationToken);
        }

        TProjection IMongoCollection<TDocument>.FindOneAndDelete<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options, CancellationToken cancellationToken)
        {
            return Source.FindOneAndDelete(session, filter, options, cancellationToken);
        }

        Task<TProjection> IMongoCollection<TDocument>.FindOneAndDeleteAsync<TProjection>(FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options, CancellationToken cancellationToken)
        {
            return Source.FindOneAndDeleteAsync(filter, options, cancellationToken);
        }

        Task<TProjection> IMongoCollection<TDocument>.FindOneAndDeleteAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options, CancellationToken cancellationToken)
        {
            return Source.FindOneAndDeleteAsync(session, filter, options, cancellationToken);
        }

        TProjection IMongoCollection<TDocument>.FindOneAndReplace<TProjection>(FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options, CancellationToken cancellationToken)
        {
            return Source.FindOneAndReplace(filter, replacement, options, cancellationToken);
        }

        TProjection IMongoCollection<TDocument>.FindOneAndReplace<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options, CancellationToken cancellationToken)
        {
            return Source.FindOneAndReplace(session, filter, replacement, options, cancellationToken);
        }

        Task<TProjection> IMongoCollection<TDocument>.FindOneAndReplaceAsync<TProjection>(FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options, CancellationToken cancellationToken)
        {
            return Source.FindOneAndReplaceAsync(filter, replacement, options, cancellationToken);
        }

        Task<TProjection> IMongoCollection<TDocument>.FindOneAndReplaceAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options, CancellationToken cancellationToken)
        {
            return Source.FindOneAndReplaceAsync(session, filter, replacement, options, cancellationToken);
        }

        TProjection IMongoCollection<TDocument>.FindOneAndUpdate<TProjection>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options, CancellationToken cancellationToken)
        {
            return Source.FindOneAndUpdate(filter, update, options, cancellationToken);
        }

        TProjection IMongoCollection<TDocument>.FindOneAndUpdate<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options, CancellationToken cancellationToken)
        {
            return Source.FindOneAndUpdate(session, filter, update, options, cancellationToken);
        }

        Task<TProjection> IMongoCollection<TDocument>.FindOneAndUpdateAsync<TProjection>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options, CancellationToken cancellationToken)
        {
            return Source.FindOneAndUpdateAsync(filter, update, options, cancellationToken);
        }

        Task<TProjection> IMongoCollection<TDocument>.FindOneAndUpdateAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options, CancellationToken cancellationToken)
        {
            return Source.FindOneAndUpdateAsync(session, filter, update, options, cancellationToken);
        }

        IAsyncCursor<TProjection> IMongoCollection<TDocument>.FindSync<TProjection>(FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options, CancellationToken cancellationToken)
        {
            return Source.FindSync(filter, options, cancellationToken);
        }

        IAsyncCursor<TProjection> IMongoCollection<TDocument>.FindSync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options, CancellationToken cancellationToken)
        {
            return Source.FindSync(session, filter, options, cancellationToken);
        }

        void IMongoCollection<TDocument>.InsertMany(IEnumerable<TDocument> documents, InsertManyOptions options, CancellationToken cancellationToken)
        {
            Source.InsertMany(documents, options, cancellationToken);
        }

        void IMongoCollection<TDocument>.InsertMany(IClientSessionHandle session, IEnumerable<TDocument> documents, InsertManyOptions options, CancellationToken cancellationToken)
        {
            Source.InsertMany(session, documents, options, cancellationToken);
        }

        Task IMongoCollection<TDocument>.InsertManyAsync(IEnumerable<TDocument> documents, InsertManyOptions options, CancellationToken cancellationToken)
        {
            return Source.InsertManyAsync(documents, options, cancellationToken);
        }

        Task IMongoCollection<TDocument>.InsertManyAsync(IClientSessionHandle session, IEnumerable<TDocument> documents, InsertManyOptions options, CancellationToken cancellationToken)
        {
            return Source.InsertManyAsync(session, documents, options, cancellationToken);
        }

        void IMongoCollection<TDocument>.InsertOne(TDocument document, InsertOneOptions options, CancellationToken cancellationToken)
        {
            Source.InsertOne(document, options, cancellationToken);
        }

        void IMongoCollection<TDocument>.InsertOne(IClientSessionHandle session, TDocument document, InsertOneOptions options, CancellationToken cancellationToken)
        {
            Source.InsertOne(session, document, options, cancellationToken);
        }

        [Obsolete]
        Task IMongoCollection<TDocument>.InsertOneAsync(TDocument document, CancellationToken _cancellationToken)
        {
            return Source.InsertOneAsync(document, _cancellationToken);
        }

        Task IMongoCollection<TDocument>.InsertOneAsync(TDocument document, InsertOneOptions options, CancellationToken cancellationToken)
        {
            return Source.InsertOneAsync(document, options, cancellationToken);
        }

        Task IMongoCollection<TDocument>.InsertOneAsync(IClientSessionHandle session, TDocument document, InsertOneOptions options, CancellationToken cancellationToken)
        {
            return Source.InsertOneAsync(session, document, options, cancellationToken);
        }

        [Obsolete]
        IAsyncCursor<TResult> IMongoCollection<TDocument>.MapReduce<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options, CancellationToken cancellationToken)
        {
            return Source.MapReduce(map, reduce, options, cancellationToken);
        }

        [Obsolete]
        IAsyncCursor<TResult> IMongoCollection<TDocument>.MapReduce<TResult>(IClientSessionHandle session, BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options, CancellationToken cancellationToken)
        {
            return Source.MapReduce(session, map, reduce, options, cancellationToken);
        }

        [Obsolete]
        Task<IAsyncCursor<TResult>> IMongoCollection<TDocument>.MapReduceAsync<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options, CancellationToken cancellationToken)
        {
            return Source.MapReduceAsync(map, reduce, options, cancellationToken);
        }

        [Obsolete]
        Task<IAsyncCursor<TResult>> IMongoCollection<TDocument>.MapReduceAsync<TResult>(IClientSessionHandle session, BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options, CancellationToken cancellationToken)
        {
            return Source.MapReduceAsync(session, map, reduce, options, cancellationToken);
        }

        IFilteredMongoCollection<TDerivedDocument> IMongoCollection<TDocument>.OfType<TDerivedDocument>()
        {
            return Source.OfType<TDerivedDocument>();
        }

        ReplaceOneResult IMongoCollection<TDocument>.ReplaceOne(FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions options, CancellationToken cancellationToken)
        {
            return Source.ReplaceOne(filter, replacement, options, cancellationToken);
        }

        [Obsolete]
        ReplaceOneResult IMongoCollection<TDocument>.ReplaceOne(FilterDefinition<TDocument> filter, TDocument replacement, UpdateOptions options, CancellationToken cancellationToken)
        {
            return Source.ReplaceOne(filter, replacement, options, cancellationToken);
        }

        ReplaceOneResult IMongoCollection<TDocument>.ReplaceOne(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions options, CancellationToken cancellationToken)
        {
            return Source.ReplaceOne(session, filter, replacement, options, cancellationToken);
        }

        [Obsolete]
        ReplaceOneResult IMongoCollection<TDocument>.ReplaceOne(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, UpdateOptions options, CancellationToken cancellationToken)
        {
            return Source.ReplaceOne(session, filter, replacement, options, cancellationToken);
        }

        Task<ReplaceOneResult> IMongoCollection<TDocument>.ReplaceOneAsync(FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions options, CancellationToken cancellationToken)
        {
            return Source.ReplaceOneAsync(filter, replacement, options, cancellationToken);
        }

        [Obsolete]
        Task<ReplaceOneResult> IMongoCollection<TDocument>.ReplaceOneAsync(FilterDefinition<TDocument> filter, TDocument replacement, UpdateOptions options, CancellationToken cancellationToken)
        {
            return Source.ReplaceOneAsync(filter, replacement, options, cancellationToken);
        }

        Task<ReplaceOneResult> IMongoCollection<TDocument>.ReplaceOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, ReplaceOptions options, CancellationToken cancellationToken)
        {
            return Source.ReplaceOneAsync(session, filter, replacement, options, cancellationToken);
        }

        [Obsolete]
        Task<ReplaceOneResult> IMongoCollection<TDocument>.ReplaceOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, UpdateOptions options, CancellationToken cancellationToken)
        {
            return Source.ReplaceOneAsync(session, filter, replacement, options, cancellationToken);
        }

        UpdateResult IMongoCollection<TDocument>.UpdateMany(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options, CancellationToken cancellationToken)
        {
            return Source.UpdateMany(filter, update, options, cancellationToken);
        }

        UpdateResult IMongoCollection<TDocument>.UpdateMany(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options, CancellationToken cancellationToken)
        {
            return Source.UpdateMany(session, filter, update, options, cancellationToken);
        }

        Task<UpdateResult> IMongoCollection<TDocument>.UpdateManyAsync(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options, CancellationToken cancellationToken)
        {
            return Source.UpdateManyAsync(filter, update, options, cancellationToken);
        }

        Task<UpdateResult> IMongoCollection<TDocument>.UpdateManyAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options, CancellationToken cancellationToken)
        {
            return Source.UpdateManyAsync(session, filter, update, options, cancellationToken);
        }

        UpdateResult IMongoCollection<TDocument>.UpdateOne(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options, CancellationToken cancellationToken)
        {
            return Source.UpdateOne(filter, update, options, cancellationToken);
        }

        UpdateResult IMongoCollection<TDocument>.UpdateOne(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options, CancellationToken cancellationToken)
        {
            return Source.UpdateOne(session, filter, update, options, cancellationToken);
        }

        Task<UpdateResult> IMongoCollection<TDocument>.UpdateOneAsync(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options, CancellationToken cancellationToken)
        {
            return Source.UpdateOneAsync(filter, update, options, cancellationToken);
        }

        Task<UpdateResult> IMongoCollection<TDocument>.UpdateOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options, CancellationToken cancellationToken)
        {
            return Source.UpdateOneAsync(session, filter, update, options, cancellationToken);
        }

        IChangeStreamCursor<TResult> IMongoCollection<TDocument>.Watch<TResult>(PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline, ChangeStreamOptions options, CancellationToken cancellationToken)
        {
            return Source.Watch(pipeline, options, cancellationToken);
        }

        IChangeStreamCursor<TResult> IMongoCollection<TDocument>.Watch<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline, ChangeStreamOptions options, CancellationToken cancellationToken)
        {
            return Source.Watch(session, pipeline, options, cancellationToken);
        }

        Task<IChangeStreamCursor<TResult>> IMongoCollection<TDocument>.WatchAsync<TResult>(PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline, ChangeStreamOptions options, CancellationToken cancellationToken)
        {
            return Source.WatchAsync(pipeline, options, cancellationToken);
        }

        Task<IChangeStreamCursor<TResult>> IMongoCollection<TDocument>.WatchAsync<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline, ChangeStreamOptions options, CancellationToken cancellationToken)
        {
            return Source.WatchAsync(session, pipeline, options, cancellationToken);
        }

        IMongoCollection<TDocument> IMongoCollection<TDocument>.WithReadConcern(ReadConcern readConcern)
        {
            return Source.WithReadConcern(readConcern);
        }

        IMongoCollection<TDocument> IMongoCollection<TDocument>.WithReadPreference(ReadPreference readPreference)
        {
            return Source.WithReadPreference(readPreference);
        }

        IMongoCollection<TDocument> IMongoCollection<TDocument>.WithWriteConcern(WriteConcern writeConcern)
        {
            return Source.WithWriteConcern(writeConcern);
        }
    }
}
