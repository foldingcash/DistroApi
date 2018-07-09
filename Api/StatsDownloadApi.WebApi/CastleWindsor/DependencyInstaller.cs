namespace StatsDownloadApi.WebApi.CastleWindsor
{
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Core;
    using Interfaces;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Logging;
    using StatsDownload.Database;
    using StatsDownload.Database.CastleWindsor;
    using StatsDownload.Database.Wrappers;
    using StatsDownload.Logging;
    using StatsDownload.Wrappers;

    public class DependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IDatabaseConnectionSettingsService>().ImplementedBy<StatsDownloadApiSettingsProvider>(),
                Component.For<IApplicationLoggingService>().ImplementedBy<StatsDownloadApiLoggingProvider>());

            container.Register(Component.For<IDateTimeService>().ImplementedBy<DateTimeProvider>(),
                Component.For<IErrorMessageService>().ImplementedBy<ErrorMessageProvider>(),
                Component.For<ILoggingService>().ImplementedBy<LoggingProvider>(),
                Component.For<IDatabaseConnectionService>().ImplementedBy<MySqlDatabaseConnectionProvider>(),
                Component.For<IDatabaseConnectionService>().ImplementedBy<MicrosoftSqlDatabaseConnectionProvider>()
                         .IsDefault(),
                Component.For<ITypedFactoryComponentSelector>().ImplementedBy<DatabaseFactoryComponentSelector>(),
                Component.For<IDatabaseConnectionServiceFactory>().AsFactory(selector =>
                    selector.SelectedWith<DatabaseFactoryComponentSelector>()),
                Component
                    .For<IStatsDownloadDatabaseService>()
                    .ImplementedBy<StatsDownloadDatabaseProvider>(),
                Component.For<IStatsDownloadApiService>().ImplementedBy<StatsDownloadApiProvider>());
        }
    }
}