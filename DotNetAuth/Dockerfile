# Use the official .NET SDK image for ARM64
FROM mcr.microsoft.com/dotnet/sdk:8.0-focal-arm64v8 AS build
WORKDIR /source

# Copy the .csproj file and restore dependencies
COPY *.csproj .
RUN dotnet restore

# Copy the rest of the source code and build the app
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET Runtime image for ARM64
FROM mcr.microsoft.com/dotnet/aspnet:8.0-focal-arm64v8 AS runtime
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose port 8080
EXPOSE 8080

# Set the environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production

# Run EF Core migrations and then start the application
ENTRYPOINT sh -c "dotnet ef database update && dotnet DotNetAuth.dll"
