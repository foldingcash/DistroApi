# Stats API

## Installing on Windows

1. Step 1

### Applications

1. WebApi
	* This is an API meant to provide services to various web applications
	
#### Error Codes

| Error Code Range | Error Reason            |
|:----------------:|-------------------------|
|       0000       | No errors               |
|     0001-0999    | Reserved for future use |
|     1000-1999    | User input error        |
|     2000-7999    | Reserved for future use |
|     8000-8999    | Database error          |
|     9000-9999    | Unexpected error        |

| Error Code | Resolution |
|:----------:|------------|
|    0000    | No errors were encountered. |
|    8000    | Database is unavailable. Wait a short period of time before trying to connect again. If the problem continues, then contact the technical advisor. |

### Settings

1. [Database Settings](SettingsConfiguration.md#stats-download-database-connection-settings)	

### Integration Tests

1. Integration tests
	
## Deployment

### Api Hardware Requirements

* HDD 120GB+
* i5 Intel / Ryzen 5 AMD
* 16 GB