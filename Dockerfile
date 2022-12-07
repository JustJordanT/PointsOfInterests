FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CityInfo.API/CityInfo.API.csproj", "CityInfo.API/"]
RUN dotnet restore "CityInfo.API/CityInfo.API.csproj"
COPY . .
WORKDIR "/src/CityInfo.API"
RUN dotnet build "CityInfo.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CityInfo.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CityInfo.API.dll"]
