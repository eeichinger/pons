using System;
using System.IO;
using Spring.Core.IO;
using Spring.Data.Common;
using Spring.Data.Core;
using Spring.Testing.Ado;
using Viz.Testing.Support;

namespace Viz.Testing.MsSql
{
    public class DatabaseInstaller
    {
        private readonly DbProvider dbProvider;
        private readonly string databaseName;
        private readonly string dataLogicalName;
        private readonly string logLogicalName;

//        public DatabaseInstaller(DbProvider dbProvider)
//            :this(dbProvider, dbProvider.DefaultDatabaseName, dbProvider.DefaultDatabaseName + "_log")
//        {}

        public DatabaseInstaller(DbProvider dbProvider, string databaseName, string dataLogicalName, string logLogicalName)
        {
            this.dbProvider = dbProvider;
            this.databaseName = databaseName;
            this.dataLogicalName = dataLogicalName;
            this.logLogicalName = logLogicalName;
        }

        public void InstallFromFileSystemBackup(FileInfo databaseBackupLocation)
        {
            Drop();

            AdoTemplate ado = new AdoTemplate(dbProvider);

            // (re-)create database(s)
            using(ChangeDatabase("master"))
            {
                string script = GetScript("RestoreDatabase.sql");
                script = script.Replace("%DATABASE_NAME%", databaseName);
                script = script.Replace("%DATABASE_LOGICALNAME_DATA%", dataLogicalName);
                script = script.Replace("%DATABASE_LOGICALNAME_LOG%", logLogicalName);
                script = script.Replace("%DATABASE_BACKUPFILE%", databaseBackupLocation.FullName);
                SimpleAdoTestUtils.ExecuteSqlScript(ado, script);                
            }
        }

        public void InstallFromResourceBackup(IResource resource)
        {
            // extract database backup resource to temp dir
            FileInfo destination = new FileInfo(Path.GetTempFileName());
            TestResourceLoader.ExportResource(resource, destination);
            
            InstallFromFileSystemBackup(destination);
            destination.Delete();
        }

        public void Drop()
        {
            AdoTemplate ado = new AdoTemplate(dbProvider);

            // (re-)create database(s)
            string script = GetScript("DropDatabase.sql");
            using(ChangeDatabase("master"))
            {
                script = script.Replace("%DATABASE_NAME%", databaseName);
                SimpleAdoTestUtils.ExecuteSqlScript(ado, script);                
            }
        }

        private IDisposable ChangeDatabase(string db)
        {
            throw new NotImplementedException();
        }

        private string GetScript(string resourceName)
        {
            string script = TestResourceLoader.GetText(this, resourceName);
            return script;
        }
    }
}