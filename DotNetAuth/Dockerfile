# Use .NET 8 SDK for ARM64
FROM mcr.microsoft.com/dotnet/sdk:8.0-focal-arm64v8 AS build
WORKDIR /source

# Copy project file and restore dependencies
COPY *.csproj .
RUN dotnet restore

# Copy source code and publish
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Use .NET 8 Runtime for ARM64
FROM mcr.microsoft.com/dotnet/aspnet:8.0-focal-arm64v8 AS runtime
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production

# Run EF Core migrations and start app
ENTRYPOINT ["sh", "-c", "dotnet ef database update && dotnet DotNetAuth.dll"]
