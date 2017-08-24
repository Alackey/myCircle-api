# myCircle-api

The API for myCircle.

## Quick Start
### Docker
Ensure docker and docker-compose are installed.

1. Set environment variables:
   * SQLPROXY_INSTANCE_NAME={YOUR_CLOUD_SQL_PROXY_INSTANCE_CONNECTION_NAME}
2. Ensure you have appsettings.Development.json and gcloud-sqlproxy-key.json in the root folder.
3. `docker-compose up -d`
4. `docker exec -it mycircleapi_api_1 bash`
5. In the docker container terminal: `dotnet run`

 