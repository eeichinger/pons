using System;
using Pons.MsSql;
using Pons.Support;
using Spring.Core.IO;
using Spring.Data.Common;

namespace Pons
{
    public class DatabaseFromBackup : IDisposable
    {
        private readonly DatabaseInstaller dbInstaller;

        public DatabaseFromBackup(DbProvider dbProvider, IResource resource, string databaseName, string logicalNameData, string logicalNameLog)
        {
            // extracts database backup resource to temp dir and restores DB
            dbInstaller = new DatabaseInstaller(dbProvider, databaseName, logicalNameData, logicalNameLog);
            dbInstaller.InstallFromResourceBackup(resource);
        }

        public DatabaseFromBackup(DbProvider dbProvider, object resourceContext, string resourceName, string databaseName, string logicalNameData, string logicalNameLog)
            :this(dbProvider, TestResourceLoader.GetResource(resourceContext, resourceName),  databaseName, logicalNameData, logicalNameLog )
        {}

        public void Dispose()
        {
            dbInstaller.Drop();
        }
    }
}