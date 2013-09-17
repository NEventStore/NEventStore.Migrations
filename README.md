NEventStore.Migrations
======================

A utility to migrate commits from between two persistence engines.

This works by hosting a source event store and a destination event store in seperate appdomains (so they can reference different versions of dependencies) and pulls commits from the source and puts them in the destination. To investigate how this works, clone the repo, open src\MigrateCommits.sln and run the MigrateCommits project. The utility uses in-memory stores.

Instructions
------------

 1. Clone the repo. You will need to make changes to it to suit your environment.
 2. Inspect the package dependencies in MigrateCommits.Source and MigrateCommits.Destination projects and ensure they match your environment.
 3. Modify ```MigrateCommits.Source.SourceStore``` to wirup to your source event store.
 4. Modify ```MigrateCommits.Destination.SourceStore``` to wirup to your destination event store.
 5. ***Important*** Add a reference to the MigrateCommits.Source and MigrateCommits.Destination projects to your events assembly so that headers and event bodies can be deserialized.
 6. Remove sample test code from ```MigrateCommits.Destination.SourceStore``` constructor.

Notes
-----

 * Backup everything first.
 * It is assumed that your messages (events & headers) are marked ```[Serializable]``` so they can be treansferred across appdomain boundaries. If they are not, either mark them as such, or, perform manual serialization, adjusting ```SourceCommitMessage``` and ```SourceEventMessage``` accordingly.
 * Snapshots are not migrated. Just re-run your snapshot process on the destination store after migration.

Questions? Head over to the [Google Group](https://groups.google.com/forum/#!forum/neventstore) or [Jabbr Room](https://jabbr.net/#/rooms/NEventStore)

