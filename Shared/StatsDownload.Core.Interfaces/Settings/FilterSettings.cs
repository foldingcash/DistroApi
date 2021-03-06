﻿namespace StatsDownload.Core.Interfaces.Settings
{
    public class FilterSettings
    {
        public bool EnableGoogleUsersFilter { get; set; }

        public bool EnableNoPaymentAddressUsersFilter { get; set; }

        public bool EnableWhitespaceNameUsersFilter { get; set; }

        public bool EnableZeroPointUsersFilter { get; set; }
    }
}