# ECommerce

## About this solution

This is a layered startup solution based on [Domain Driven Design (DDD)](https://abp.io/docs/latest/framework/architecture/domain-driven-design) practises. All the fundamental ABP modules are already installed. Check the [Application Startup Template](https://abp.io/docs/latest/solution-templates/layered-web-application) documentation for more info.

### Pre-requirements

* [.NET10.0+ SDK](https://dotnet.microsoft.com/download/dotnet)
* [Node v18 or 20](https://nodejs.org/en)

### Configurations

The solution comes with a default configuration that works out of the box. However, you may consider to change the following configuration before running your solution:

* **PostgreSQL:** The default connection string is `Host=localhost;Port=5432;Database=ECommerce;User ID=postgres;Password=postgres;`. Override it in `appsettings.json` or `appsettings.secrets.json` (in both `ECommerce.HttpApi.Host` and `ECommerce.DbMigrator`) to match your local PostgreSQL. Run DbMigrator from `src/ECommerce.DbMigrator` (or from solution root: the migrator uses the output directory for config) after PostgreSQL is running to apply migrations.

### Before running the application

* Run `abp install-libs` in the solution folder if you cloned the repo (client-side dependencies).
* **Apply migrations:** Ensure PostgreSQL is running and the connection string in `appsettings.json` (or `appsettings.secrets.json`) is correct, then run: `dotnet run --project src/ECommerce.DbMigrator/ECommerce.DbMigrator.csproj` (or run from `src/ECommerce.DbMigrator`). This creates the database and seeds initial data.
* **Run the backend:** `dotnet run --project src/ECommerce.HttpApi.Host/ECommerce.HttpApi.Host.csproj`
* **Run the Angular app:** From `angular` folder run `npm start` (dev server at http://localhost:4200).

#### Generating a Signing Certificate

In the production environment, you need to use a production signing certificate. ABP Framework sets up signing and encryption certificates in your application and expects an `openiddict.pfx` file in your application.

To generate a signing certificate, you can use the following command:

```bash
dotnet dev-certs https -v -ep openiddict.pfx -p d41bad50-96fc-45aa-b871-4d2bcddb56c6
```

> `d41bad50-96fc-45aa-b871-4d2bcddb56c6` is the password of the certificate, you can change it to any password you want.

It is recommended to use **two** RSA certificates, distinct from the certificate(s) used for HTTPS: one for encryption, one for signing.

For more information, please refer to: [OpenIddict Certificate Configuration](https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html#registering-a-certificate-recommended-for-production-ready-scenarios)

> Also, see the [Configuring OpenIddict](https://abp.io/docs/latest/Deployment/Configuring-OpenIddict#production-environment) documentation for more information.

### Solution structure

This is a layered monolith application that consists of the following applications:

* `angular`: Angular application.
* `ECommerce.DbMigrator`: A console application which applies the migrations and also seeds the initial data. It is useful on development as well as on production environment.
* `ECommerce.HttpApi.Host`: ASP.NET Core API application that is used to expose the APIs to the clients.

### Localization boundaries

- Keep ABP's built-in `GET /api/abp/application-localization` endpoint unchanged (no custom controller override).
- This endpoint serves only ABP resource texts (for example `Localization/ECommerce` JSON values and module resource strings).
- Multi-lingual catalog entity content (such as category/brand/product translated data) must be returned by catalog application service APIs, not by the ABP application-localization endpoint.

#### Test Projects

The `test` folder contains the following test projects:

* `ECommerce.Application.Tests`: Application layer tests.
* `ECommerce.Domain.Tests`: Domain layer tests.
* `ECommerce.EntityFrameworkCore.Tests`: Entity Framework Core integration tests.




## Deploying the application

Deploying an ABP application follows the same process as deploying any .NET or ASP.NET Core application. However, there are important considerations to keep in mind. For detailed guidance, refer to ABP's [deployment documentation](https://abp.io/docs/latest/Deployment/Index).

### Additional resources


#### Internal Resources

You can find detailed setup and configuration guide(s) for your solution below:

* [Angular](./angular/README.md)

#### External Resources
You can see the following resources to learn more about your solution and the ABP Framework:

* [Web Application Development Tutorial](https://abp.io/docs/latest/tutorials/book-store/part-1)
* [Application Startup Template](https://abp.io/docs/latest/startup-templates/application/index)
