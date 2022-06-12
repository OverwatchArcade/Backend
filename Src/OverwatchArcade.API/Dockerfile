FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OverwatchArcade.API/OverwatchArcade.API.csproj", "OverwatchArcade.API/"]
RUN dotnet restore "OverwatchArcade.API/OverwatchArcade.API.csproj"
COPY . .
WORKDIR "/src/OverwatchArcade.API"
RUN dotnet build "OverwatchArcade.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OverwatchArcade.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OverwatchArcade.API.dll"]
