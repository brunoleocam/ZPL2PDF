# 🚀 Guia de Publicação - Docker Images

Guia completo passo a passo para publicar as imagens Docker do ZPL2PDF no **GitHub Container Registry** e **Docker Hub**.

---

## 📋 **PRÉ-REQUISITOS**

### 1. Conta no GitHub
- ✅ Você já tem: `github.com/brunoleocam`
- ✅ Repositório: `github.com/brunoleocam/ZPL2PDF`

### 2. Conta no Docker Hub
- 🔗 Criar em: https://hub.docker.com/signup
- 📝 Escolher username: `brunoleocam` (recomendado)

### 3. Ferramentas Instaladas
```bash
# Verificar Docker
docker --version

# Verificar Git
git --version

# Verificar GitHub CLI (opcional, mas recomendado)
gh --version
```

---

## 🎯 **OTIMIZAÇÃO CONCLUÍDA!**

### **Resultados:**

| Versão | Tamanho | Redução | Status |
|--------|---------|---------|--------|
| **Original** | 674MB | - | ⚠️ Pesada |
| **Optimized** | 579MB | -14% | ✅ Melhor |
| **Alpine** | 470MB | **-30%** | ✅ **MELHOR!** |

**Escolha:** Vamos usar a versão **Alpine (470MB)** como padrão!

---

## 📦 **PASSO 1: GitHub Container Registry (ghcr.io)**

### **1.1 - Criar Personal Access Token (PAT)**

1. Acesse: https://github.com/settings/tokens
2. Clique em "**Generate new token**" → "**Generate new token (classic)**"
3. Configure:
   - **Note:** `ZPL2PDF Docker Publishing`
   - **Expiration:** `No expiration` ou `1 year`
   - **Scopes:** Marque:
     - ✅ `write:packages`
     - ✅ `read:packages`
     - ✅ `delete:packages`
4. Clique em "**Generate token**"
5. **⚠️ COPIE O TOKEN AGORA** (não poderá ver novamente!)

### **1.2 - Login no GitHub Container Registry**

```powershell
# No Windows PowerShell:

# Salvar token em variável (substitua YOUR_TOKEN)
$GITHUB_TOKEN = "ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"

# Login no ghcr.io
echo $GITHUB_TOKEN | docker login ghcr.io -u brunoleocam --password-stdin
```

**Saída esperada:**
```
Login Succeeded
```

### **1.3 - Tag e Push da Imagem**

```powershell
# Tag da imagem Alpine (otimizada)
docker tag zpl2pdf:alpine ghcr.io/brunoleocam/zpl2pdf:latest
docker tag zpl2pdf:alpine ghcr.io/brunoleocam/zpl2pdf:2.0.0
docker tag zpl2pdf:alpine ghcr.io/brunoleocam/zpl2pdf:2.0
docker tag zpl2pdf:alpine ghcr.io/brunoleocam/zpl2pdf:2

# Push para GitHub Container Registry
docker push ghcr.io/brunoleocam/zpl2pdf:latest
docker push ghcr.io/brunoleocam/zpl2pdf:2.0.0
docker push ghcr.io/brunoleocam/zpl2pdf:2.0
docker push ghcr.io/brunoleocam/zpl2pdf:2
```

**Tempo estimado:** ~5-10 minutos (upload de 470MB)

### **1.4 - Tornar Imagem Pública**

1. Acesse: https://github.com/brunoleocam?tab=packages
2. Clique em `zpl2pdf`
3. Clique em "**Package settings**"
4. Role até "**Danger Zone**"
5. Clique em "**Change visibility**"
6. Selecione "**Public**"
7. Digite `zpl2pdf` para confirmar
8. Clique em "**I understand, change package visibility**"

**Agora qualquer pessoa pode baixar:**
```bash
docker pull ghcr.io/brunoleocam/zpl2pdf:latest
```

---

## 🐳 **PASSO 2: Docker Hub**

### **2.1 - Criar Conta**

1. Acesse: https://hub.docker.com/signup
2. Preencha:
   - **Username:** `brunoleocam` (recomendado)
   - **Email:** seu email
   - **Password:** senha forte
3. Confirme email

### **2.2 - Criar Repositório**

1. Acesse: https://hub.docker.com/repositories
2. Clique em "**Create Repository**"
3. Configure:
   - **Name:** `zpl2pdf`
   - **Category:** Escolha uma (veja abaixo)
   - **Short Description:** Use uma destas opções (limite: 100 caracteres):
   
   **Opção 1 (Recomendada - 57 caracteres):**
   ```
   Convert ZPL (Zebra Programming Language) labels to PDF easily
   ```
   
   **Opção 2 (56 caracteres):**
   ```
   Convert Zebra ZPL labels to PDF - Multi-language support
   ```
   
   **Opção 3 (56 caracteres):**
   ```
   ZPL to PDF converter with daemon mode and multi-language
   ```
   
   - **Visibility:** `Public`
