
# ASP.NET Core Utilities

## Route translations

### How to use it:
#### In Startup.cs
Under ConfigureServices, you need to add the following:
```c#
public void ConfigureServices(IServiceCollection services)
{
    var builder = services.AddControllersWithViews(options =>
    {
        // Add filter to set current filter in cookies
        options.AddCultureCookieFilter();
    });
    
    services.AddLocalization(options => options.ResourcesPath = "Resources");
    builder.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
    builder.AddDataAnnotationsLocalization();

    // Inject required services, add routing and replace current LinkGenerator
    services.AddRoutingLocalization(_configuration);
}
```
Under Configure, you need to add the following:
```c#
public void Configure(IApplicationBuilder app)
{
    // Setup Request localization, Rewriter and Routing
    app.UseRoutingLocalization();
    app.UseStaticFiles();
    
    // Setup Endpoints with culture
    app.UseEndpointsLocalization();
}
```

#### In appsettings.json
You need to add the following structure in your appsettings:
```json
{
  "RoutingTranslation":
  {
    "SupportedCultures": "fr, en",
    "DefaultCulture": "fr"
  }
}
```

#### Attribute in Controllers
```c#
[Translate("en", "orders")]
[Translate("fr", "commandes")]
public class OrdersController : Controller
{
    [Translate("fr", "liste")]
    public IActionResult List()
    {
        return View();
    }
}
```

#### Generate your first urls
```c#
<form asp-controller="orders" asp-action="list"></form>

<a asp-controller="products" asp-action="detail" asp-route-id="12"></a>

<a asp-route-culture="fr">French</a>
```

### Options
You can inject an IOptions that contains your configured cultures. Use the following:
```c#
IOptions<RequestLocalizationOptions> options
```

### Custom route translation
You can create your own custom validation by deriving from ICustomTranslation:
```c#
public class ProductTranslation : ICustomTranslation
{    
    public string ControllerName => "products";
    
    public string ActionName => "detail";
    
    public RewriteRule[] RewriteRules => new[]
    {
        new RewriteRule(
            @"^([-a-zA-Z]+)\/([^\/]+)\/[-\/a-zA-Z0-9]+\/p-([=._a-zA-Z0-9]+)-.*$",
            "$1/$2/detail/$3")
    };
    
    public string GenerateUrlPath(RouteValueDictionary values, FragmentString fragment)
    {
        return "/" +
           $"{values.GetParameterValue(RouteValue.Culture)}/" +
           $"{values.GetParameterValue(RouteValue.Controller)}/" + 
           "10-control-and-testing/" +
           "14-testing-string/" +
           $"p-{values.GetParameterValue(RouteValue.Id)}-testing-product-string";
    }
}
```
Then add it as a scoped in Startup.cs:
```c#
// Add custom translations as singleton
public void ConfigureServices(IServiceCollection services)
{
    services.AddScoped<ICustomTranslation, ProductTranslation>();
}
```