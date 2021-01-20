namespace StatsDownload.DataStore
{
    using System;
    using System.Reflection;

    using Castle.Facilities.TypedFactory;

    using StatsDownload.Core.Interfaces;

    public class DataStoreFactoryComponentSelector : DefaultTypedFactoryComponentSelector
    {
        private const string UncDataStoreType = "Unc";

        private readonly IDataStoreSettings settings;

        public DataStoreFactoryComponentSelector(IDataStoreSettings settings)
        {
            this.settings = settings;
        }

        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            string dataStoreType = settings.DataStoreType;

            if (string.Equals(dataStoreType, UncDataStoreType, StringComparison.OrdinalIgnoreCase))
            {
                return typeof (UncDataStoreProvider).FullName;
            }

            return base.GetComponentName(method, arguments);
        }
    }
}