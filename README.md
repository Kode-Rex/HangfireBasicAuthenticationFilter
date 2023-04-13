# NuGet

The dotnet core nuget package can be found at https://www.nuget.org/packages/Hangfire.Dashboard.Basic.Authentication

## About ## 

Hangfire.Dashboard.Basic.Authentication is a re-usable Hangfire Basic Authentication filter that can be easily imported and configured in your project. It provides a quick and easy way to add basic authentication to your Hangfire dashboard, without the need for complex configuration.

## Configurion ##

In your startup file, configure the Hangfire dashboard options and add the BasicAuthAuthorizationFilter:

```
using Hangfire.Dashboard.Basic.Authentication;

// ...

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // ...

    var options = new DashboardOptions
    {
        Authorization = new[]
        {
            new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
            {
                SslRedirect = false,
                RequireSsl = false,
                LoginCaseSensitive = true,
                Users = new []
                {
                    new BasicAuthAuthorizationUser
                    {
                        Login = "myuser",
                        PasswordClear = "mypassword"
                    }
                }
            })
        }
    };

    app.UseHangfireDashboard("/hangfire", options);
    app.UseHangfireServer();

    // ...
}
```

## Usage ##
Just replace my_username and my_password with your desired values. And tune the SSL setting for your needs. 
