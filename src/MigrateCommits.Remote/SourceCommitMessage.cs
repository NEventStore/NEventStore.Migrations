namespace MigrateStreams.Remote
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SourceCommitMessage
    {
        public Guid StreamId { get; set; }
        public int StreamRevision { get; set; }
        public Guid CommitId { get; set; }
        public int CommitSequence { get; set; }
        public DateTime CommitStamp { get; set; }
        public Dictionary<string, object> Headers { get; set; }
        public SourceEventMessage[] Events { get; set; }
    }
}