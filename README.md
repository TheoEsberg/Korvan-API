# Korvan-API

## Init database
First you need to create a Docker PostgreSQL Database, do that with the following docker command in terminal.
IMPORTANT: If running as published work, change all of the variables below :)
```powershell
docker run --name korvan-postgres -e POSTGRES_PASSWORD=supersecret -e POSTGRES_USER=korvanuser -e POSTGRES_DB=korvandb -p 5432:5432 -d postgres:16
```
