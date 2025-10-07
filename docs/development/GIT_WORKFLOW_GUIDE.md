# 🔀 Git Workflow Guide - ZPL2PDF

## 🎯 **Objetivo**

Este guia explica como trabalhar com Git no projeto ZPL2PDF, incluindo:
- Alterar usuário Git local
- Estratégia de branches
- Fluxo de desenvolvimento → release

---

## 👤 **1. ALTERAR USUÁRIO GIT (devdemobile → brunoleocam)**

### **Verificar Configuração Atual**

```bash
git config --global user.name
git config --global user.email
git config --global user.signingkey  # Se usar GPG
```

### **Alterar para brunoleocam**

```bash
# Alterar nome
git config --global user.name "brunoleocam"

# Alterar email
git config --global user.email "brunoleocam@users.noreply.github.com"

# OU usar seu email real
git config --global user.email "seuemail@gmail.com"

# Verificar
git config --global --list
```

### **Apenas para Este Repositório (Opcional)**

Se quiser usar `brunoleocam` apenas neste projeto:

```bash
cd C:\Dev\ZPL2PDF

# Local (só para este repo)
git config user.name "brunoleocam"
git config user.email "brunoleocam@users.noreply.github.com"

# Verificar
git config --local --list
```

---

## 🌳 **2. ESTRATÉGIA DE BRANCHES**

### **Branches Atuais no Projeto**

Segundo sua estrutura:

| Branch | Propósito | Status |
|--------|-----------|--------|
| `main` | **Produção** - Código estável, releases | Protegida |
| `dev` | **Desenvolvimento** - Integração de features | Ativa |
| `feature` | Features específicas | Temporária |
| `hotfix` | Correções urgentes | Temporária |

### **Fluxo Recomendado (Git Flow Simplificado)**

```
main (produção)
  ↑
  └── dev (desenvolvimento)
       ↑
       ├── feature/nova-funcionalidade
       ├── feature/outra-feature
       └── hotfix/correcao-urgente
```

---

## 🚀 **3. FLUXO DE TRABALHO ATUAL**

### **Situação Atual**

Você fez várias alterações locais que não estão no GitHub:
- ✅ WinGet manifests e scripts
- ✅ Documentação atualizada
- ✅ Correções de usuário (devdemobile → brunoleocam)
- ✅ GitHub Actions
- ✅ Multi-idioma completo

### **Opção 1: RECOMENDADA - Via Branch `dev`**

**Por quê?**
- ✅ Mais seguro - `main` fica protegida
- ✅ Permite revisão antes do merge
- ✅ Segue Git Flow padrão
- ✅ Pode testar CI/CD na branch `dev` primeiro

**Passo a passo:**

```bash
# 1. Verificar status atual
git status

# 2. Verificar branch atual
git branch

# 3. Mudar para branch dev
git checkout dev

# 4. Atualizar dev com main (se necessário)
git merge main

# 5. Adicionar todas as mudanças
git add .

# 6. Commit com mensagem descritiva
git commit -m "feat: add WinGet automation, update docs, fix username to brunoleocam

- Add WinGet manifests (4 YAML files)
- Add winget-submit.ps1 automation script
- Add GitHub Action for WinGet publishing
- Update all documentation (WINGET_GUIDE, CI_CD_WORKFLOW, etc.)
- Fix all devdemobile references to brunoleocam
- Update README with v2.0.0 features
- Update CHANGELOG with detailed v2.0.0 changes
- Add multi-language support documentation"

# 7. Push para dev
git push origin dev

# 8. Criar Pull Request: dev → main no GitHub
# Ir para: https://github.com/brunoleocam/ZPL2PDF/compare/main...dev
```

### **Opção 2: Direto para `main` (Mais Arriscado)**

**Use apenas se:**
- ❌ Você tem certeza absoluta que tudo funciona
- ❌ Não quer fazer revisão
- ❌ Quer release imediata

