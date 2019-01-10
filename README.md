# HangfireBasicAuthenticationFilter
The dotnet core nuget package can be found at https://www.nuget.org/packages/Hangfire.Dashboard.Basic.Authentication

## Usage ##
Just replace my_username and my_password with your desired values.

```
app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization = new[] { new HangfireCustomBasicAuthenticationFilter{User="my_username",Pass="my_password" }
});
```
