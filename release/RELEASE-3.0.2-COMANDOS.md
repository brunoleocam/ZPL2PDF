# Release 3.0.2 – Comandos para builds, instalador e Docker

Execute na raiz do projeto (`c:\Dev\ZPL\ZPL2PDF`) no PowerShell.  
Recomendado: executar um passo por vez e conferir o resultado antes do próximo.

---

## 1. Builds multi-plataforma (todas as plataformas)

Gera binários em `Assets\` (zip/tar.gz para Windows, Linux e macOS).

```powershell
.\release\07-build-all-platforms.ps1 -Version "3.0.2"
```

**Com testes (padrão):** leva mais tempo.  
**Sem testes (mais rápido):**

```powershell
.\release\07-build-all-platforms.ps1 -Version "3.0.2" -SkipTests
```

Saída esperada: `Assets\ZPL2PDF-v3.0.2-win-x64.zip`, `Assets\ZPL2PDF-v3.0.2-linux-x64.tar.gz`, etc., e `Assets\SHA256SUMS.txt`.

---

## 2. Instalador Windows (Inno Setup)

Requer Inno Setup 6 e usa o publish win-x64 (ou faz o publish se não existir).

```powershell
.\release\08-build-installer.ps1 -Version "3.0.2"
```

Saída esperada: `installer\Output\ZPL2PDF-Setup-3.0.2.exe` (e cópia em `Assets\` se o script fizer isso).

---

## 3. Imagens Docker

Requer Docker em execução.

```powershell
.\release\11-build-docker-images.ps1 -Version "3.0.2"
```

Cria as tags: `brunoleocam/zpl2pdf:3.0.2`, `latest`, `alpine`, `3.0`, `3` e equivalentes em `ghcr.io/brunoleocam/zpl2pdf`.

---

## Ordem sugerida

1. **07** (builds) → 2. **08** (instalador) → 3. **11** (Docker).

O passo 08 pode ser executado sem o 07: o script `scripts\build-installer.ps1` faz `dotnet publish` para win-x64 se necessário.

---

## Publicação (opcional)

- **Docker Hub:** `.\release\14-publish-dockerhub.ps1 -Version "3.0.2"` (após `docker login`).
- **GHCR:** `.\release\15-publish-ghcr.ps1 -Version "3.0.2"` (token com `write:packages`).

---

## Remover docs do Git (uma vez)

Os arquivos abaixo já estão no `.gitignore`. Para parar de versioná-los **mantendo os arquivos no disco**, execute (uma vez):

```powershell
git rm --cached docs/release-notes-v3.0.0.md docs/ROADMAP.md docs/zebra-fonts-research.md docs/LABELARY_API.md
```

Se quiser deixar de versionar também o relatório de testes manuais:

```powershell
git rm --cached TESTES-MANUAIS-3.0.2.md
```

Depois faça commit das alterações (incluindo o `.gitignore` atualizado).
