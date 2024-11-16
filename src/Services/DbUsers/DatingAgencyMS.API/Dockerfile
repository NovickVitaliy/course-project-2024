FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/DbUsers/DatingAgencyMS.API/DatingAgencyMS.API.csproj", "src/Services/DbUsers/DatingAgencyMS.API/"]
COPY ["src/Services/DbUsers/DatingAgencyMS.Application/DatingAgencyMS.Application.csproj", "src/Services/DbUsers/DatingAgencyMS.Application/"]
COPY ["Common.Business/Common.Business.csproj", "Common.Business/"]
COPY ["Common.Filtering/Common.Filtering.csproj", "Common.Filtering/"]
COPY ["src/Services/DbUsers/DatingAgencyMS.Domain/DatingAgencyMS.Domain.csproj", "src/Services/DbUsers/DatingAgencyMS.Domain/"]
COPY ["src/Services/DbUsers/DatingAgencyMS.Infrastructure/DatingAgencyMS.Infrastructure.csproj", "src/Services/DbUsers/DatingAgencyMS.Infrastructure/"]
COPY ["src/Common/Common.Auth/Common.Auth.csproj", "src/Common/Common.Auth/"]
RUN dotnet restore "src/Services/DbUsers/DatingAgencyMS.API/DatingAgencyMS.API.csproj"
COPY . .
WORKDIR "/src/src/Services/DbUsers/DatingAgencyMS.API"
RUN dotnet build "DatingAgencyMS.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DatingAgencyMS.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatingAgencyMS.API.dll"]
