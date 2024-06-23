# Using ConfigCat's Feature Flags in an ASP.NET Core Application

**[Read the blog post here](https://configcat.com/blog/2021/10/10/aspnetcore-options-pattern/)**

This repository complements the guide on integrating ConfigCat's feature management service with an ASP.NET Core 8 web API service and the `Options<T>` pattern.

## Build & Run

### Prerequisites

- Download and install the [.NET 8 SDK](https://dotnet.microsoft.com/en-us/learn/aspnet/blazor-tutorial/install).

- Check if the installation was successful with the following command:

```sh
dotnet --version
```

### Add your credentials

1. Edit the `appsettings.json` file and add your ConfigCat SDK Key.

2. Navigate to the [ConfigCat dashboard](https://app.configcat.com) and create new feature flag with the name **`myFeature`**.

### Run

1. Execute the following command to run the application and click the link printed to the terminal to launch the app in your browser:

```sh
dotnet watch
```

2. Append `/api/feature` to the URL to see the status of the feature flag.

## Learn more

Useful links to technical resources.

- [Introduction to .NET](https://dotnet.microsoft.com/en-us/learn/dotnet/what-is-dotnet)

- [Get started with .NET](https://learn.microsoft.com/en-us/dotnet/core/get-started) - learn how to create and run a "Hello World!" app with .NET.

[**ConfigCat**](https://configcat.com) also supports many other frameworks and languages. Check out the full list of supported SDKs [here](https://configcat.com/docs/sdk-reference/overview/).

You can also explore other code samples for various languages, frameworks, and topics here in the [ConfigCat labs](https://github.com/configcat-labs) on GitHub.

Keep up with ConfigCat on [X](https://x.com/configcat), [Facebook](https://www.facebook.com/configcat), [LinkedIn](https://www.linkedin.com/company/configcat/), and [GitHub](https://github.com/configcat).

## Authors

[Alex](https://github.com/bigmirc)

[Chavez](https://github.com/codedbychavez)

## Contributions

Contributions are welcome!
