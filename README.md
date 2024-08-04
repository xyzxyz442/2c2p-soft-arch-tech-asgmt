# 2c2p-soft-arch-tech-asgmt

2C2P Software Architect Technical Assignment

## Architecture

See [further](docs)

## Development

```bash
# run
dotnet run --project src/Dev2C2P.Services/Platform/Platform.API

# watch mode
dotnet watch --non-interactive --project src/Dev2C2P.Services/Platform/Platform.API
```

For local development, use `docker-compose` to setup the environment by executing command below

```bash
# running detach mode
docker compose up -d

# force recreate
docker compose up --force-recreate
```

### Database

Run migrations from API project

```bash
# Initialize
dotnet ef migrations add InitialTransactionDbContext --context TransactionDbContext --project ../Platform.Infrastructure/Platform.Infrastructure.csproj

# Cleanup
dotnet ef migrations remove --context TransactionDbContext --project ../Platform.Infrastructure/Platform.Infrastructure.csproj

# Update Database
dotnet ef database update --project ../Platform.Infrastructure/Platform.Infrastructure.csproj
```

> > Using can ignore the error when executing the script.

## Test

Include `postman` file with samples request for testing [here](docs/api.postman_collection.json)

## Endpoints

### Liveness and HealthCheck

See [liveness](requests/liveness.http)
See [hc](requests/hc.http)

### Upload

See [upload](request/upload/upload.http)
