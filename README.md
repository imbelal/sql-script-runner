# SqlScriptRunner Azure Function App

## Overview

`SqlScriptRunner` is an Azure Function App that executes SQL scripts stored in Azure Blob Storage and uploads the results back to Blob Storage. The function is triggered via HTTP requests and can handle multiple scripts in a single run. It leverages services to read SQL scripts from Blob Storage, execute the scripts on a SQL database, and upload the query results as CSV files back to Blob Storage.

## Features

- **Blob Storage Integration**: Reads SQL scripts from a specified blob container.
- **SQL Query Execution**: Executes the SQL scripts on a target SQL database.
- **Result Uploads**: Converts query results into CSV format and uploads them back to Blob Storage.
- **HTTP Trigger**: Can be triggered via HTTP `GET` or `POST` requests.

## Requirements

- **Azure Account**: For setting up Function App and Blob Storage.
- **Blob Storage**: SQL scripts must be uploaded to a blob container (e.g., `scripts`).
- **SQL Database**: The function will execute the SQL scripts against a configured SQL database.
- **Azure Functions Core Tools**: For local development and testing.

## Getting Started

### Prerequisites

1. **.NET Core SDK** (v6.0 or later)
2. **Azure Functions Core Tools**
3. **Azure Storage Account** with a blob container for SQL scripts.
4. **SQL Database** to execute the queries.

### Setup

1. **Clone the repository**:
    ```bash
    git clone https://github.com/your-repo/sql-script-runner.git
    cd sql-script-runner
    ```

2. **Install Azure Functions Core Tools** (for local testing):
    ```bash
    npm install -g azure-functions-core-tools@4 --unsafe-perm true
    ```

3. **Configure Connection Strings**:

   - **Blob Storage**: Set up the connection string for the Azure Blob Storage in the `local.settings.json` file.
   - **SQL Database**: Configure the SQL connection string for executing scripts.

   Example `local.settings.json`:
   ```json
   {
     "IsEncrypted": false,
     "Values": {
       "AzureWebJobsStorage": "Your_AzureWebJobsStorage_ConnectionString",
       "FUNCTIONS_WORKER_RUNTIME": "dotnet",
       "BlobStorageConnectionString": "Your_Blob_Storage_ConnectionString",
       "SqlConnectionString": "Your_SQL_Database_ConnectionString"
     }
   }

4. **Run the Function Locally**:

   ```bash
   func start
   ```
### Deploy to Azure
1. **Create a Function App in the Azure portal or using the Azure CLI**:
   ```bash
   az functionapp create --resource-group <ResourceGroup> --consumption-plan-location <Location> --runtime dotnet --functions-version 4 --name <FunctionAppName> --storage-account <StorageAccountName>
   ```
2. **Deploy the function**:
   ```bash
   func azure functionapp publish <FunctionAppName>
   ```
3. **Set Environment Variables**:
   In the Azure portal, navigate to your Function App, go to "Configuration" and add the following application settings:
   - BlobStorageConnectionString: Your Azure Blob Storage connection string.
   - SqlConnectionString: Your SQL database connection string.
  
### Usage
The function can be triggered via an HTTP GET or POST request. The endpoint will execute all SQL scripts stored in the scripts container in Blob Storage and upload the results as CSV files back to the same container.
```bash
curl -X POST https://<your-function-app-name>.azurewebsites.net/api/execute-scripts
```
```graphql
SqlScriptRunner/
│
├── Services/
│   ├── IBlobStorageService.cs           # Interface for blob storage operations
│   ├── ISqlQueryExecutor.cs             # Interface for executing SQL queries
│   └── Implementations/
│       ├── BlobStorageService.cs        # Implements blob storage operations
│       └── SqlQueryExecutor.cs          # Implements SQL execution logic
│
├── SqlScriptExecutionFunction.cs        # Azure function class
├── local.settings.json                  # Local development settings
├── host.json                            # Function configuration file
├── SqlScriptRunner.csproj               # Project file
└── README.md                            # Documentation
```

### Contributing
Feel free to submit issues or pull requests if you would like to contribute. Please ensure that any code changes are well-tested and documented.

### License
This project is licensed under the MIT License. See the LICENSE file for details.
   
