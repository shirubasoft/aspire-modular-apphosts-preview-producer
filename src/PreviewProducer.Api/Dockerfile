FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props global.json ./
COPY src/PreviewProducer.Api/PreviewProducer.Api.csproj src/PreviewProducer.Api/
RUN dotnet restore src/PreviewProducer.Api/PreviewProducer.Api.csproj

COPY src/PreviewProducer.Api/ src/PreviewProducer.Api/
RUN dotnet publish src/PreviewProducer.Api/PreviewProducer.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "PreviewProducer.Api.dll"]
