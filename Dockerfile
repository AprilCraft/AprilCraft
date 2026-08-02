FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AprilCraft.Web/AprilCraft.Web.csproj", "AprilCraft.Web/"]
RUN dotnet restore "AprilCraft.Web/AprilCraft.Web.csproj"
COPY . .
WORKDIR "/src/AprilCraft.Web"
RUN dotnet build "AprilCraft.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AprilCraft.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AprilCraft.Web.dll"]
