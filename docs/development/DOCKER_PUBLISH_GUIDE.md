# 🚀 Docker Publishing Guide

## 🎯 **Overview**

This guide explains how to publish Docker images for ZPL2PDF to container registries like GitHub Container Registry and Docker Hub.

---

## 📋 **Prerequisites**

### **Required Accounts**
- GitHub account (for GitHub Container Registry)
- Docker Hub account (for Docker Hub publishing)

### **Required Tools**
```bash
# Verify Docker
docker --version

# Verify Git
git --version

# Verify GitHub CLI (optional but recommended)
gh --version
```

---

## 📦 **GitHub Container Registry (ghcr.io)**

### **1. Create Personal Access Token**

1. Go to: https://github.com/settings/tokens
2. Click "**Generate new token**" → "**Generate new token (classic)**"
3. Configure:
   - **Note:** `ZPL2PDF Docker Publishing`
   - **Expiration:** `No expiration` or `1 year`
   - **Scopes:** Check:
     - ✅ `write:packages`
     - ✅ `read:packages`
     - ✅ `delete:packages`
4. Click "**Generate token**"
5. **⚠️ COPY THE TOKEN NOW** (you won't see it again!)

### **2. Login to GitHub Container Registry**

```bash
# Login to ghcr.io
echo $GITHUB_TOKEN | docker login ghcr.io -u YOUR_GITHUB_USERNAME --password-stdin
```

### **3. Tag and Push Image**

```bash
# Tag the Alpine image (optimized)
docker tag zpl2pdf:alpine ghcr.io/YOUR_USERNAME/zpl2pdf:latest
docker tag zpl2pdf:alpine ghcr.io/YOUR_USERNAME/zpl2pdf:2.0.0
docker tag zpl2pdf:alpine ghcr.io/YOUR_USERNAME/zpl2pdf:2.0
docker tag zpl2pdf:alpine ghcr.io/YOUR_USERNAME/zpl2pdf:2

# Push to GitHub Container Registry
docker push ghcr.io/YOUR_USERNAME/zpl2pdf:latest
docker push ghcr.io/YOUR_USERNAME/zpl2pdf:2.0.0
docker push ghcr.io/YOUR_USERNAME/zpl2pdf:2.0
docker push ghcr.io/YOUR_USERNAME/zpl2pdf:2
```

### **4. Make Image Public**

1. Go to: https://github.com/YOUR_USERNAME?tab=packages
2. Click on `zpl2pdf`
3. Click "**Package settings**"
4. Scroll to "**Danger Zone**"
5. Click "**Change visibility**"
6. Select "**Public**"
7. Type `zpl2pdf` to confirm
8. Click "**I understand, change package visibility**"

---

## 🐳 **Docker Hub**

### **1. Create Account**

1. Go to: https://hub.docker.com/signup
2. Fill in:
   - **Username:** Choose your username
   - **Email:** Your email
   - **Password:** Strong password
3. Confirm email

### **2. Create Repository**

1. Go to: https://hub.docker.com/repositories
2. Click "**Create Repository**"
3. Configure:
   - **Name:** `zpl2pdf`
   - **Category:** Choose one (see below)
   - **Short Description:** Keep under 100 characters
   - **Visibility:** `Public`
4. Click "**Create**"

**Category Suggestions:**
- **Developer Tools** (recommended)
- **Utilities** (alternative)
- **Productivity** (alternative)

### **3. Login to Docker Hub**

```bash
# Interactive login
docker login

# Enter:
# Username: YOUR_DOCKERHUB_USERNAME
# Password: YOUR_PASSWORD
```

### **4. Tag and Push Image**

```bash
# Tag the Alpine image
docker tag zpl2pdf:alpine YOUR_USERNAME/zpl2pdf:latest
docker tag zpl2pdf:alpine YOUR_USERNAME/zpl2pdf:2.0.0
docker tag zpl2pdf:alpine YOUR_USERNAME/zpl2pdf:2.0
docker tag zpl2pdf:alpine YOUR_USERNAME/zpl2pdf:2
docker tag zpl2pdf:alpine YOUR_USERNAME/zpl2pdf:alpine

# Push to Docker Hub
docker push YOUR_USERNAME/zpl2pdf:latest
docker push YOUR_USERNAME/zpl2pdf:2.0.0
docker push YOUR_USERNAME/zpl2pdf:2.0
docker push YOUR_USERNAME/zpl2pdf:2
docker push YOUR_USERNAME/zpl2pdf:alpine
```

### **5. Configure README on Docker Hub**

1. Go to: https://hub.docker.com/r/YOUR_USERNAME/zpl2pdf
2. In the "**Description**" tab, click "**Edit**"
3. Add comprehensive README content including:
   - Project description
   - Quick start examples
   - Usage instructions
   - Environment variables
   - Links to documentation

---

## ✅ **Verification**

### **Test GitHub Container Registry**

```bash
# Remove local image (to test download)
docker rmi ghcr.io/YOUR_USERNAME/zpl2pdf:latest

# Download from GitHub
docker pull ghcr.io/YOUR_USERNAME/zpl2pdf:latest

# Test
docker run --rm ghcr.io/YOUR_USERNAME/zpl2pdf:latest /app/ZPL2PDF -help
```

### **Test Docker Hub**

```bash
# Remove local image
docker rmi YOUR_USERNAME/zpl2pdf:latest

# Download from Docker Hub
docker pull YOUR_USERNAME/zpl2pdf:latest

# Test
docker run --rm YOUR_USERNAME/zpl2pdf:latest /app/ZPL2PDF -help
```

---

## 🤖 **Automation with GitHub Actions**

Create `.github/workflows/docker-publish.yml`:

```yaml
name: Docker Build and Publish

on:
  release:
    types: [published]
  workflow_dispatch:

env:
  REGISTRY_GHCR: ghcr.io
  REGISTRY_DOCKERHUB: docker.io
  IMAGE_NAME: zpl2pdf

jobs:
  build-and-push:
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v3

      - name: Log in to GitHub Container Registry
        uses: docker/login-action@v3
        with:
          registry: ${{ env.REGISTRY_GHCR }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Log in to Docker Hub
        uses: docker/login-action@v3
        with:
          registry: ${{ env.REGISTRY_DOCKERHUB }}
          username: ${{ secrets.DOCKERHUB_USERNAME }}
          password: ${{ secrets.DOCKERHUB_TOKEN }}

      - name: Extract metadata
        id: meta
        uses: docker/metadata-action@v5
        with:
          images: |
            ${{ env.REGISTRY_GHCR }}/${{ github.repository_owner }}/${{ env.IMAGE_NAME }}
            ${{ env.REGISTRY_DOCKERHUB }}/${{ secrets.DOCKERHUB_USERNAME }}/${{ env.IMAGE_NAME }}
          tags: |
            type=semver,pattern={{version}}
            type=semver,pattern={{major}}.{{minor}}
            type=semver,pattern={{major}}
            type=raw,value=latest

      - name: Build and push
        uses: docker/build-push-action@v5
        with:
          context: .
          platforms: linux/amd64,linux/arm64
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}
          cache-from: type=gha
          cache-to: type=gha,mode=max
```

### **Configure Secrets**

1. Go to: https://github.com/YOUR_USERNAME/ZPL2PDF/settings/secrets/actions
2. Click "**New repository secret**"
3. Add:
   - `DOCKERHUB_USERNAME` = `YOUR_DOCKERHUB_USERNAME`
   - `DOCKERHUB_TOKEN` = (create at https://hub.docker.com/settings/security)

**Now when you create a release, it publishes automatically!**

---

## 📊 **Quick Commands Summary**

### **Build Local**
```bash
docker build -t zpl2pdf:alpine .
```

### **GitHub Container Registry**
```bash
# Login
echo $GITHUB_TOKEN | docker login ghcr.io -u YOUR_USERNAME --password-stdin

# Tag and Push
docker tag zpl2pdf:alpine ghcr.io/YOUR_USERNAME/zpl2pdf:latest
docker push ghcr.io/YOUR_USERNAME/zpl2pdf:latest
```

### **Docker Hub**
```bash
# Login
docker login

# Tag and Push
docker tag zpl2pdf:alpine YOUR_USERNAME/zpl2pdf:latest
docker push YOUR_USERNAME/zpl2pdf:latest
```

---

## ❓ **FAQ**

**Q: Why do I need the username prefix?**
A: It's required on Docker Hub. Names without prefix are reserved for official images.

**Q: Can I use just "zpl2pdf"?**
A: Users can create a local alias: `docker tag YOUR_USERNAME/zpl2pdf:latest zpl2pdf:latest`

**Q: What's the cost?**
A: FREE for public repositories on both GitHub and Docker Hub.

**Q: How long does upload take?**
A: ~10 minutes for 470MB (depends on internet speed).

**Q: Can I delete later?**
A: Yes, you own the images and can delete them anytime.

---

## ✅ **Checklist**

- [ ] GitHub account created
- [ ] Docker Hub account created
- [ ] GitHub PAT token created
- [ ] Login to ghcr.io working
- [ ] Login to Docker Hub working
- [ ] Image tagged for ghcr.io
- [ ] Image tagged for Docker Hub
- [ ] Push to ghcr.io completed
- [ ] Push to Docker Hub completed
- [ ] Image public on GitHub
- [ ] Repository public on Docker Hub
- [ ] README configured on Docker Hub
- [ ] Download test working

---

**After completion, users can install with:**
```bash
# GitHub Container Registry
docker pull ghcr.io/YOUR_USERNAME/zpl2pdf:latest

# Docker Hub
docker pull YOUR_USERNAME/zpl2pdf:latest
```

**Both work and stay synchronized!** ✅