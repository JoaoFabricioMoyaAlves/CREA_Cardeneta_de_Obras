FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/CadernetaObras.Api/CadernetaObras.Api.csproj src/CadernetaObras.Api/
COPY src/CadernetaObras.Application/CadernetaObras.Application.csproj src/CadernetaObras.Application/
COPY src/CadernetaObras.Domain/CadernetaObras.Domain.csproj src/CadernetaObras.Domain/
COPY src/CadernetaObras.Infrastructure/CadernetaObras.Infrastructure.csproj src/CadernetaObras.Infrastructure/
RUN dotnet restore src/CadernetaObras.Api/CadernetaObras.Api.csproj

COPY src/ src/
RUN dotnet publish src/CadernetaObras.Api/CadernetaObras.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "CadernetaObras.Api.dll"]
