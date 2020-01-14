# Asp_authentication
chat messaging using both cookie and jwt authentication in asp.net core 3.1

# Requirements:
#### MySql
#### Asp.net Core 3.1

# How to use
Create an appsettings.json file with your database jwt secret and username and password:
```json
{
  "JwtAuthentication": {
    "Secret": "Secret_string_wow_LLLLLLLLLL",
    "Issuer": "https://localhost:44367/",
    "Audience": "https://localhost:44367/"
  },
  "MySql": {
    "ConnectionString": "server=localhost;database=aspauth;user=root;password=super_secure_Password2@2"
  },
}
```
Update the database
------------------------------------------------------------------------------------------
Bring up a command window in the project directory and run:

```console dotnet ef migrations add Init && dotnet ef database update ```

Test using one of the users specified in /Data/UserDataSeeder.cs

Use the admin panel to login and authenticate via cookies
------------------------------------------------------------------------------------------
localhost:44367/api/Admin/Login/ 

Use the api to authenticate via Jwt instead of cookies at:
------------------------------------------------------------------------------------------
localhost:44367/api/User/Authenticate/ 

Post body: {username, password}

Use the api to get list of messages:
------------------------------------------------------------------------------------------
localhost:44367/api/Messages/
