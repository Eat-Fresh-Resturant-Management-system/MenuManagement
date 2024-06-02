# Use the official .NET 8 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and project files
COPY WAOProjectMenu.sln .
COPY MenuManagement/MenuManagement.csproj MenuManagement/

# Restore dependencies
RUN dotnet restore MenuManagement/MenuManagement.csproj

# Copy the remaining files and build the application
COPY . .
WORKDIR /src/MenuManagement
RUN dotnet build MenuManagement.csproj -c Release -o /app/build

# Publish the application
RUN dotnet publish MenuManagement.csproj -c Release -o /app/publish

# Use the official .NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port the application runs on
EXPOSE 5136

# Define the entry point for the application
ENTRYPOINT ["dotnet", "MenuManagement.dll"]
