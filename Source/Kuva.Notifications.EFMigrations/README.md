# Kuva.Notifications EF Migrations

Migrations for `Kuva.Notifications.Repository.Context.NotificationsDbContext` are centralized in this project.

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
