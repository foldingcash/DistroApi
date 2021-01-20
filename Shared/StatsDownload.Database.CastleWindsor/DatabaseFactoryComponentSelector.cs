namespace StatsDownload.Database.CastleWindsor
{
    using System;
    using System.Reflection;

    using Castle.Facilities.TypedFactory;

    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Settings;
    using StatsDownload.Database.Wrappers;

    public class DatabaseFactoryComponentSelector : DefaultTypedFactoryComponentSelector
    {
        private readonly DatabaseSettings settings;

        public DatabaseFactoryComponentSelector(IOptions<DatabaseSettings> settings)
        {
            this.settings = settings.Value;
        }

        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            string databaseType = settings.Type;

            if (string.Equals(databaseType, "MicrosoftSql", StringComparison.OrdinalIgnoreCase))
            {
                return typeof (MicrosoftSqlDatabaseConnectionProvider).FullName;
            }

            return base.GetComponentName(method, arguments);
        }
    }
}