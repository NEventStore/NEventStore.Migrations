namespace MigrateStreams.Remote
{
    using System;

    public interface ISourceStore : IDisposable
    {
        SourceCommitMessage ReadNext();
    }
}