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

            //Uncomment if you want to add some test data
            //CreateTestStream();

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
                Events = _commits.Current.Events.Select(e => new SourceEventMessage{ Headers = SerializeHeaders(e.Headers), Body = SerializeBody(e.Body) }).ToArray()
            };
        }

        object SerializeBody(object body)
        {
            // We're we're simply returning the object, b/c we're assuming that events are marked as serializable
            //You can also add serialization code here

            var bodyType = body.GetType();

            if(!bodyType.IsSerializable)
            {
                throw new InvalidOperationException(string.Format(
                    "Cannot read commit from source store. The body of type {0} is not serializable. " +
                    "Please make it serializable or change the method SourceStore.SerializeBody to use custom serialization.", 
                    bodyType.FullName));
            }

            return body;
        }

        Dictionary<string, object> SerializeHeaders(Dictionary<string, object> headers)
        {
            // We're we're simply returning the object, b/c we're assuming that events are marked as serializable
            //You can also add serialization code for each value here

            var notSerializable = headers.Values
                .Select(v => v.GetType())
                .Where(t => !t.IsSerializable)
                .Select(t => t.FullName)
                .ToArray();

            if(notSerializable.Length > 0)
            {
                throw new InvalidOperationException(string.Format(
                    "Cannot read commit from source store. The following header types are not serializable: '{0}'. " +
                    "Please make them serializable or change the method SourceStore.SerializeHeaders to use custom serialization.", 
                    String.Join(", ", notSerializable)));
            }

            return headers;
        }

        protected void CreateTestStream(int numberOfStreams = 10)
        {
            for (var i = 0; i < numberOfStreams; i++)
            {
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
            }
        }

        public void Dispose()
        {
            _store.Dispose();
        }
    }

    public class NonSerializable
    {
        
    }
}