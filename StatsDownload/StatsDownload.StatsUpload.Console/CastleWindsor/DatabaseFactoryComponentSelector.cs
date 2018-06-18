namespace StatsDownload.StatsUpload.Console.CastleWindsor
{
    using System;
    using System.Reflection;
    using Castle.Facilities.TypedFactory;
    using Core.Interfaces;
    using Core.Wrappers;

    public class DatabaseFactoryComponentSelector : DefaultTypedFactoryComponentSelector
    {
        private readonly IDatabaseConnectionSettingsService settings;

        public DatabaseFactoryComponentSelector(IDatabaseConnectionSettingsService settings)
        {
            this.settings = settings;
        }

        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            var databaseType = settings.GetDatabaseType();
            if (string.Equals(databaseType, "MySql", StringComparison.OrdinalIgnoreCase))
            {
                return typeof (MySqlDatabaseConnectionProvider).FullName;
            }

            if (string.Equals(databaseType, "MicrosoftSql", StringComparison.OrdinalIgnoreCase))
            {
                return typeof (MicrosoftSqlDatabaseConnectionProvider).FullName;
            }

            return base.GetComponentName(method, arguments);
        }
    }
}