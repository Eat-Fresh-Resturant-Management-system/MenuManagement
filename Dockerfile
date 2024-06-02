# Base stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5136

ENV ASPNETCORE_URLS=http://+:5136

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN echo "Current directory before WORKDIR /src" && pwd && ls -la
WORKDIR /src
RUN pwd
COPY ["/eatfresh.menumanagement/MenuManagement/MenuManagement.csproj", "./"]
RUN echo "Current directory after copying csproj" && pwd && ls -la
RUN dotnet restore "MenuManagement.csproj"
COPY MenuManagement/ .
RUN echo "Current directory after copying all files" && pwd && ls -la
WORKDIR "/src/"
RUN echo "Current directory before build" && pwd && ls -la
RUN dotnet build "MenuManagement.csproj" -c Release

# Publish stage
FROM build AS publish
RUN dotnet publish "MenuManagement.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
RUN echo "Current directory before copying from publish" && pwd && ls -la
COPY --from=publish /app/publish .
RUN echo "Current directory after copying from publish" && pwd && ls -la
ENTRYPOINT ["dotnet", "MenuManagement.dll"]
