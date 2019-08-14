
# .NET CORE 2.2 - Plugin Architecture

This is a example application that utilizes most of bits and parts from the ServiceBase library, including the plugin architecture.


Key features

- Dynamic Plugin Loading
- Localization
- Theming


The Folder structure is as follows

```txt
├── Web Host/
│   └── Plugins/
│       ├── Plugin A/
│       └── Plugin B/
│       └── BaseTheme/
└── Shared Library/
```

    ❗ The `Plugins` folder HAS to be inside the Web Host application, since the compilation context for Razor does not work if it is outside. 
    
    Or at least thats what I understood


Simple Theme Plugin would look like follows

```txt
├── Public/
│   ├── css/
│   │   └── main.css
│   └── js/
│       └── main.js
├── Resources/
│   └── Localization/
│       ├── Shared.de-DE.json
│       └── Shared.en-US.json
├── Views/
│   ├── Home/
│   │   └── Home.cshtml
│   ├── Shared/
│   │   └── _Layout.cshtml
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── gulpfile.js
└── package.js
```

## Actions

This is a kinda hook pattern, so you can implement a method and it will be executed in a centralized place from plugin architecture.

### ConfigureServicesAction
```csharp
namespace ThemeA
{
    using Microsoft.Extensions.DependencyInjection;
    using ServiceBase.Mvc.Plugins;

    public class ConfigureServicesAction : IConfigureServicesAction
    {
        public void Execute(IServiceCollection services)
        {
            // Add here your services
        }
    }
}
```

### ConfigureAction

```csharp
namespace PluginFoo
{
    using Microsoft.AspNetCore.Builder;
    using ServiceBase.Mvc.Plugins;

    public class ConfigureAction : IConfigureAction
    {
        public void Execute(IApplicationBuilder applicationBuilder)
        {
            // Configure here your services
        }
    }
}
```

### IAddMvcAction

### IUseMvcAction


## Building

To be able to use Visual Studio in old fashioned manner, like hit F5 and debug a crap out of it, you will need to add a reference to the plugin and add a static reference. Then you will not have to worry about starting and pausing the process with some fishy tricks like `Console.ReadLine()` and then attaching the debugger and so on.

## Publishing

tbd;