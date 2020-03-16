FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
RUN curl -sL https://deb.nodesource.com/setup_10.x |  bash -
RUN apt-get install -y nodejs

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["SignalRGammon/SignalRGammon.csproj", "SignalRGammon/"]
RUN dotnet restore "SignalRGammon/SignalRGammon.csproj"
COPY ["SignalRGammon/ClientApp/package*.json", "SignalRGammon/ClientApp/"]
WORKDIR /src/SignalRGammon/ClientApp
RUN npm ci
WORKDIR /src
COPY . .
WORKDIR "/src/SignalRGammon"
RUN dotnet build "SignalRGammon.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SignalRGammon.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SignalRGammon.dll"]