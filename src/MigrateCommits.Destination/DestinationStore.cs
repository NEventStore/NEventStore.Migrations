using System.Collections.Generic;

namespace MigrateStreams.Destination
{
    using System;
    using System.Linq;
    using MigrateStreams.Remote;
    using NEventStore;

    public class DestinationStore : MarshalByRefObject, IDestinationStore
    {
        private readonly IStoreEvents _store;

        public DestinationStore()
        {
            //TODO: Setup your destination store here
            _store = Wireup.Init().UsingInMemoryPersistence().InitializeStorageEngine().Build();
        }

        public void Put(SourceCommitMessage sourceCommit)
        {
            var commitAttempt = new CommitAttempt(
                sourceCommit.StreamId,
                sourceCommit.StreamRevision,
                sourceCommit.CommitId,
                sourceCommit.CommitSequence,
                sourceCommit.CommitStamp,
                sourceCommit.Headers,
                sourceCommit.Events.Select(s => new EventMessage{ Headers = DeserializeHeaders(s.Headers), Body = DeserializeBody(s) }).ToArray());
            _store.Advanced.Commit(commitAttempt);
        }

        static object DeserializeBody(SourceEventMessage s)
        {
            // We're we're simply returning the object, b/c we're assuming that events are marked as serializable
            //You can also add deserialization code here that matches the serialization code from the source
            return s.Body;
        }

        static Dictionary<string, object> DeserializeHeaders(Dictionary<string, object> headers)
        {
            // We're we're simply returning the object, b/c we're assuming that events are marked as serializable
            //You can also add deserialization code here that matches the serialization code from the source
            return headers;
        }

        public void Dispose()
        {
            _store.Dispose();
        }
    }
}