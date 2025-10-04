# Multi-stage build for ZPL2PDF
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set working directory
WORKDIR /app

# Copy project files
COPY src/ZPL2PDF.csproj ./src/
COPY tests/ ./tests/

# Restore dependencies
RUN dotnet restore src/ZPL2PDF.csproj

# Copy source code
COPY src/ ./src/

# Build the application
RUN dotnet build src/ZPL2PDF.csproj --configuration Release --no-restore

# Publish the application
RUN dotnet publish src/ZPL2PDF.csproj --configuration Release --no-build --output /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime

# Install required packages
RUN apt-get update && apt-get install -y \
    libgdiplus \
    libc6-dev \
    && rm -rf /var/lib/apt/lists/*

# Create non-root user
RUN groupadd -r zpl2pdf && useradd -r -g zpl2pdf zpl2pdf

# Set working directory
WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# Create directories for monitoring and output
RUN mkdir -p /app/watch /app/output && \
    chown -R zpl2pdf:zpl2pdf /app

# Switch to non-root user
USER zpl2pdf

# Set environment variables
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Expose ports (if needed for health checks)
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD dotnet ZPL2PDF.dll status || exit 1

# Default command (daemon mode)
CMD ["dotnet", "ZPL2PDF.dll", "start", "-l", "/app/watch", "-o", "/app/output"]

# Labels
LABEL maintainer="brunoleocam"
LABEL description="ZPL2PDF - ZPL to PDF Converter"
LABEL version="2.0.0"
