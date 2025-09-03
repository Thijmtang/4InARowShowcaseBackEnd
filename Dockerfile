# Use the .NET SDK for the build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy solution file and project files
COPY DotNetAuth.sln .
COPY DotNetAuth/*.csproj ./DotNetAuth/

# Restore dependencies
WORKDIR /source/DotNetAuth
RUN dotnet restore

# Installeer Git vóór je build-dependencies
RUN apt-get update && apt-get install -y git

# Copy the rest of the source code and publish the app
COPY DotNetAuth/. ./DotNetAuth/
RUN dotnet publish  /app --no-restore

# Use the .NET Runtime for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

# Expose the API port (default is 80)
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "DotNetAuth.dll"]
