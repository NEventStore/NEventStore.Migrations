namespace MigrateStreams.Remote
{
    using System;

    public interface IDestinationStore : IDisposable
    {
        void Put(SourceCommitMessage sourceCommit);
    }
}