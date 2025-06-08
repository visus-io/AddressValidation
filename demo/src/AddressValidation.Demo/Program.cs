namespace AddressValidation.Demo;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Extensions;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Abstractions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Services;
using Services.Abstractions;
using Visus.AddressValidation.Integration.FedEx.Extensions;
using Visus.AddressValidation.Integration.Google.Extensions;
using Visus.AddressValidation.Integration.PitneyBowes.Extensions;
using Visus.AddressValidation.Integration.Ups.Extensions;

[SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task")]
[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
internal static class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .Enrich.WithThreadId()
                    .Enrich.WithMachineName()
                    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture, theme: AnsiConsoleTheme.Code)
                    .WriteTo.ApplicationInsights(TelemetryConverter.Traces)
                    .CreateBootstrapLogger();

        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(builder.Environment.ContentRootPath, "Data"));

            builder.Configuration.AddSqlite();

            builder.Services.AddDataProtection()
                   .SetApplicationName("AddressValidation.Demo")
                   .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "Data", "Keys")));

            builder.Services.AddApplicationInsightsTelemetry();

            builder.Services.AddDbContextFactory<GeoContext, GeoContextFactory>();
            builder.Services.AddDbContextFactory<SettingsContext, SettingsContextFactory>();

            builder.Services.AddRadzenComponents();
            builder.Services.AddRazorComponents().AddInteractiveServerComponents();

            builder.Services.AddScoped<ICountryRepository, CountryRepository>();
            builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
            builder.Services.AddScoped<IStateRepository, StateRepository>();

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddFedExAddressValidation();
            builder.Services.AddGoogleAddressValidation();
            builder.Services.AddPitneyBowesAddressValidation();
            builder.Services.AddUpsAddressValidation();

            builder.Services.AddTransient<IGeographyService, GeographyService>();
            builder.Services.AddScoped<ILoadingIndicatorService, LoadingIndicatorService>();
            builder.Services.AddTransient<ISettingsService, SettingsService>();

            builder.Services.AddSerilog();

            WebApplication app = builder.Build();

            if ( !app.Environment.IsDevelopment() )
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

            using ( IServiceScope scope = app.Services.CreateScope() )
            {
                IDbContextFactory<SettingsContext> contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<SettingsContext>>();

                await using SettingsContext dbContext = await contextFactory.CreateDbContextAsync();
                await dbContext.Database.EnsureCreatedAsync();
            }

            await app.RunAsync();
        }
        catch ( Exception e )
        {
            Log.Fatal(e, "Host terminated unexpectedly.");
            Environment.ExitCode = 1;
        }
        finally
        {
            await Task.Delay(1000);
            await Log.CloseAndFlushAsync();
        }
    }
}
