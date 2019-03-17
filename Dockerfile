## Base Configuration
FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Local
ENV ASPNETCORE_URLS http://*:9203
EXPOSE 9203

## Restore and build the project
FROM microsoft/dotnet:2.1-sdk AS builder
WORKDIR /src
COPY *.sln ./
COPY Backend.API/Backend.API.csproj Backend.API/
COPY Backend.Manager/Backend.Manager.csproj Backend.Manager/
COPY Backend.Tests/Backend.Tests.csproj Backend.Tests/
COPY Minio/Minio.csproj Minio/
RUN dotnet restore

COPY . .
WORKDIR /src/Backend.API
RUN dotnet build -c Configuration=Release -o /app


## Publish the project
FROM builder AS publish
RUN dotnet publish -c Release -o /app

## Build image
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
RUN	mkdir ../Settings && cd ../Settings/
COPY Settings/* /Settings/

ENTRYPOINT ["dotnet", "Backend.API.dll"]
