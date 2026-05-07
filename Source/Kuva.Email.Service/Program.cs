using Kuva.Email.Service.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddKeyVaultIfConfigured();
builder.Services.AddKuvaEmailService(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseKuvaEmailPipeline();

app.Run();

public partial class Program;
