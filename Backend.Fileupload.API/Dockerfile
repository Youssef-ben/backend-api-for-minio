FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Backend.Fileupload.API/Backend.Fileupload.API.csproj Backend.Fileupload.API/
RUN dotnet restore Backend.Fileupload.API/Backend.Fileupload.API.csproj
COPY . .
WORKDIR /src/Backend.Fileupload.API
RUN dotnet build Backend.Fileupload.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Backend.Fileupload.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Backend.Fileupload.API.dll"]
