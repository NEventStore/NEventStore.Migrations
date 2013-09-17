namespace MigrateStreams.Remote
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SourceEventMessage
    {
        public Dictionary<string, object> Headers { get; set; }
        public object Body { get; set; }
    }
}