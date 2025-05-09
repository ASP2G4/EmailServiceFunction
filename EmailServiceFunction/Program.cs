using Azure.Communication.Email;
using EmailServiceFunction.Models;
using EmailServiceFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.Configure<AzureCommunicationsSettings>(builder.Configuration.GetSection("AzureCommunicationServices"));
builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AzureCommunicationsSettings>>().Value;
    return new EmailClient(settings.ConnectionString);
});
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Build().Run();
