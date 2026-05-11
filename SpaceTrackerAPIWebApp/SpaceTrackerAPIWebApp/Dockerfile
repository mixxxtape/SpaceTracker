FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY ["SpaceTrackerAPIWebApp.csproj", "."]
RUN dotnet restore "SpaceTrackerAPIWebApp.csproj" --no-cache

COPY . .
RUN dotnet publish "SpaceTrackerAPIWebApp.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app

RUN groupadd --system appgroup && \
    useradd --system --gid appgroup appuser

COPY --from=build /app/publish .
RUN find /app -name "*.br" -delete && find /app -name "*.gz" -delete
RUN chown -R appuser:appgroup /app

USER appuser

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SpaceTrackerAPIWebApp.dll"]