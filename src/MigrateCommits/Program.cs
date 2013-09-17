namespace MigrateStreams
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using MigrateStreams.Remote;

    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Migrating commits...");
            int count = 0;
            using (var sourceStoreHost = new StoreHost<ISourceStore>(
                "SourceStore",
                "MigrateStreams.Source.dll",
                "MigrateStreams.Source.SourceStore"))
            {
                using (var destinationStoreHost = new StoreHost<IDestinationStore>(
                    "DestinationStore",
                    "MigrateStreams.Destination.dll",
                    "MigrateStreams.Destination.DestinationStore"))
                {
                    SourceCommitMessage sourceCommitMessage;
                    while ((sourceCommitMessage = sourceStoreHost.Store.ReadNext()) != null)
                    {
                        destinationStoreHost.Store.Put(sourceCommitMessage);
                        count++;
                    }
                }
            }
            Console.WriteLine("Migrated {0} commits", count);
            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        private class StoreHost<T> : IDisposable
            where T: IDisposable
        {
            private readonly AppDomain _appDomain;
            private readonly T _store;

            public StoreHost(string applicationBase, string assemblyFileName, string storeType)
            {
                string friendlyName = applicationBase;
                string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                applicationBase = Path.Combine(currentPath, applicationBase);
                string assemblyPath = Path.Combine(applicationBase, assemblyFileName);
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
                Debug.Assert(File.Exists(assemblyPath));

                _appDomain = AppDomain.CreateDomain(
                    friendlyName,
                    AppDomain.CurrentDomain.Evidence,
                    new AppDomainSetup
                    {
                        ApplicationBase = applicationBase,
                    });
                _store = (T)_appDomain.CreateInstanceAndUnwrap(assemblyName.Name, storeType);
            }

            public T Store
            {
                get { return _store; }
            }

            public void Dispose()
            {
                _store.Dispose();
                AppDomain.Unload(_appDomain);
            }
        }
    }
}