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

## Test

Include `postman` file with sample request for testing [here](docs/api.postman_collection.json)

## Endpoints

### Liveness and HealthCheck

See [liveness](requests/liveness.http)
See [hc](requests/hc.http)

### Upload

See [upload](request/upload/upload.http)
