FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["XpoTestWebApi/XpoTestWebApi.csproj", "XpoTestWebApi/"]
RUN dotnet restore "XpoTestWebApi/XpoTestWebApi.csproj"
COPY . .
WORKDIR "/src/XpoTestWebApi"
RUN dotnet build "XpoTestWebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "XpoTestWebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "XpoTestWebApi.dll"]
