---
uid: getting_started
title: Getting Started with YARP
---

# Getting Started with YARP

YARP is designed as a library that provides the core proxy functionality which you can then customize by adding or replacing modules. YARP is currently provided as a NuGet package and code snippets. We plan on providing a project template and pre-built exe in the future. 

YARP is implemented on top of .NET Core infrastructure and is usable on Windows, Linux or MacOS. Development can be done with the SDK and your favorite editor, [Microsoft Visual Studio](https://visualstudio.microsoft.com/vs/) or [Visual Studio Code](https://code.visualstudio.com/).

YARP 1.0.0 Preview 11 supports ASP.NET Core 3.1 and 5.0. You can download the .NET 5 SDK from https://dotnet.microsoft.com/download/dotnet/5.0.
Visual Studio support for .NET 5 is included in Visual Studio 2019 v16.8 or newer.

A fully commented variant of the getting started app can be found at [Basic YARP Sample](https://github.com/microsoft/reverse-proxy/tree/main/samples/BasicYarpSample)

### Create a new project

Start by creating an "Empty" ASP.NET Core application using the command line:

```
dotnet new web -n MyProxy 
```

use `-f` to specify `netcoreapp3.1` or `net5.0`.

Or create a new ASP.NET Core web application in Visual Studio, and choose "Empty" for the project template. 

### Update the project file

Open the Project and make sure it includes the appropriate target framework: 
 
 ```XML
<PropertyGroup>
  <TargetFramework>net5.0</TargetFramework>
</PropertyGroup> 
```

And then add:
 
 ```XML
<ItemGroup> 
  <PackageReference Include="Yarp.ReverseProxy" Version="1.0.0-preview.11.*" />
</ItemGroup> 
```

### Update Startup

YARP is implemented as a ASP.NET Core component, and so the majority of the sample code is in Startup.cs. 

YARP can use configuration files or a custom provider to define the routes and endpoints for the proxy. This sample uses config files and is initialized in the `ConfigureServices` method. 

The `Configure` method defines the ASP.NET pipeline for processing requests. The reverse proxy is plugged into ASP.NET endpoint routing, and then has its own sub-pipeline for the proxy. Here proxy pipeline modules, such as load balancing, can be added to customize the handling of the request. 

```C#
public IConfiguration Configuration { get; }

public Startup(IConfiguration configuration)
{
    Configuration = configuration;
}

public void ConfigureServices(IServiceCollection services) 
{ 
    // Add the reverse proxy to capability to the server
    var proxyBuilder = services.AddReverseProxy();
    // Initialize the reverse proxy from the "ReverseProxy" section of configuration
    proxyBuilder.LoadFromConfig(Configuration.GetSection("ReverseProxy"));
} 

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    
    // Enable endpoint routing, required for the reverse proxy
    app.UseRouting();
    // Register the reverse proxy routes
    app.UseEndpoints(endpoints => 
    {
        endpoints.MapReverseProxy(); 
    }); 
} 
```
 
### Configuration 

The configuration for YARP is defined in the appsettings.json file. See [Configuration Files](configfiles.md) for details.

The configuration can also be provided programmatically. See [Configuration Providers](configproviders.md) for details.

You can find out more about the available configuration options by looking at [ProxyRoute](xref:Yarp.ReverseProxy.Abstractions.ProxyRoute) and [Cluster](xref:Yarp.ReverseProxy.Abstractions.Cluster).
 
 ```JSON
 {
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "ReverseProxy": {
    "Routes": {
      "route1" : {
        "ClusterId": "cluster1",
        "Match": {
          "Path": "{**catch-all}"
        },
      }
    },
    "Clusters": {
      "cluster1": {
        "Destinations": {
          "cluster1/destination1": {
            "Address": "https://example.com/"
          }
        }
      }
    }
  }
}
```

### Running the project

Use `dotnet run` called within the sample's directory or `dotnet run --project <path to .csproj file>`
