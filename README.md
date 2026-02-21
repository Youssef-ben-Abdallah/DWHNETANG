# NetDwhProject + salesbi-ui

Production-style full stack starter with Clean Architecture, repository pattern, ASP.NET Core API, Angular UI, OLTP + DWH SQL Server databases.

## Seed Accounts
- admin / Admin123!
- user / User123!

## Backend setup
1. Create DWH database `AdventureWorksDW_Sales` manually and run your provided `script.sql`.
2. Configure connection strings in `NetDwhProject.Api/appsettings.json`.
3. Run migrations for OLTP:
   ```bash
   dotnet ef migrations add InitialOltp --project NetDwhProject.Infrastructure --startup-project NetDwhProject.Api
   dotnet ef database update --project NetDwhProject.Infrastructure --startup-project NetDwhProject.Api
   ```
4. Run API:
   ```bash
   dotnet run --project NetDwhProject.Api
   ```

## Frontend setup
```bash
cd salesbi-ui
npm install
npm run start
```
App runs on `http://localhost:4200`.

## Notes
- DWH is read-only and queried with `AsNoTracking()`.
- Date defaults for analytics fallback to MIN/MAX order dates or 2011-05-01 to 2011-06-30.
- Controllers depend only on repository interfaces.
