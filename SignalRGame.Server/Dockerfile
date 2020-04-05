FROM mcr.microsoft.com/dotnet/core/aspnet:5.0.0-preview-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:5.0.100-preview-buster AS build
RUN curl -sL https://deb.nodesource.com/setup_10.x |  bash -
RUN apt-get install -y nodejs

WORKDIR /src
COPY ["SignalRGame.Server/SignalRGame.Server.csproj", "SignalRGame.Server/"]
RUN dotnet restore "SignalRGame.Server/SignalRGame.Server.csproj"
COPY ["SignalRGame.Server/ClientApp/package*.json", "SignalRGame.Server/ClientApp/"]
WORKDIR /src/SignalRGame.Server/ClientApp
RUN npm ci
WORKDIR /src
COPY . .
WORKDIR "/src/SignalRGame.Server"
RUN dotnet build "SignalRGame.Server.csproj" -c Release -o /app/build
WORKDIR /src
RUN dotnet test -c Release


FROM build AS publish
WORKDIR "/src/SignalRGame.Server"
RUN dotnet publish "SignalRGame.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SignalRGame.Server.dll"]