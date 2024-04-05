// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Azure.Identity;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.OpenApi.Models;
using NOITechParkDoorSignage.Application.ActionFilters;
using NOITechParkDoorSignage.Application.BackgroundJobs;
using NOITechParkDoorSignage.Application.Models;
using NOITechParkDoorSignage.Application.Services.Impl;
using NOITechParkDoorSignage.Application.Services.Interfaces;
using NOITechParkDoorSignage.Domain.Room;
using NOITechParkDoorSignage.Infrastructure.Data;
using NOITechParkDoorSignage.Infrastructure.Data.Impl;
using NOITechParkDoorSignage.Infrastructure.Data.Interfaces;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.BuildAndReplacePlaceholders();

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<EFRoomContext>(options => options.UseInMemoryDatabase(databaseName: "CalendarDB"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Add a header parameter to all API operations
    c.OperationFilter<HeaderParameterOperationFilter>();
});


builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage();
});
builder.Services.AddHangfireServer();

builder.Services.AddScoped<ICalendarSourceService, GraphCalendarSourceService>();
builder.Services.AddScoped<IUnitOfWork, EFUnitOfWork>();
builder.Services.AddScoped<IRoomRepository, EFRoomRepository>();
builder.Services.AddScoped<IRoomService, EFRoomService>();
builder.Services.AddSingleton<ILabelService, ConfigurationLabelServices>();


SetupMicrosoftGraph();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "NOITechParkDoorSignage API");
});

BootstrapDatabase();

ConfigureHangfireDashboard();

// Start recurrent Hangfire Job to query Microsoft Graph API
RecurringJob.AddOrUpdate<MicrosoftGraphJob>("MicrosoftGraphJob", x => x.SyncOffice365Data(), Cron.Minutely);
RecurringJob.TriggerJob("MicrosoftGraphJob");

app.UseAuthorization();

app.MapControllers();


app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/room"), appBuilder =>
{
    app.UseMiddleware<LabelMiddleware>();
});

app.Run();

void ConfigureHangfireDashboard()
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var configuration = services.GetRequiredService<IConfiguration>();

        app.UseHangfireDashboard("/jobs", options: new DashboardOptions
        {
            DashboardTitle = "NOITechParkDoorSignage Jobs",
            Authorization = new[] { new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
            {
                RequireSsl = false,
                SslRedirect = false,
                LoginCaseSensitive = true,
                Users = new []
                {
                    new BasicAuthAuthorizationUser
                    {
                        Login = "admin",
                        PasswordClear =  configuration.GetValue<string>("Hangfire:ManagementPassword")
                    }
                }

            })
            }
        });
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Cannot configure Hangfire Dashboard.");
    }
}

void BootstrapDatabase()
{

    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var configuration = services.GetRequiredService<IConfiguration>();

        // Insert all Room entities by read the configuration
        var roomConfigurationOption = new ConfigurationRoomsOption();
        roomConfigurationOption.Rooms = configuration.GetSection("Rooms").Get<List<RoomOption>>();

        if (roomConfigurationOption.Rooms == null)
        {
            throw new Exception("No rooms found in configuration");
        }

        if (roomConfigurationOption.Rooms.Count > 0)
        {
            var context = services.GetRequiredService<EFRoomContext>();
            context.Database.EnsureCreated();

            foreach (var room in roomConfigurationOption.Rooms)
            {
                var roomEntity = new Room
                {
                    Email = room.Email,
                    DisplayName = room.Name,
                    Location = room.Location
                };
                context.Rooms.Add(roomEntity);
            }
            context.SaveChanges();
        }
        else
        {
            Log.Warning("No rooms found in configuration");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}
void SetupMicrosoftGraph()
{
    // The client credentials flow requires that you request the
    // /.default scope, and pre-configure your permissions on the
    // app registration in Azure. An administrator must grant consent
    // to those permissions beforehand.
    var scopes = new[] { "https://graph.microsoft.com/.default" };

    // Values from app registration
    var clientId = builder.Configuration.GetValue<string>("MicrosoftGraph:ClientId");
    var tenantId = builder.Configuration.GetValue<string>("MicrosoftGraph:TenantId");
    var clientSecret = builder.Configuration.GetValue<string>("MicrosoftGraph:ClientSecret");

    // using Azure.Identity;
    var options = new ClientSecretCredentialOptions
    {
        AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
    };

    // https://learn.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
    var clientSecretCredential = new ClientSecretCredential(
        tenantId, clientId, clientSecret, options);

    var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

    builder.Services.AddSingleton<GraphServiceClient>(graphClient);
}

class HeaderParameterOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

        // Add a header parameter
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "label-id",
            In = ParameterLocation.Header,
            Required = true, // Set to true if the header is required,
            AllowEmptyValue = false,
            Description = "The label ID of the room (usally it MAC address)",
        });
    }
}