# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project file and restore
COPY ["src-mcp/docs-ifa-mcp/docs-ifa-mcp.csproj", "docs-ifa-mcp/"]
RUN dotnet restore "docs-ifa-mcp/docs-ifa-mcp.csproj"

# Copy project source
COPY ["src-mcp/docs-ifa-mcp/", "docs-ifa-mcp/"]

# Build
WORKDIR "/src/docs-ifa-mcp"
RUN dotnet build "docs-ifa-mcp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "docs-ifa-mcp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Copy documentation from the forked repo
# This assumes the Dockerfile is run from the repo root
COPY ["versioned_docs/version-v6.0.0/", "/app/docs/auth/"]

# Set environment variables
ENV Documentation__BasePath=/app/docs
ENV Documentation__FallbackPath=/app/docs
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_ENVIRONMENT=Production

# Run the MCP server
ENTRYPOINT ["dotnet", "docs-ifa-mcp.dll"]