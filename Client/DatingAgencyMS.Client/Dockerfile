FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Client/DatingAgencyMS.Client/DatingAgencyMS.Client.csproj", "Client/DatingAgencyMS.Client/"]
COPY ["Common.Business/Common.Business.csproj", "Common.Business/"]
COPY ["Common.Filtering/Common.Filtering.csproj", "Common.Filtering/"]
RUN dotnet restore "Client/DatingAgencyMS.Client/DatingAgencyMS.Client.csproj"
COPY . .
WORKDIR "/src/Client/DatingAgencyMS.Client"
RUN dotnet build "DatingAgencyMS.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DatingAgencyMS.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatingAgencyMS.Client.dll"]
