#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0.9-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt update && \
    apt install libglib2.0-0 libnss3 libxcb-composite0 -y 

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["OWArcadeBackend.csproj", "."]
RUN dotnet restore "./OWArcadeBackend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "OWArcadeBackend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OWArcadeBackend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OWArcadeBackend.dll"]