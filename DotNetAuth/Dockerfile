# Use .NET 8 SDK for ARM64 (needed for EF tools)
FROM mcr.microsoft.com/dotnet/sdk:8.0-focal-arm64v8 AS build
WORKDIR /source

# Copy project file and restore dependencies
COPY *.csproj .
RUN dotnet restore

# Copy source code and publish
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# Use SDK again for runtime to run EF migrations
FROM mcr.microsoft.com/dotnet/sdk:8.0-focal-arm64v8 AS runtime
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production

# Run EF Core migrations and start app
CMD dotnet ef database update && dotnet DotNetAuth.dll
