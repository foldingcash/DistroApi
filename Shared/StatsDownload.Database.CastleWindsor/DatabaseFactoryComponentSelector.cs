namespace StatsDownload.Database.CastleWindsor
{
    using System;
    using System.Reflection;

    using Castle.Facilities.TypedFactory;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Database.Wrappers;

    public class DatabaseFactoryComponentSelector : DefaultTypedFactoryComponentSelector
    {
        private readonly IDatabaseConnectionSettingsService settings;

        public DatabaseFactoryComponentSelector(IDatabaseConnectionSettingsService settings)
        {
            this.settings = settings;
        }

        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            string databaseType = settings.GetDatabaseType();

            if (string.Equals(databaseType, "MicrosoftSql", StringComparison.OrdinalIgnoreCase))
            {
                return typeof (MicrosoftSqlDatabaseConnectionProvider).FullName;
            }

            return base.GetComponentName(method, arguments);
        }
    }
}