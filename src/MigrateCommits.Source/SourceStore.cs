namespace MigrateStreams.Source
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MigrateStreams.Remote;
    using NEventStore;

    public class SourceStore : MarshalByRefObject, ISourceStore
    {
        private readonly IStoreEvents _store;
        private readonly IEnumerator<Commit> _commits;

        public SourceStore()
        {
            //TODO: Setup your source store here
            _store = Wireup.Init().UsingInMemoryPersistence().InitializeStorageEngine().Build();

            //TODO: Remove below (for testing purposes only)
            using (var stream = _store.OpenStream(Guid.NewGuid(), 0, int.MaxValue))
            {
                var eventMessage = new EventMessage
                {
                    Body = "body",
                };
                eventMessage.Headers.Add("key", "value");
                stream.Add(eventMessage);
                stream.UncommittedHeaders.Add("header", "value");
                stream.CommitChanges(Guid.NewGuid());
            }
            //TODO: Remove above (for testing purposes only)

            _commits = _store.Advanced.GetFrom(DateTime.MinValue).GetEnumerator();
        }

        public SourceCommitMessage ReadNext()
        {
            if (!_commits.MoveNext())
            {
                return null;
            }
            return new SourceCommitMessage
            {
                StreamId = _commits.Current.StreamId,
                StreamRevision = _commits.Current.StreamRevision,
                CommitId = _commits.Current.CommitId,
                CommitSequence = _commits.Current.CommitSequence,
                CommitStamp = _commits.Current.CommitStamp,
                Headers = _commits.Current.Headers,
                Events = _commits.Current.Events.Select(e => new SourceEventMessage{ Headers = e.Headers, Body = e.Body }).ToArray()
            };
        }

        public void Dispose()
        {
            _store.Dispose();
        }
    }
}