```bash
# 1. Verificar branch
git branch

# 2. Se não estiver em main, mudar
git checkout main

# 3. Adicionar tudo
git add .

# 4. Commit
git commit -m "feat: v2.0.0 - WinGet automation and documentation updates"

# 5. Push
git push origin main
```

---

## 📋 **4. CHECKLIST PRÉ-PUSH**

Antes de fazer push, verifique:

### **Verificações Essenciais**

```bash
# 1. Status do repositório
git status

# 2. Ver o que será commitado
git diff --cached

# 3. Verificar se não há arquivos sensíveis
git ls-files | grep -E "(\.env|\.key|password|secret)"

# 4. Ver commits locais não enviados
git log origin/main..HEAD

# 5. Verificar remote correto
git remote -v
```

### **Checklist Manual**

- [ ] ✅ Usuário Git alterado para `brunoleocam`
- [ ] ✅ Sem arquivos `.env` ou senhas commitados
- [ ] ✅ Build funcionando (`dotnet build`)
- [ ] ✅ Testes passando (`dotnet test`)
- [ ] ✅ Nenhum `TODO` ou `FIXME` crítico no código
- [ ] ✅ README atualizado
- [ ] ✅ CHANGELOG atualizado
- [ ] ✅ Sem referências a `devdemobile`

---

## 🔄 **5. APÓS O PUSH: CRIAR RELEASE**

### **Opção A: Via GitHub Web (Recomendado)**

1. **Ir para Releases**
   - https://github.com/brunoleocam/ZPL2PDF/releases

2. **Clicar em "Draft a new release"**

3. **Preencher:**
   - **Tag**: `v2.0.0`
   - **Target**: `main` (após merge de dev)
   - **Title**: `v2.0.0 - Multi-language, WinGet, Docker`
   - **Description**: Copiar de `CHANGELOG.md`

4. **Anexar Binários:**
   - `ZPL2PDF-Setup-2.0.0.exe` (Windows installer)
   - `ZPL2PDF-v2.0.0-win-x64.zip`
   - `ZPL2PDF-v2.0.0-win-x86.zip`
   - `ZPL2PDF-v2.0.0-linux-x64.tar.gz`
   - `ZPL2PDF-v2.0.0-osx-x64.tar.gz`
   - Etc... (todos os builds)

5. **Publicar**
   - ✅ Marca como "Latest release"
   - ✅ Dispara GitHub Actions (Docker, WinGet)

### **Opção B: Via Git CLI**

```bash
# 1. Criar tag local
git tag -a v2.0.0 -m "Release v2.0.0 - Multi-language, WinGet, Docker"

# 2. Push da tag
git push origin v2.0.0

# 3. Criar release no GitHub (via gh CLI)
gh release create v2.0.0 \
  --title "v2.0.0 - Multi-language, WinGet, Docker" \
  --notes-file CHANGELOG.md \
  build/publish/ZPL2PDF-Setup-2.0.0.exe \
  build/publish/ZPL2PDF-v2.0.0-*.zip \
  build/publish/ZPL2PDF-v2.0.0-*.tar.gz
```

---

## ⚙️ **6. O QUE ACONTECE APÓS A RELEASE**

Quando você criar a release `v2.0.0`, automaticamente:

### **GitHub Actions Dispara:**

```
Release v2.0.0 criada
        ↓
┌───────────────────────────────┐
│ .github/workflows/           │
│ docker-publish.yml           │ → Build + Push Docker
│                              │   (ghcr.io + Docker Hub)
└───────────────────────────────┘
        ↓
┌───────────────────────────────┐
│ .github/workflows/           │
│ winget-publish.yml           │ → Cria PR para
│                              │   microsoft/winget-pkgs
└───────────────────────────────┘
        ↓
    ✅ DONE!
```

**Resultado:**
- ✅ Docker: `docker pull brunoleocam/zpl2pdf:2.0.0`
- ✅ WinGet: PR criado (aguarda aprovação ~1-7 dias)
- ✅ Usuários podem baixar release manualmente

---

## 🔍 **7. VERIFICAR O QUE VAI SER COMMITADO**