4. Clique em "**Create**"

> **📝 Notas:**
> - **Category (Categoria):** Use `Developer Tools` (recomendado) ou `Utilities`
> - **Short Description:** Aparece nas buscas e listagens
> - **Full Description:** Configure no passo 2.5 com Markdown completo

#### **📂 Categorias Sugeridas:**

| Categoria | Por quê? | Prioridade |
|-----------|----------|------------|
| **Developer Tools** | Ferramenta de conversão para desenvolvedores | ⭐ **Recomendada** |
| **Utilities** | Utilitário de conversão de arquivos | ⭐⭐ Alternativa |
| **Productivity** | Automação de conversão de etiquetas | ⭐⭐ Alternativa |

### **2.3 - Login no Docker Hub**

```powershell
# Login interativo
docker login

# Digite:
# Username: brunoleocam
# Password: sua_senha
```

**Saída esperada:**
```
Login Succeeded
```

### **2.4 - Tag e Push da Imagem**

```powershell
# Tag da imagem Alpine
docker tag zpl2pdf:alpine brunoleocam/zpl2pdf:latest
docker tag zpl2pdf:alpine brunoleocam/zpl2pdf:2.0.0
docker tag zpl2pdf:alpine brunoleocam/zpl2pdf:2.0
docker tag zpl2pdf:alpine brunoleocam/zpl2pdf:2
docker tag zpl2pdf:alpine brunoleocam/zpl2pdf:alpine

# Push para Docker Hub
docker push brunoleocam/zpl2pdf:latest
docker push brunoleocam/zpl2pdf:2.0.0
docker push brunoleocam/zpl2pdf:2.0
docker push brunoleocam/zpl2pdf:2
docker push brunoleocam/zpl2pdf:alpine
```

**Tempo estimado:** ~5-10 minutos

### **2.5 - Configurar README no Docker Hub**

> **⚠️ Importante:** A **Short Description** (criada no passo 2.2) tem limite de 100 caracteres.
> O **Full Description** abaixo NÃO tem limite e aceita Markdown completo.

1. Acesse: https://hub.docker.com/r/brunoleocam/zpl2pdf
2. Na aba "**Description**", clique em "**Edit**" ou role até a seção de edição
3. Cole este conteúdo no campo **Full Description**:

````markdown
# ZPL2PDF - ZPL to PDF Converter

Convert ZPL (Zebra Programming Language) labels to PDF easily.

