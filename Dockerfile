# Use the .NET SDK for the build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy solution file and project files
COPY DotNetAuth.sln .
COPY DotNetAuth/*.csproj ./DotNetAuth/

# Restore dependencies
WORKDIR /source/DotNetAuth
RUN dotnet restore

# Install Git for build dependencies
RUN apt-get update && apt-get install -y git

# Copy the rest of the source code
COPY DotNetAuth/. .

# Publish the app to /app directory
RUN dotnet publish -c Release -o /app --no-restore

# Use the .NET Runtime for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

# Expose the API port (default is 80)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Run the application
ENTRYPOINT ["sh", "-c", "dotnet ef database update && dotnet DotNetAuth.dll"]
