FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5050
EXPOSE 5050

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PictureDatabaseAPI/PictureDatabaseAPI.csproj", "PictureDatabaseAPI/"]
RUN dotnet restore "PictureDatabaseAPI/PictureDatabaseAPI.csproj"
COPY . .
WORKDIR "/src/PictureDatabaseAPI"
RUN dotnet build "PictureDatabaseAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "PictureDatabaseAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER root
RUN mkdir -p /app/data && chmod 777 /app/data
USER $APP_UID
ENTRYPOINT ["dotnet", "PictureDatabaseAPI.dll"]