namespace StatsDownload.DataStore
{
    using System;
    using System.Reflection;

    using Castle.Facilities.TypedFactory;

    using Microsoft.Extensions.Options;

    using StatsDownload.Core.Interfaces.Settings;

    public class DataStoreFactoryComponentSelector : DefaultTypedFactoryComponentSelector
    {
        private const string UncDataStoreType = "Unc";

        private readonly DataStoreSettings settings;

        public DataStoreFactoryComponentSelector(IOptions<DataStoreSettings> settings)
        {
            this.settings = settings.Value;
        }

        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            string dataStoreType = settings.Type;

            if (string.Equals(dataStoreType, UncDataStoreType, StringComparison.OrdinalIgnoreCase))
            {
                return typeof (UncDataStoreProvider).FullName;
            }

            return base.GetComponentName(method, arguments);
        }
    }
}