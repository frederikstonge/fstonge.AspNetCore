
# ASP.NET Core Utilities

## Async Distribued Session

### How to use it:
#### In Startup.cs
Under ConfigureServices, you need to add the following:
```c#
public void ConfigureServices(IServiceCollection services)
{
    var builder = services.AddControllersWithViews();
    
    // Store temp data in session
    builder.AddSessionStateTempDataProvider();

    // Add any distributed cache (this is in memory, but I can be Redis or others)
    services.AddDistributedMemoryCache();

    // Use the package to use the async distributed session
    services.AddAsyncDistributedSession();
}
```
Under Configure, you need to add the following:
```c#
public void Configure(IApplicationBuilder app)
{
    
    // Setup async distributed session
    app.UseAsyncDistributedSession();

    app.UseEndpoints();
}
```