namespace StatsDownloadApi.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Interfaces;
    using Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Enums;

    public class StatsDownloadApiDatabaseProvider : IStatsDownloadApiDatabaseService
    {
        private readonly IStatsDownloadDatabaseService statsDownloadDatabaseService;

        public StatsDownloadApiDatabaseProvider(IStatsDownloadDatabaseService statsDownloadDatabaseService)
        {
            this.statsDownloadDatabaseService = statsDownloadDatabaseService ??
                                                throw new ArgumentNullException(nameof(statsDownloadDatabaseService));
        }

        public IList<FoldingUser> GetFoldingMembers(DateTime startDate, DateTime endDate)
        {
            var users = new List<FoldingUser>();
            statsDownloadDatabaseService.CreateDatabaseConnectionAndExecuteAction(service =>
            {
                DbParameter startDateParameter =
                    service.CreateParameter("@StartDate", DbType.Date, ParameterDirection.Input);
                DbParameter endDateParameter =
                    service.CreateParameter("@EndDate", DbType.Date, ParameterDirection.Input);

                startDateParameter.Value = startDate;
                endDateParameter.Value = endDate;

                var dataTable = new DataTable();
                service.ExecuteStoredProcedure(Constants.StatsDownloadApiDatabase.GetFoldingMembersProcedureName,
                    new[] { startDateParameter, endDateParameter },
                    dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    users.Add(new FoldingUser(row["FriendlyName"] as string,
                        row["BitcoinAddress"] as string,
                        (row["PointsGained"] as long?).GetValueOrDefault(),
                        (row["WorkUnitsGained"] as long?).GetValueOrDefault()));
                }
            });
            return users;
        }

        public IList<Member> GetMembers(DateTime startDate, DateTime endDate)
        {
            var users = new List<Member>();
            statsDownloadDatabaseService.CreateDatabaseConnectionAndExecuteAction(service =>
            {
                DbParameter startDateParameter =
                    service.CreateParameter("@StartDate", DbType.Date, ParameterDirection.Input);
                DbParameter endDateParameter =
                    service.CreateParameter("@EndDate", DbType.Date, ParameterDirection.Input);

                startDateParameter.Value = startDate;
                endDateParameter.Value = endDate;

                var dataTable = new DataTable();
                service.ExecuteStoredProcedure(Constants.StatsDownloadApiDatabase.GetMembersProcedureName,
                    new[] { startDateParameter, endDateParameter },
                    dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    users.Add(new Member(row["UserName"] as string,
                        row["FriendlyName"] as string,
                        row["BitcoinAddress"] as string, (row["TeamNumber"] as long?).GetValueOrDefault(),
                        (row["StartPoints"] as long?).GetValueOrDefault(),
                        (row["StartWorkUnits"] as long?).GetValueOrDefault(),
                        (row["PointsGained"] as long?).GetValueOrDefault(),
                        (row["WorkUnitsGained"] as long?).GetValueOrDefault()));
                }
            });
            return users;
        }

        public IList<Team> GetTeams()
        {
            var users = new List<Team>();
            statsDownloadDatabaseService.CreateDatabaseConnectionAndExecuteAction(service =>
            {
                var dataTable = new DataTable();
                service.ExecuteStoredProcedure(Constants.StatsDownloadApiDatabase.GetTeamsProcedureName,
                    dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    users.Add(new Team((row["TeamNumber"] as long?).GetValueOrDefault(), row["TeamName"] as string));
                }
            });
            return users;
        }

        public (bool isAvailable, DatabaseFailedReason reason) IsAvailable()
        {
            return statsDownloadDatabaseService.IsAvailable(Constants.StatsDownloadApiDatabase.ApiObjects);
        }
    }
}