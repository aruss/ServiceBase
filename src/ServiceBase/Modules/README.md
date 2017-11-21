# .NET CORE module support

Add following to your appsettings.json

```
{
  "Modules": [
    { "Type": "Some.Fancy.Namespace.FooModule, SomeFancyAssembly" }
    { "Type": "Some.Fancy.Namespace.BarModule, SomeFancyAssembly" }
  ]
}
```

Your Startup.cs would look like this

```
public class Startup : IStartup
{
    private readonly ModulesStartup _modulesStartup;

    public Startup(IConfiguration configuration)
    {
        this._modulesStartup = new ModulesStartup(configuration);
    }

    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        // Add your services here

        this._modulesStartup.ConfigureServices(services);

        return services.BuildServiceProvider();
    }

    public virtual void Configure(IApplicationBuilder app)
    {
        // Configure your servies here

        this._modulesStartup.Configure(app);
    }
}
```

If a module class can not be found the ModulesStartup class will throw
TypeLoadException containing the information about the assembly it tries to load.