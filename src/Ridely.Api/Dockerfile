FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /src
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Ridely.Api/Ridely.Api.csproj", "Ridely.Api/"]
RUN dotnet restore "Ridely.Api/Ridely.Api.csproj"
COPY ["src/Ridely.Api", "Ridely.Api"]
COPY ["src/Ridely.Domain", "Ridely.Domain"]
COPY ["src/Ridely.Shared", "Ridely.Shared"]
COPY ["src/Ridely.Application", "Ridely.Application"]
COPY ["src/Ridely.Infrastructure", "Ridely.Infrastructure"]
COPY ["src/Ridely.Contracts", "Ridely.Contracts"]
COPY ["src/Ridely.ServiceDefaults", "Ridely.ServiceDefaults"]
WORKDIR "/src/Ridely.Api"
RUN dotnet build "Ridely.Api.csproj" -c $BUILD_CONFIGURATION -o /src/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Ridely.Api.csproj" -c $BUILD_CONFIGURATION -o /src/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /src
COPY --from=publish /src/publish .
ENTRYPOINT ["dotnet", "Ridely.Api.dll"]
