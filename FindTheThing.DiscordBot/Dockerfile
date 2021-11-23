#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FindTheThing.DiscordBot/FindTheThing.DiscordBot.csproj", "FindTheThing.DiscordBot/"]
RUN dotnet restore "FindTheThing.DiscordBot/FindTheThing.DiscordBot.csproj"
COPY . .
WORKDIR "/src/FindTheThing.DiscordBot"
RUN dotnet build "FindTheThing.DiscordBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FindTheThing.DiscordBot.csproj" -c Release -o /app/publish
COPY FindTheThing.DiscordBot/appsettings.json /app/publish/

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FindTheThing.DiscordBot.dll"]