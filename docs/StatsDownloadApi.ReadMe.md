# Stats API

## Getting Started

The stats API is used to expose the stats data in the stats database.

## Prerequisites

* .NET Core 2.0
* Microsoft SQL Server 2017

## Installation

### Installing on Windows

1. Step 1

### Settings

1. [Database Settings](SettingsConfiguration.md#stats-download-database-connection-settings)	

### Applications

1. WebApi
	* This is an API meant to provide services to various web applications

## Using the API

### Calling the API

* Replace {server} with the hosting server name
* Replace {api_path} with the application path, if any

#### Calling GetDistro

Both parameters 'StartDate' and 'EndDate' are required

```
GET http://{server}/{api_path}/v1/GetDistro?StartDate=01-01-0001&EndDate=12-31-9999
```

the API will return a JSON response

#### GetDistro Response Format

```
{
  "distro":
    [
      {
        "bitcoinAddress":"{bitcoinAddress}"
      }
    ],
  "distroCount":{distroCount},
  "firstErrorCode":0,
  "success":true
}
```

### Errors

#### Error Ranges

| Error Code Range | Error Reason            |
|:----------------:|-------------------------|
|       0000       | No errors               |
|     0001-0999    | Reserved for future use |
|     1000-1999    | User input error        |
|     2000-7999    | Reserved for future use |
|     8000-8999    | Database error          |
|     9000-9999    | Unexpected error        |

#### Error Codes

| Error Code | Resolution                                                                                                                                                      |
|:----------:|-----------------------------------------------------------------------------------------------------------------------------------------------------------------|
|    0000    | No errors were encountered.                                                                                                                                     |
|    1010    | The end date was not parsable to a DateTime object; includes not providing a value, a string value, and invalid date (lower/upper). The format is MM-DD-YYYY.   |
|    1000    | The start date was not parsable to a DateTime object; includes not providing a value, a string value, and invalid date (lower/upper). The format is MM-DD-YYYY. |
|    8000    | Database is unavailable. Wait a short period of time before trying to connect again. If the problem continues, then contact the technical advisor.              |

#### Error Response Format

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
	
## Running the tests

### Unit Tests

Options for running the unit tests:
* Use ReSharper's Test Runner from within Visual Studio
* Use the nunit console runner, using a command such as:
```
nunit-console {path-to-assembly}
```
	
### Integration Tests

1. Integration tests
	
## Deployment

### Api Hardware Requirements

* HDD 120GB+
* i5 Intel / Ryzen 5 AMD
* 16 GB