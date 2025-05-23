FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["lequeuer.api/lequeuer.api.csproj", "lequeuer.api/"]
RUN dotnet restore "lequeuer.api/lequeuer.api.csproj"
COPY . .
WORKDIR "/src/lequeuer.api"
RUN dotnet build "lequeuer.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "lequeuer.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "lequeuer.api.dll"]
