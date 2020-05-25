# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln ./
COPY Backend.Minio.Api/*.csproj ./

RUN dotnet restore ./*.csproj

# copy and publish app and libraries
COPY . .
RUN dotnet publish Backend.Minio.Api -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as app
WORKDIR /app
COPY --from=build /app .

# copy project settings
RUN	mkdir ../Settings && cd ../Settings/
COPY Settings/* /Settings/

ENTRYPOINT ["dotnet", "Backend.Minio.Api.dll"]

VOLUME [ "../Settings" ]

EXPOSE 5100