# 🛠️ SqlScriptRunner Function App

## 🌟 Overview

**`SqlScriptRunner`** is a powerful Function App designed to execute SQL scripts stored in **Azure Blob Storage** against a target SQL database. It also uploads the results back to Blob Storage, providing a seamless workflow for database management. The function is triggered via HTTP requests and can handle multiple scripts in a single run. It supports **MSSQL**, **PostgreSQL**, and **MySQL** databases.

## 🚀 Features

- **🗃️ Blob Storage Integration**: Reads SQL scripts from a specified blob container.
- **⚙️ SQL Query Execution**: Executes SQL scripts on a target SQL database.
- **🌐 Multi-Database Support**: Compatible with **MSSQL** ![MSSQL Icon](https://img.icons8.com/color/48/000000/microsoft-sql-server.png) , **PostgreSQL** ![PostgreSQL Icon](https://img.icons8.com/color/48/000000/postgreesql.png) , **MySQL** ![MySQL Icon](https://img.icons8.com/color/48/000000/mysql-logo.png) databases.
- **📅 Script Execution Strategies**: Flexible strategies for executing scripts based on naming conventions (e.g., daily, weekly, monthly).
- **📤 Result Uploads**: Converts query results into **CSV** format and uploads them back to Blob Storage.
- **🔗 HTTP Trigger**: Can be triggered via HTTP `GET` or `POST` requests.

### 📜 Script Execution Strategies
The application utilizes various script execution strategies to determine whether a script should be executed based on its filename. These strategies enhance flexibility and allow the system to easily adapt to different execution schedules.

- **☀️ Daily Execution**: Scripts named with the suffix `script1_daily.sql` are executed daily.
- **📅 Weekly Execution**: Scripts suffixed with `weekly_` followed by the day of the week (e.g., `script2_weekly_monday.sql`) are executed on the specified day.
- **🗓️ Monthly Execution**: Scripts suffixed with `monthly_` followed by a day number (e.g., `script3_monthly_01.sql`) are executed on the specified day of each month.

## 💻 Simple UI
The application also provides a user interface (UI) where all scripts or individual scripts can be executed, and the results can be downloaded in CSV format.

![image](https://github.com/user-attachments/assets/a1b046d8-8a39-4243-88bc-a704e9ad2ea6)

## 📋 Requirements

- **🪙 Azure Account**: Required for setting up Function App and Blob Storage.
- **🗃️ Blob Storage**: SQL scripts must be uploaded to a blob container (e.g., `scripts`).
- **💾 SQL Database**: The function will execute the SQL scripts against a configured SQL database (MSSQL / PostgreSQL / MySQL).
- **⚙️ Azure Functions Core Tools**: For local development and testing.

## 🚦 Getting Started

### ✅ Prerequisites

1. **.NET Core SDK** (v6.0 or later)
2. **Azure Functions Core Tools**
3. **Azure Storage Account** with a blob container for SQL scripts.
4. **SQL Database** to execute the queries.

### 📥 Setup

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

   - **Blob Storage**: Set up the connection string for Azure Blob Storage in the `local.settings.json` file.
   - **SQL Database**: Configure the SQL connection string for executing scripts.

4. **Run the Function Locally**:

   ```bash
   func start



### 🌐 Deploy to Azure
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
  
### 📡 Usage
The function can be triggered via an HTTP GET or POST request. The endpoint will execute all SQL scripts stored in the scripts container in Blob Storage and upload the results as CSV files back to the same container.
```bash
curl -X POST https://<your-function-app-name>.azurewebsites.net/api/execute-scripts
```

### 🙌 Contributing
Feel free to submit issues or pull requests if you would like to contribute. Please ensure that any code changes are well-tested and documented.

### 📜 License
This project is licensed under the MIT License. See the LICENSE file for details.
   