> 📖 **Full documentation:** [GitHub Repository](https://github.com/brunoleocam/ZPL2PDF)

## Features

- ✅ Multi-language support (EN, PT, ES, FR, DE, IT, JA, ZH)
- ✅ Daemon mode (automatic folder monitoring)
- ✅ Conversion mode (single file)
- ✅ Cross-platform (Linux x64, ARM64, ARM)
- ✅ Ultra-lightweight Alpine Linux base (470MB)
- ✅ Health checks and auto-restart

## Quick Start

### Daemon Mode (Auto-Convert)

```bash
docker run -d \
  --name zpl2pdf \
  -v ./watch:/app/watch \
  -v ./output:/app/output \
  -e ZPL2PDF_LANGUAGE=en-US \
  brunoleocam/zpl2pdf:latest
```

### Single File Conversion

```bash
docker run --rm \
  -v ./input:/app/input:ro \
  -v ./output:/app/output \
  brunoleocam/zpl2pdf:latest \
  /app/ZPL2PDF -i /app/input/label.txt -o /app/output -n result.pdf
```

### Using Docker Compose

```yaml
version: '3.8'

services:
  zpl2pdf:
    image: brunoleocam/zpl2pdf:latest
    container_name: zpl2pdf-daemon
    volumes:
      - ./watch:/app/watch
      - ./output:/app/output
    environment:
      - ZPL2PDF_LANGUAGE=pt-BR
    restart: unless-stopped
```

## Supported Languages

Set via `ZPL2PDF_LANGUAGE` environment variable:

- `en-US` - English
- `pt-BR` - Portuguese (Brazil)
- `es-ES` - Spanish
- `fr-FR` - French
- `de-DE` - German
- `it-IT` - Italian
- `ja-JP` - Japanese
- `zh-CN` - Chinese

## 📚 Documentation & Links

- **📖 Main Repository:** [github.com/brunoleocam/ZPL2PDF](https://github.com/brunoleocam/ZPL2PDF)
- **📘 README:** [Full Documentation](https://github.com/brunoleocam/ZPL2PDF/blob/main/README.md)
- **🐳 Docker Guide:** [Docker Usage Guide](https://github.com/brunoleocam/ZPL2PDF/blob/main/docs/DOCKER_GUIDE.md)
- **🌍 Languages:** [Multi-language Configuration](https://github.com/brunoleocam/ZPL2PDF/blob/main/docs/LANGUAGE_CONFIGURATION.md)

## Tags

- `latest` - Latest stable release (Alpine Linux)
- `2.0.0`, `2.0`, `2` - Specific version tags
- `alpine` - Alpine Linux base (ultra-lightweight)

## License

MIT License - See [LICENSE](https://github.com/brunoleocam/ZPL2PDF/blob/main/LICENSE)
````

4. Clique em "**Update**"

---

## ✅ **PASSO 3: VERIFICAR PUBLICAÇÃO**

### **3.1 - Testar GitHub Container Registry**

```powershell
# Remover imagem local (para testar download)
docker rmi ghcr.io/brunoleocam/zpl2pdf:latest

# Baixar do GitHub
docker pull ghcr.io/brunoleocam/zpl2pdf:latest

# Testar
docker run --rm ghcr.io/brunoleocam/zpl2pdf:latest /app/ZPL2PDF -help
```

### **3.2 - Testar Docker Hub**

```powershell
# Remover imagem local
docker rmi brunoleocam/zpl2pdf:latest

# Baixar do Docker Hub
docker pull brunoleocam/zpl2pdf:latest

# Testar
docker run --rm brunoleocam/zpl2pdf:latest /app/ZPL2PDF -help
```

### **3.3 - Verificar Online**

- **GitHub:** https://github.com/brunoleocam?tab=packages
- **Docker Hub:** https://hub.docker.com/r/brunoleocam/zpl2pdf

---

## 🤖 **PASSO 4: AUTOMAÇÃO COM GITHUB ACTIONS (OPCIONAL)**

Criar arquivo `.github/workflows/docker-publish.yml`:

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

### **Configurar Secrets:**

1. Acesse: https://github.com/brunoleocam/ZPL2PDF/settings/secrets/actions
2. Clique em "**New repository secret**"
3. Adicione:
   - `DOCKERHUB_USERNAME` = `brunoleocam`
   - `DOCKERHUB_TOKEN` = (criar em https://hub.docker.com/settings/security)

**Agora quando fizer um release, publica automaticamente!**

---

## 📊 **RESUMO DOS COMANDOS**

### **Build Local:**
```powershell
docker build -t zpl2pdf:alpine .
```

### **GitHub Container Registry:**
```powershell
# Login
echo $GITHUB_TOKEN | docker login ghcr.io -u brunoleocam --password-stdin

# Tag e Push
docker tag zpl2pdf:alpine ghcr.io/brunoleocam/zpl2pdf:latest
docker push ghcr.io/brunoleocam/zpl2pdf:latest
```

### **Docker Hub:**
```powershell
# Login
docker login

# Tag e Push
docker tag zpl2pdf:alpine brunoleocam/zpl2pdf:latest
docker push brunoleocam/zpl2pdf:latest
```

---

## ❓ **FAQ**

**P: Por que preciso do "brunoleocam/" no nome?**
R: É obrigatório no Docker Hub. Nomes sem prefixo são reservados para imagens oficiais.

**P: Posso usar só "zpl2pdf"?**
R: Usuários podem criar um alias local: `docker tag brunoleocam/zpl2pdf:latest zpl2pdf:latest`

**P: Qual o custo?**
R: GRÁTIS para repositórios públicos em ambos (GitHub e Docker Hub).

**P: Quanto tempo leva o upload?**
R: ~10 minutos para 470MB (depende da internet).

**P: Posso deletar depois?**
R: Sim, você é o dono e pode deletar a qualquer momento.

---

## ✅ **CHECKLIST**

- [ ] Conta GitHub criada
- [ ] Conta Docker Hub criada
- [ ] Token GitHub PAT criado
- [ ] Login no ghcr.io funcionando
- [ ] Login no Docker Hub funcionando
- [ ] Imagem taggeada para ghcr.io
- [ ] Imagem taggeada para Docker Hub
- [ ] Push para ghcr.io concluído
- [ ] Push para Docker Hub concluído
- [ ] Imagem pública no GitHub
- [ ] Repositório público no Docker Hub
- [ ] README configurado no Docker Hub
- [ ] Teste de download funcionando

---

## 🎉 **PRONTO!**

Após completar, usuários poderão:

```bash
# Opção 1: GitHub
docker run -d -v ./watch:/app/watch ghcr.io/brunoleocam/zpl2pdf:latest

# Opção 2: Docker Hub
docker run -d -v ./watch:/app/watch brunoleocam/zpl2pdf:latest
```

**Ambos funcionam e estão sincronizados!** ✅
