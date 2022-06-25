FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
# Install NodeJS to run playwright installer
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_16.x | bash \
    && apt-get install nodejs -yq \
    && npm install -g npm
# Install browsers and depdencies for playwright
RUN npx playwright@1.17.0 install chromium \
    && npx playwright@1.17.0 install-deps chromium
#Uninstall NodeJS
RUN apt remove nodejs curl gnupg -yq \
    rm -fv /etc/apt/sources.list.d/nodesource*

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["OverwatchArcade.API/OverwatchArcade.API.csproj", "OverwatchArcade.API/"]
RUN dotnet restore "OverwatchArcade.API/OverwatchArcade.API.csproj"
COPY . .
WORKDIR "/src/OverwatchArcade.API"
RUN dotnet build "OverwatchArcade.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OverwatchArcade.API.csproj" -c Release -o /app/publish
COPY --from=build /app/build/playwright.ps1 /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OverwatchArcade.API.dll"]
