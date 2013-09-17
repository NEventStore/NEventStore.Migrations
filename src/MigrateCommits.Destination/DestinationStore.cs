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
                sourceCommit.Events.Select(s => new EventMessage{ Headers = s.Headers, Body = s.Body }).ToArray());
            _store.Advanced.Commit(commitAttempt);
        }

        public void Dispose()
        {
            _store.Dispose();
        }
    }
}