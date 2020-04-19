FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["SignalRGame.ServerLocator/SignalRGame.ServerLocator.csproj", "SignalRGame.ServerLocator/"]
RUN dotnet restore "SignalRGame.ServerLocator/SignalRGame.ServerLocator.csproj"
COPY . .
WORKDIR "/src/SignalRGame.ServerLocator"
RUN dotnet build "SignalRGame.ServerLocator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SignalRGame.ServerLocator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SignalRGame.ServerLocator.dll"]