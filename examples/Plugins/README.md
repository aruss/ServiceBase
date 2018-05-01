
# .NET CORE 2.0 - Plugin Architecture

This is a example application that utilizes most of bits and parts from the ServiceBase library, including the plugin architecture.

The Folder structure is as follows

```txt
Plugin Architecture /
├── Web Host/
│   └── Plugins/
│       ├── Plugin A/
│       └── Plugin B/
│       └── BaseTheme/
└── Shared Library/
```

    ❗ The `Plugins` folder HAS to be inside the Web Host application, since the compilation context for Razor does not work if it is outside.

## Building

To be able to use Visual Studio in old fashioned manner, like hit F5 and debug a crap out of it, you will need to add a reference to the plugin and add a static reference. Then you will not have to worry about starting and pausing the process with some fishy tricks like `Console.ReadLine()` and then attaching the debugger and so on.

## Publishing