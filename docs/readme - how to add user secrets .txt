# this will save secretsid tag to <propertyGroup>
dotnet user-secrets init
# the secret key will follow appsettings hirarchy
dotnet user-secrets set "ConnectionStrings:GamestoreContext" "------"
# list all secrets
dotnet user-secrets list
# IConfiguation now can read secret as extacly like defined in app settings

var connString = builder.Configuration.GetConnectionString("GamestoreContext");


