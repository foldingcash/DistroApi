namespace StatsDownload.Core.Implementations
{
    using System;
    using System.Reflection;

    using Castle.Facilities.TypedFactory;

    using StatsDownload.Core.Interfaces;

    public class DataStoreFactoryComponentSelector : DefaultTypedFactoryComponentSelector
    {
        private readonly IDataStoreSettings settings;

        public DataStoreFactoryComponentSelector(IDataStoreSettings settings)
        {
            this.settings = settings;
        }

        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            string databaseType = settings.DataStoreType;

            if (string.Equals(databaseType, "Unc", StringComparison.OrdinalIgnoreCase))
            {
                return typeof (UncDataStoreProvider).FullName;
            }

            return base.GetComponentName(method, arguments);
        }
    }
}