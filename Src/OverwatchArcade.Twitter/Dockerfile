FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OverwatchArcade.Twitter/OverwatchArcade.Twitter.csproj", "OverwatchArcade.Twitter/"]
RUN dotnet restore "OverwatchArcade.Twitter/OverwatchArcade.Twitter.csproj"
COPY . .
WORKDIR "/src/OverwatchArcade.Twitter"
RUN dotnet build "OverwatchArcade.Twitter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OverwatchArcade.Twitter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OverwatchArcade.Twitter.dll"]
