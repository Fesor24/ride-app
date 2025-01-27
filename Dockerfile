FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /source
COPY ["src/Soloride.API/SolorideAPI.csproj", "src/Soloride.API/"]
RUN dotnet restore "src/Soloride.API/SolorideAPI.csproj"
COPY . .
WORKDIR "/source/src/Soloride.API"
RUN dotnet build "SolorideAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SolorideAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

RUN mkdir -p /app/publish/Static
COPY /src/Soloride.API/Static /app/publish/Static

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SolorideAPI.dll"]
