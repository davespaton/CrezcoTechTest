# CrezcoTechTest

Simple location-from-ip wrapper around: https://freeipapi.com/

## Overview

Can be executed by running `docker-compose up -d`

- API can be access via [Swagger](https://localhost:5001/swagger/index.html)
- External location API call made to [FreeApi](https://freeipapi.com/)
- Persistence using MongoDB
- Caching using both Redis and HTTP response caching

### Other notes

- Structured logging provided using Serilog
- External API retry logic using Polly
- If the cache is unavailable, data will still be written to storage and returned to caller
- Tests will require MongoDB and Redis to be running

### Missing features / Incomplete features

- MongoDB retry / outage logic
- Cache
  - Cache could implement memory cache and distributed cache
  - Cache could accept more detailed config (e.g. disabling cache per type / configurable cache expiry per cache type)
- No rate limiting (this could be handled by a reverse proxy / api gateway in front of the service)
- No metrics being collected, this could be handled with [OpenTelemetry](https://opentelemetry.io/docs/)
- For testing purposes I used docker-compose, however for a real system I would prefer:
  - MongoDB / Redis hosted using a PaaS, not in a container
  - K8s or container PaaS (e.g. azure container apps) for api hosting
- For ease, most config has been left in `appsettings.json`
  - UserSecrets and Environment variables would be preferred
- Tests are a partial list that covers most basis, however it is not an exhaustive list