Antes de fazer push, veja exatamente o que mudou:

```bash
# Ver arquivos modificados
git status

# Ver diferenças detalhadas
git diff

# Ver apenas nomes de arquivos
git diff --name-only

# Ver estatísticas
git diff --stat

# Ver diferenças staged (já adicionadas)
git diff --cached
```

---

## 🛡️ **8. PROTEÇÕES E SEGURANÇA**

### **Evitar Commits Acidentais**

```bash
# Ver o que SERIA commitado (dry-run)
git commit --dry-run

# Adicionar apenas arquivos específicos
git add arquivo1.cs arquivo2.md

# Adicionar por partes (interativo)
git add -p
```

### **Desfazer Mudanças (Antes do Push)**

```bash
# Desfazer último commit (mantém mudanças)
git reset --soft HEAD~1

# Desfazer último commit (descarta mudanças)
git reset --hard HEAD~1

# Desfazer mudanças em arquivo específico
git checkout -- arquivo.cs

# Remover arquivo do stage
git reset HEAD arquivo.cs
```

---

## 📊 **9. ESTRATÉGIA RECOMENDADA PARA VOCÊ**

### **Plano de Ação:**

```bash
# 1. PREPARAÇÃO
git config --global user.name "brunoleocam"
git config --global user.email "brunoleocam@users.noreply.github.com"

# 2. VERIFICAR ESTADO
git status
git log --oneline -10

# 3. CRIAR BRANCH DE TRABALHO (segurança extra)
git checkout -b release/v2.0.0

# 4. ADICIONAR TUDO
git add .

# 5. COMMIT
git commit -m "feat: v2.0.0 - WinGet automation, multi-language, Docker optimization

Major Changes:
- Add WinGet package automation (manifests + scripts + GitHub Action)
- Add comprehensive documentation (WINGET_GUIDE, CI_CD_WORKFLOW)
- Fix all devdemobile → brunoleocam references
- Update README with v2.0.0 features
- Update CHANGELOG with detailed v2.0.0 changes
- Add multi-language system documentation
- Optimize Docker image (Alpine 470MB)

Files Added:
- manifests/*.yaml (WinGet manifests)
- scripts/winget-submit.ps1 (automation script)
- .github/workflows/winget-publish.yml (GitHub Action)
- docs/development/WINGET_GUIDE.md
- docs/development/GIT_WORKFLOW_GUIDE.md

Files Updated:
- README.md (modern design, v2.0.0 features)
- CHANGELOG.md (detailed v2.0.0 changelog)
- CONTRIBUTING.md (updated release process)
- docs/development/CI_CD_WORKFLOW.md
- scripts/README.md"

# 6. PUSH BRANCH
git push origin release/v2.0.0

# 7. CRIAR PR: release/v2.0.0 → main
# Ir para: https://github.com/brunoleocam/ZPL2PDF/compare/main...release/v2.0.0

# 8. APÓS MERGE: CRIAR RELEASE
# https://github.com/brunoleocam/ZPL2PDF/releases/new
```

---

## 🎯 **10. CHECKLIST FINAL**

Antes de criar a release v2.0.0:

- [ ] ✅ Usuário Git alterado para `brunoleocam`
- [ ] ✅ Código commitado e pushed
- [ ] ✅ PR merged para `main` (se usou branch)
- [ ] ✅ Build completo executado (`.\scripts\build-all-platforms.ps1`)
- [ ] ✅ Installer compilado (`.\installer\build-installer.ps1`)
- [ ] ✅ Todos os binários testados
- [ ] ✅ Documentação revisada
- [ ] ✅ CHANGELOG completo
- [ ] ✅ Nenhuma referência a `devdemobile`

---

## 📚 **Referências Úteis**

- [Git Flow](https://nvie.com/posts/a-successful-git-branching-model/)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
- [GitHub Flow](https://guides.github.com/introduction/flow/)

---

**Este workflow garante segurança, rastreabilidade e facilita colaboração futura!** 🚀
