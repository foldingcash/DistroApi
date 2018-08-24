namespace StatsDownloadApi.WebApi.CastleWindsor
{
    using Castle.Facilities.TypedFactory;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Core;
    using Database;
    using Interfaces;
    using NLog;
    using NLog.Web;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Logging;
    using StatsDownload.Database;
    using StatsDownload.Database.CastleWindsor;
    using StatsDownload.Database.Wrappers;
    using StatsDownload.Email;
    using StatsDownload.Logging;
    using StatsDownload.Wrappers;

    public class DependencyInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ILogger>()
                                        .Instance(NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger()));

            container.Register(
                Component.For<IDatabaseConnectionSettingsService, IEmailSettingsService>()
                         .ImplementedBy<StatsDownloadApiSettingsProvider>(),
                Component.For<IApplicationLoggingService>().ImplementedBy<StatsDownloadApiLoggingProvider>(),
                Component.For<IStatsDownloadApiEmailService>().ImplementedBy<StatsDownloadApiEmailProvider>());

            container.Register(Component.For<IDateTimeService>().ImplementedBy<DateTimeProvider>(),
                Component.For<IErrorMessageService>().ImplementedBy<ErrorMessageProvider>(),
                Component.For<ILoggingService>().ImplementedBy<LoggingProvider>(),
                Component.For<IEmailService>().ImplementedBy<EmailProvider>(),
                Component.For<IEmailSettingsValidatorService>().ImplementedBy<EmailSettingsValidatorProvider>(),
                Component.For<IDatabaseConnectionService>().ImplementedBy<MySqlDatabaseConnectionProvider>(),
                Component.For<IDatabaseConnectionService>().ImplementedBy<MicrosoftSqlDatabaseConnectionProvider>()
                         .IsDefault(),
                Component.For<ITypedFactoryComponentSelector>().ImplementedBy<DatabaseFactoryComponentSelector>(),
                Component.For<IDatabaseConnectionServiceFactory>().AsFactory(selector =>
                    selector.SelectedWith<DatabaseFactoryComponentSelector>()),
                Component.For<IStatsDownloadApiTokenDistributionService>()
                         .ImplementedBy<StandardTokenDistributionProvider>(),
                Component
                    .For<IStatsDownloadDatabaseService>()
                    .ImplementedBy<StatsDownloadDatabaseProvider>(),
                Component.For<IStatsDownloadApiDatabaseService>().ImplementedBy<StatsDownloadApiDatabaseProvider>(),
                Component.For<IStatsDownloadApiService>().ImplementedBy<StatsDownloadApiProvider>()
            );
        }
    }
}