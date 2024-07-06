FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/DbUsers/DbUsers.API/DbUsers.API.csproj", "src/Services/DbUsers/DbUsers.API/"]
RUN dotnet restore "src/Services/DbUsers/DbUsers.API/DbUsers.API.csproj"
COPY . .
WORKDIR "/src/src/Services/DbUsers/DbUsers.API"
RUN dotnet build "DbUsers.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DbUsers.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DbUsers.API.dll"]
