FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["wiki-update-companion.csproj", ""]
RUN dotnet restore "./wiki-update-companion.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "wiki-update-companion.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "wiki-update-companion.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER root
ENTRYPOINT ["dotnet", "wiki-update-companion.dll"]