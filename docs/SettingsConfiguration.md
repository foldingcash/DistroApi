# Stats Settings

## Logging Settings

1. [NLog configuration](http://nlog-project.org/config/)

## Stats Download Database Connection Settings

1. FoldingCoin
	* A connection string to the database loaded with the schema and stored procedures
2. DbCommandTimeout
	* The timeout value, in seconds, to use for all database command functions. If not included in the app.config file, the application will not explicitly set the value on the command, using the framework's default value.

## Stats Download Email Settings

1. SmtpHost
	* The SMTP server to route emails through
2. Port
	* The SMTP server's port to communicate on
3. FromAddress
	* The address emails when be sent from
4. DisplayName
	* The display name the emails sent will have
5. Password
	* The password to access and use the SMTP server
6. Receivers
	* A semi-colon separated list of emails to send notifications to

## File Download Settings

1. DownloadUri
	* The uri to the stats file resource
2. DownloadTimeoutSeconds
	* The amount of time in seconds we will try to download the stats file
3. DownloadDirectory
	* The directory to store the download files and process them for upload
4. AcceptAnySslCert
	* Accept all SSL certificates regardless if they are valid
5. MinimumWaitTimeInHours
	* The amount of time in hours we should wait between file downloads
	
## Stats Upload Settings

1. StatsFileTimeZoneAndOffset
	* A key value pair of time zone symbols and its' offset

### Filters
1. EnableNoPaymentAddressUsersFilter
	* The stats upload only processes users with a payment address
2. EnableZeroPointUsersFilter
	* The stats upload will filter out users with zero points
3. EnableWhitespaceNameUsersFilter
	* The stats upload will filter out users with a whitespace name
4. EnableGoogleUsersFilter
	* The stats upload will filter out users whoms name start with 'google'

## Test Harness Only Settings

1. DisableMinimumWaitTime
	* Disables the minimum wait time feature
2. DisableSecureFilePayload
	* Disables trying to upgrade HTTP connections to HTTPS connections automatically
3. EnableOneHundredUsersFilter
	* The stats upload only processes one hundred users