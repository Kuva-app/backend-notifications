# Kuva.Notifications

Microservico transacional de notificacoes do ecossistema Kuva. Ele recebe pedidos de envio, valida destinatarios e variaveis, renderiza templates armazenados no banco, seleciona um provider ativo e registra status, tentativas e eventos de auditoria.

## Arquitetura

![arquitetura](https://github.com/Kuva-app/docs/blob/master/kuva-arquitetura-mvp.drawio.png?raw=true)

```text
Kuva.Notifications.Service -> Kuva.Notifications.Business -> Kuva.Notifications.Repository -> SQL Server
                                  |
                                  -> canais/providers (Email via Fake, SMTP ou SendGrid no MVP)
```

Projetos: 

- `Source/Kuva.Notifications.Entities`: entidades, enums, DTOs e value objects.
- `Source/Kuva.Notifications.Repository`: EF Core, Fluent API, repositorios e Unit of Work.
- `Source/Kuva.Notifications.Business`: validacao, renderizacao, idempotencia e envio.
- `Source/Kuva.Notifications.Service`: Web API, Swagger, health checks, metrics, Key Vault e middlewares.
- `Source/Kuva.Notifications.EFMigrations`: migrations do EF Core.
- `Source/Kuva.Notifications.Tests`: testes NUnit com Moq, FluentAssertions e EF InMemory.

## Rodando local

Requisitos:

- .NET SDK 10
- Docker e Docker Compose
- SQL Server local ou o container do `docker-compose.yml`

Comandos principais:

```bash
dotnet restore Source/Kuva.Notifications.sln
dotnet build Source/Kuva.Notifications.sln
dotnet test Source/Kuva.Notifications.sln
docker compose up --build
```

Com Docker Compose:

```bash
export SA_PASSWORD='Change_this_password_123!'
docker compose up --build
```

```powershell
$env:SA_PASSWORD = "Change_this_password_123!"
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
dotnet user-secrets init --project Source/Kuva.Notifications.Service
dotnet user-secrets set "ConnectionStrings:NotificationsDatabase" "<connection-string>" --project Source/Kuva.Notifications.Service
dotnet user-secrets set "Smtp:Password" "<password>" --project Source/Kuva.Notifications.Service
dotnet user-secrets set "SendGrid:ApiKey" "<api-key>" --project Source/Kuva.Notifications.Service
```

## Azure Key Vault

Configure `KeyVault:Uri` por variavel de ambiente ou app settings do Azure. O provider usa `DefaultAzureCredential`.

Segredos esperados:

```text
ConnectionStrings--NotificationsDatabase
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
  --project Source/Kuva.Notifications.EFMigrations \
  --startup-project Source/Kuva.Notifications.Service \
  --context NotificationsDbContext

dotnet ef database update \
  --project Source/Kuva.Notifications.EFMigrations \
  --startup-project Source/Kuva.Notifications.Service \
  --context NotificationsDbContext
```

A migration inicial cria as tabelas, indices e seed para provider fake e os templates `ORDER_RECEIVED` e `ORDER_READY_FOR_PICKUP`.

## Endpoints

```http
POST /api/v1/notifications/send
GET /api/v1/notifications/{id}
GET /api/v1/templates/{code}
POST /api/v1/templates
PUT /api/v1/templates/{id}
PATCH /api/v1/templates/{id}/status
GET /health
GET /health/live
GET /health/ready
GET /metrics
```

Payload de envio:

```json
{
  "type": "Email",
  "templateId": "7a40f1f4-5e62-4427-90c0-b89ac3cd0002",
  "externalReference": "order-123",
  "source": "Kuva.Orders",
  "priority": "Normal",
  "recipients": [
    {
      "address": "cliente@email.com",
      "name": "Cliente Kuva",
      "role": "To"
    }
  ],
  "variables": {
    "customerName": "Cliente Kuva",
    "orderNumber": "123",
    "storeName": "Loja Piloto"
  },
  "metadata": {
    "orderId": "123",
    "storeId": "456"
  }
}
```

## Variaveis de ambiente

- `ASPNETCORE_ENVIRONMENT`
- `ASPNETCORE_URLS`
- `ConnectionStrings__NotificationsDatabase`
- `Notifications__Provider`
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

- `kuva_notifications_requests_total`
- `kuva_notifications_sent_total`
- `kuva_notifications_failed_total`
- `kuva_notifications_template_not_found_total`
- `kuva_notifications_invalid_variables_total`
- `kuva_notifications_send_duration_seconds`
- `kuva_notifications_provider_failures_total`

Arquivos:

- `prometheus.yml`
- `alert_rules.yml`

## Decisoes tecnicas

- Envio sincrono controlado no MVP, com arquitetura pronta para evoluir para worker e fila.
- Templates persistidos no banco para alteracao sem redeploy.
- Renderizacao simples de `{{variableName}}`, sem engine que execute codigo.
- Idempotencia logica por `type + templateId + externalReference + primary recipient address` em janela de 24 horas.
- Corpo completo da notificacao e variaveis sensiveis nao sao registrados em log.
- Endpoints administrativos de template preparados com `[Authorize]`.

## Troubleshooting

- `NETSDK1045`: instale o SDK .NET 10. O projeto mira `net10.0`.
- Falha de SQL no health check: verifique `ConnectionStrings:NotificationsDatabase`.
- Provider SMTP/SendGrid falhando: confirme secrets no User Secrets, variaveis de ambiente ou Key Vault.
- `401` em endpoints de template: configure `Jwt:Authority` e `Jwt:Audience` ou use um token valido do ambiente interno.
