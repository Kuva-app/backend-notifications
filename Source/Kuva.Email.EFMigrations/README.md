# Kuva.Email EF Migrations

Migrations for `Kuva.Email.Repository.Context.EmailDbContext` are centralized in this project.

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
