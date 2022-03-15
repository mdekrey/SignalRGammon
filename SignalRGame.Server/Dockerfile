FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN curl -sL https://deb.nodesource.com/setup_16.x |  bash -
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


FROM build AS publish
RUN dotnet publish "SignalRGame.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SignalRGame.Server.dll"]