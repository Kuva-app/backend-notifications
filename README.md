# Kuva.Email

Microservico transacional de e-mails do ecossistema Kuva. Ele recebe pedidos de envio, valida destinatarios e variaveis, renderiza templates armazenados no banco, seleciona um provider ativo e registra status, tentativas e eventos de auditoria.

## Arquitetura

```text
Kuva.Email.Service -> Kuva.Email.Business -> Kuva.Email.Repository -> SQL Server
                                  |
                                  -> Fake, SMTP ou SendGrid provider
```

Projetos:

- `Source/Kuva.Email.Entities`: entidades, enums, DTOs e value objects.
- `Source/Kuva.Email.Repository`: EF Core, Fluent API, repositorios e Unit of Work.
- `Source/Kuva.Email.Business`: validacao, renderizacao, idempotencia e envio.
- `Source/Kuva.Email.Service`: Web API, Swagger, health checks, metrics, Key Vault e middlewares.
- `Source/Kuva.Email.EFMigrations`: migrations do EF Core.
- `Source/Kuva.Email.Tests`: testes NUnit com Moq, FluentAssertions e EF InMemory.

## Rodando local

Requisitos:

- .NET SDK 10
- Docker e Docker Compose
- SQL Server local ou o container do `docker-compose.yml`

Comandos principais:

```bash
dotnet restore Source/Kuva.Email.sln
dotnet build Source/Kuva.Email.sln
dotnet test Source/Kuva.Email.sln
docker compose up --build
```

Com Docker Compose:

```bash
export SA_PASSWORD='Change_this_password_123!'
docker compose up --build
```

Servicos:

- API: `http://localhost:8080`
- Swagger UI: `http://localhost:8080/swagger`
- Health: `http://localhost:8080/health`
- Metrics: `http://localhost:8080/metrics`
- Prometheus: `http://localhost:9090`

## User Secrets

Nao coloque segredos em `appsettings.json`. Para desenvolvimento local:

```bash
dotnet user-secrets init --project Source/Kuva.Email.Service
dotnet user-secrets set "ConnectionStrings:EmailDatabase" "<connection-string>" --project Source/Kuva.Email.Service
dotnet user-secrets set "Smtp:Password" "<password>" --project Source/Kuva.Email.Service
dotnet user-secrets set "SendGrid:ApiKey" "<api-key>" --project Source/Kuva.Email.Service
```

## Azure Key Vault

Configure `KeyVault:Uri` por variavel de ambiente ou app settings do Azure. O provider usa `DefaultAzureCredential`.

Segredos esperados:

```text
ConnectionStrings--EmailDatabase
Smtp--Host
Smtp--Port
Smtp--Username
Smtp--Password
Smtp--FromEmail
Smtp--FromName
SendGrid--ApiKey
ServiceBus--ConnectionString
ApplicationInsights--ConnectionString
```

## Migrations

```bash
dotnet ef migrations add InitialCreate \
  --project Source/Kuva.Email.EFMigrations \
  --startup-project Source/Kuva.Email.Service \
  --context EmailDbContext

dotnet ef database update \
  --project Source/Kuva.Email.EFMigrations \
  --startup-project Source/Kuva.Email.Service \
  --context EmailDbContext
```

A migration inicial cria as tabelas, indices e seed para provider fake e os templates `ORDER_RECEIVED` e `ORDER_READY_FOR_PICKUP`.

## Endpoints

```http
POST /api/v1/emails/send
GET /api/v1/emails/{id}
GET /api/v1/templates/{code}
POST /api/v1/templates
PUT /api/v1/templates/{id}
PATCH /api/v1/templates/{id}/status
GET /health
GET /health/live
GET /health/ready
GET /metrics
```

## Variaveis de ambiente

- `ASPNETCORE_ENVIRONMENT`
- `ASPNETCORE_URLS`
- `ConnectionStrings__EmailDatabase`
- `Email__Provider`
- `KeyVault__Uri`
- `Smtp__Host`
- `Smtp__Port`
- `Smtp__Username`
- `Smtp__Password`
- `Smtp__FromEmail`
- `Smtp__FromName`
- `SendGrid__ApiKey`
- `Jwt__Authority`
- `Jwt__Audience`

## Observabilidade

Metricas Prometheus:

- `kuva_email_requests_total`
- `kuva_email_sent_total`
- `kuva_email_failed_total`
- `kuva_email_template_not_found_total`
- `kuva_email_invalid_variables_total`
- `kuva_email_send_duration_seconds`
- `kuva_email_provider_failures_total`

Arquivos:

- `prometheus.yml`
- `alert_rules.yml`

## Decisoes tecnicas

- Envio sincrono controlado no MVP, com arquitetura pronta para evoluir para worker e fila.
- Templates persistidos no banco para alteracao sem redeploy.
- Renderizacao simples de `{{variableName}}`, sem engine que execute codigo.
- Idempotencia logica por `templateCode + externalReference + primary recipient` em janela de 24 horas.
- Corpo completo do e-mail e variaveis sensiveis nao sao registrados em log.
- Endpoints administrativos de template preparados com `[Authorize]`.

## Troubleshooting

- `NETSDK1045`: instale o SDK .NET 10. O projeto mira `net10.0`.
- Falha de SQL no health check: verifique `ConnectionStrings:EmailDatabase`.
- Provider SMTP/SendGrid falhando: confirme secrets no User Secrets, variaveis de ambiente ou Key Vault.
- `401` em endpoints de template: configure `Jwt:Authority` e `Jwt:Audience` ou use um token valido do ambiente interno.
