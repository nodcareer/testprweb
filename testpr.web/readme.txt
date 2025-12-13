  "ConnectionStrings": {
    "AzureBlobStorage": ""
    "AzureStorageQueue":""
    "AzureSqlDatabase": ""
  
  }

 # ef tool
 dotnet tool install --global dotnet-ef

  # run migrations: 
  dotnet ef migrations add InitialCreate 
  dotnet ef database update

