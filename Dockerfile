# =============================================================================
# ZPL2PDF - Alpine Linux Build (Lightweight)
# =============================================================================
# This Dockerfile creates a lightweight container using Alpine Linux
# Includes fonts required for SkiaSharp text rendering
# Build: docker build -t zpl2pdf:latest .
# =============================================================================

# -----------------------------------------------------------------------------
# Stage 1: Build
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0.306 AS build

# Set working directory
WORKDIR /src

# Copy project file and restore dependencies
COPY ZPL2PDF.csproj .
COPY Resources/ ./Resources/
RUN dotnet restore ZPL2PDF.csproj

# Copy source code
COPY src/ ./src/

# Build and publish for linux-musl (Alpine)
RUN dotnet publish ZPL2PDF.csproj \
    --configuration Release \
    --runtime linux-musl-x64 \
    --self-contained true \
    --output /app/publish \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -p:PublishReadyToRun=true \
    -p:PublishTrimmed=false

# -----------------------------------------------------------------------------
# Stage 2: Runtime (Alpine Linux - Ultra Light)
# -----------------------------------------------------------------------------
FROM alpine:3.23 AS runtime

# Install runtime dependencies
# - libgdiplus: Required for System.Drawing compatibility
# - icu-libs/icu-data-full: Unicode and globalization support
# - fontconfig + fonts: Required for SkiaSharp text rendering (fixes "asset null" error)
#   * ttf-dejavu: Good coverage for Western characters (~3MB)
#   * ttf-liberation: Microsoft font alternatives (~2MB)
#   * font-noto: Extensive Unicode coverage including accented characters (~100MB)
#
# NOTE: font-noto significantly increases image size but provides best Unicode support.
# For smaller images with basic Latin support only, remove font-noto line.
# Current image size: ~250MB (with all fonts) vs ~150MB (without font-noto)
RUN apk add --no-cache \
    libgdiplus \
    libintl \
    icu-libs \
    icu-data-full \
    libstdc++ \
    libgcc \
    ca-certificates \
    tzdata \
    fontconfig \
    ttf-dejavu \
    ttf-liberation \
    font-noto \
    && fc-cache -f

# Create non-root user
RUN addgroup -S zpl2pdf && adduser -S zpl2pdf -G zpl2pdf

# Set working directory
WORKDIR /app

# Copy executable
COPY --from=build /app/publish/ZPL2PDF .

# Create directories and set permissions
RUN mkdir -p /app/config && \
    chmod +x /app/ZPL2PDF && \
    chown -R zpl2pdf:zpl2pdf /app

# Switch to non-root user
USER zpl2pdf

# Set environment variables
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    DOTNET_RUNNING_IN_CONTAINER=true \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8 \
    ZPL2PDF_LANGUAGE=en-US

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD /app/ZPL2PDF status || exit 1

# Volume mount points
VOLUME ["/app/config"]

# Default command
CMD ["/app/ZPL2PDF", "run", "-l", "/app/watch"]

# Metadata
LABEL maintainer="brunoleocam" \
      description="ZPL2PDF - Alpine Linux (Ultra Lightweight)" \
      version="2.0.0-alpine"
