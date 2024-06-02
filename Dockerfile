FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5136

ENV ASPNETCORE_URLS=http://+:5136

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MenuManagement/MenuManagement.csproj", "MenuManagement/"]
COPY ["WAOProjectMenu.sln", "./"]
RUN dotnet restore "MenuManagement/MenuManagement.csproj"
COPY . .
WORKDIR "/src/MenuManagement"
RUN dotnet build "MenuManagement.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MenuManagement.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MenuManagement.dll"]
