# Use the ASP.NET runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["Web/V.NET.API/V.NET.API.csproj", "Web/V.NET.API/"]
RUN dotnet restore "Web/V.NET.API/V.NET.API.csproj"

# Copy the rest of the application code
COPY . .

# Build the application
WORKDIR "/src/Web/V.NET.API"
RUN dotnet build "V.NET.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "V.NET.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Use the base image to create the final container
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "V.NET.API.dll"]
