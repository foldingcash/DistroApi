# Using the API

The FoldingCoin (FLDC) StatsDownload API exposes merged folding data through a standard API.

## Calling the API

* Replace {server} with the hosting server name
* Replace {api_path} with the application path, if any

### ApiExplorer

The API explorer is automatically generated HTML documentation on the functionality available from the API

#### Calling ApiExplorer

```
GET http://{server}/{api_path}/
HTTP 200
```

or

```
GET http://{server}/{api_path}/v1/
HTTP 200
```

### GetDistro

Call to calculate a distribution to merged folders for the configured amount and timespan.

#### Calling GetDistro

Parameters 'StartDate', 'EndDate', and 'Amount' are required.

```
GET http://{server}/{api_path}/v1/GetDistro?StartDate=01-01-0001&EndDate=12-31-9999&Amount=7750000
HTTP 200
```

Default behavior:

* JSON response
* Decimal precision
* Rounded to eight decimal places
* Midpoint values are rounded to the nearest even number (or banker's rounding)
* Proportionally distributes the specified amount to the users for their points completed over the specified date-range
* Total distribution amount may not equal the specified amount (plus or minus by a small amount)

#### GetDistro Response Format

```
{
  "distro":
    [
      {
        "amount":{amount},
        "bitcoinAddress":"{bitcoinAddress}",
        "pointsGained":{pointsGained},
        "workUnitsGained":{workUnitsGained}
      }
    ],
  "distroCount":{distroCount},
  "firstErrorCode":0,
  "success":true,
  "totalDistro":{totalDistro},
  "totalPoints":{totalPoints},
  "totalWorkUnits":{totalWorkUnits}
}
```

### GetMemberStats

Call to get all folding members.

#### Calling GetMemberStats

Parameters 'StartDate' and 'EndDate' are required.

```
GET http://{server}/{api_path}/v1/GetMemberStats?StartDate=01-01-0001T00:00:00&EndDate=12-31-9999T23:59:59
HTTP 200
```

Default behavior:

* JSON response

#### GetMemberStats Response Format

```
{
  "members":
    [
      {
        "userName":"{userName}",
        "friendlyName":"{friendlyName}",
        "bitcoinAddress":"{bitcoinAddress}",
        "teamNumber":{teamNumber},
        "pointsGained":{pointsGained},
        "workUnitsGained":{workUnitsGained}
      }
    ],
  "memberCount":{distroCount},
  "firstErrorCode":0,
  "success":true
}
```

### GetTeams

Call to get all folding teams.

#### Calling GetTeams

No Parameters.

```
GET http://{server}/{api_path}/v1/GetTeams
HTTP 200
```

Default behavior:

* JSON response

#### GetTeams Response Format

```
{
  "teams":
    [
      {
        "teamNumber":{teamNumber},
        "teamName":"{teamName}"
      }
    ],
  "teamCount":{distroCount},
  "firstErrorCode":0,
  "success":true
}
```

## Errors

### Error Ranges

| Error Code Range | Error Reason            |
|:----------------:|-------------------------|
|       0000       | No errors               |
|     0001-0999    | Reserved for future use |
|     1000-1999    | User input error        |
|     2000-7999    | Reserved for future use |
|     8000-8999    | Database error          |
|     9000-9999    | Unexpected error        |

### Error Codes

| Error Code | Description                                                                                                                                                           |
|:----------:|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------|
|    0000    | No errors were encountered.                                                                                                                                           |
|    1000    | The start date is required, the format is MM-DD-YYYY or MM-DD-YYYYTHH:mm:ss; includes not providing a value, an incorrectly formatted date, and invalid dates.        |
|    1010    | The end date is required, the format is MM-DD-YYYY or MM-DD-YYYYTHH:mm:ss; includes not providing a value, an incorrectly formatted date, and invalid dates.          |
|    1020    | The start date must be a date prior to the current date; start with yesterday's date.                                                                                 |
|    1030    | The end date must be a date prior to the current date; start with yesterday's date.                                                                                   |
|    1040    | The start date must be earlier than or equal to the end date; ensure the end date is later than or equal to the start date.                                           |
|    1050    | The amount is required and is a whole number; includes not providing a value, an incorrectly formatted amount, and exceeding the Int32 upper/lower bound.             |
|    1060    | The amount was zero; the amount must be greater than zero.                                                                                                            |
|    1070    | The amount was negative; the amount must be greater than zero.                                                                                                        |
|    8000    | The database is unavailable. Wait a short period of time before trying again. If the problem continues, then the configuration is incorrect or access is blocked.     |
|    8005    | The database is missing required objects. Contact the person or oganization hosting the API.                                                                          |
|    9000    | There was an unexpected exception while processing. Try again and if the problem continues, then contact the person or organization hosting the API.                  |

### Error Response Format

```
{
  "errorCount":{errorCount},
  "errors":
    [
      {
        "errorCode":{errorCode},
        "errorMessage":"{errorMessage}"
      }
    ],
  "firstErrorCode":{firstErrorCode},
  "success":false
}
```