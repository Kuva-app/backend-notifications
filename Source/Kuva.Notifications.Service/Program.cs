using Kuva.Notifications.Service.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddKeyVaultIfConfigured();
builder.Services.AddKuvaNotificationsService(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseKuvaNotificationsPipeline();

await app.RunAsync();