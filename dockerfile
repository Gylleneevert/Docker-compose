FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081



# Use the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY  . "minimalAPI-master/"
RUN dotnet restore "minimalAPI-master/minimalAPI-webbutveckling-labb2.csproj"
COPY . .
WORKDIR "/src/minimalAPI-master"
RUN dotnet build "minimalAPI-webbutveckling-labb2.csproj" -c Release -o /app/build

# Use the build image to publish the application
FROM build AS publish
RUN dotnet publish "minimalAPI-webbutveckling-labb2.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "minimalAPI-webbutveckling-labb2.dll"]