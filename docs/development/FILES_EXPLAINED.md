# 📁 Explicação dos Arquivos do Projeto

## 🎯 **Propósito deste Documento**

Este guia explica o propósito de cada arquivo importante no projeto ZPL2PDF, especialmente aqueles que podem gerar dúvidas.

---

## 📋 **ARQUIVOS QUE FORAM REMOVIDOS**

### ❌ **winget-manifest.yaml** (raiz)

**Era:** Manifest WinGet no formato antigo (v1.4.0, arquivo único)

**Por que foi removido:**
- Formato desatualizado (WinGet v1.6.0 usa 4 arquivos separados)
- Substituído pelos novos manifests em `manifests/`

**Substituído por:**
- `manifests/brunoleocam.ZPL2PDF.yaml` (version manifest)
- `manifests/brunoleocam.ZPL2PDF.installer.yaml` (installer details)
- `manifests/brunoleocam.ZPL2PDF.locale.en-US.yaml` (English locale)
- `manifests/brunoleocam.ZPL2PDF.locale.pt-BR.yaml` (Portuguese locale)

---

### ❌ **README.pt.md** (raiz)

**Era:** README em português duplicado

**Por que foi removido:**
- Duplicação - mesma informação que `docs/i18n/README.pt-BR.md`
- Melhor organização em `docs/i18n/`

**Substituído por:**
- `docs/i18n/README.pt-BR.md` (versão correta e atualizada)

---

### ❌ **PR_MERGE_TROUBLESHOOTING.md**

**Era:** Guia temporário para resolver conflitos de PR

**Por que foi removido:**
- Instrução pontual que já foi usada
- Não é útil para usuários finais
- Informação já está em `GIT_WORKFLOW_GUIDE.md`

---

### ❌ **DOCKER_RESUMO_PUBLICACAO.md**

**Era:** Resumo da otimização Docker

**Por que foi removido:**
- Informação duplicada
- Conteúdo já está em `DOCKER_GUIDE.md` e `DOCKER_PUBLISH_GUIDE.md`
- Era apenas para referência durante desenvolvimento

---

## 📂 **ARQUIVOS IMPORTANTES QUE PERMANECEM**

### ✅ **manifests/** (pasta)

**Propósito:** Armazena os manifests WinGet no formato correto (v1.6.0)

**Estrutura:**
```
manifests/
├── brunoleocam.ZPL2PDF.yaml              # Version manifest
├── brunoleocam.ZPL2PDF.installer.yaml    # Installer details (URL, SHA256)
├── brunoleocam.ZPL2PDF.locale.en-US.yaml # English locale
├── brunoleocam.ZPL2PDF.locale.pt-BR.yaml # Portuguese locale
└── README.md                             # Instruções de uso
```

**Quando é usado:**
- Script `scripts/winget-submit.ps1` lê estes arquivos
- GitHub Action `.github/workflows/winget-publish.yml` usa eles
- Submete PR automaticamente para `microsoft/winget-pkgs`

**NÃO DELETAR:** Essencial para automação WinGet!

---

### ✅ **zpl2pdf.json.example**

**Propósito:** Template de configuração para usuários

**Como usar:**
1. Copiar para `zpl2pdf.json`
2. Editar com preferências pessoais
3. Colocar na pasta do executável

**Exemplo:**
```json
{
  "language": "pt-BR",
  "defaultWatchFolder": "C:\\Labels",
  "labelWidth": 10,
  "labelHeight": 5,
  "unit": "cm",
  "dpi": 203
}
```

**NÃO DELETAR:** Referência essencial para usuários!

---

### ✅ **CHANGELOG.md**

**Propósito:** Histórico de todas as versões do projeto

**Formato:** [Keep a Changelog](https://keepachangelog.com/)

**Estrutura:**
```markdown
## [2.0.0] - 2025-01-07
### Added
- Nova funcionalidade X
### Changed
- Mudança Y
### Fixed
- Correção Z
```

**Quando atualizar:**
- A cada release
- Documentar TODAS as mudanças
- Ajuda usuários a entender o que mudou

**NÃO DELETAR:** Obrigatório para releases profissionais!

---

### ✅ **CONTRIBUTING.md**

**Propósito:** Guia para contribuidores do projeto

**Conteúdo:**
- Como configurar ambiente de desenvolvimento
- Padrões de código
- Processo de Pull Request
- Como fazer releases

**Quando usar:**
- Novos contribuidores
- Antes de criar PR
- Referência para padrões

**NÃO DELETAR:** Essencial para open source!

---

### ✅ **.cursor/rules/** (pasta)

**Propósito:** Regras e contexto para o AI assistant (Cursor)

**Arquivos:**
- `cursorrules.mdc` - Regras de implementação (sempre aplicadas)
- `implementation.mdc` - Plano detalhado e TODO list

**Quando é usado:**
- Cursor AI lê estes arquivos automaticamente
- Mantém contexto do projeto
- Garante padrões de código

**NÃO DELETAR:** Ajuda na manutenção futura!

---

### ✅ **rpm/** (pasta)

**Propósito:** Arquivos para criar pacotes RPM (Red Hat/CentOS/Fedora)

**Estrutura:**
```
rpm/
└── zpl2pdf.spec    # Especificação do pacote RPM
```

**Como criar pacote RPM:**
```bash
rpmbuild -ba rpm/zpl2pdf.spec
```

**Distribuições suportadas:**
- Red Hat Enterprise Linux (RHEL)
- CentOS
- Fedora
- openSUSE

**Status:** ⏳ Estrutura criada, mas não publicado ainda

**NÃO DELETAR:** Será usado para distribuição Linux!

---

### ✅ **debian/** (pasta)

**Propósito:** Arquivos para criar pacotes .deb (Debian/Ubuntu)

**Estrutura:**
```
debian/
└── control    # Metadados do pacote
```

**Como criar pacote .deb:**
```bash
dpkg-deb --build debian zpl2pdf_2.0.0_amd64.deb
```

**Distribuições suportadas:**
- Debian
- Ubuntu
- Linux Mint
- Pop!_OS

**Status:** ⏳ Estrutura criada, mas não publicado ainda

**NÃO DELETAR:** Será usado para distribuição Linux!

---

### ✅ **scripts/** (pasta)

**Propósito:** Scripts de automação de build e release

**Arquivos:**
| Arquivo | Propósito |
|---------|-----------|
| `build-all-platforms.ps1/.sh` | Build para 8 plataformas |
| `release.ps1/.sh` | Automação completa de release |
| `winget-submit.ps1` | Submeter para WinGet automaticamente |
| `build-installer.ps1` | Compilar installer Inno Setup |
| `README.md` | Documentação dos scripts |

**NÃO DELETAR:** Essenciais para build e distribuição!

---

### ✅ **installer/** (pasta)

**Propósito:** Arquivos para criar instalador Windows (Inno Setup)

**Estrutura:**
```
installer/
├── ZPL2PDF-Setup.iss        # Script Inno Setup
├── build-installer.ps1      # Script de compilação
└── README.md                # Documentação
```

**Como usar:**
```powershell
.\installer\build-installer.ps1
```

**Resultado:** `ZPL2PDF-Setup-2.0.0.exe` (35.46 MB)

**NÃO DELETAR:** Essencial para usuários Windows!

---

### ✅ **.github/workflows/** (pasta)

**Propósito:** GitHub Actions (CI/CD automation)

**Arquivos:**
| Workflow | Trigger | Ação |
|----------|---------|------|
| `docker-publish.yml` | Release | Build + Publish Docker |
| `winget-publish.yml` | Release | Criar PR WinGet |
| `ci.yml` | Push/PR | Testes automáticos |

**Quando executa:**
- Automaticamente ao criar release
- A cada push para `main`

**NÃO DELETAR:** Automação CI/CD depende deles!

---

## 📊 **RESUMO: O QUE MANTER vs DELETAR**

### ✅ **MANTER (ESSENCIAIS):**

| Arquivo/Pasta | Propósito |
|---------------|-----------|
| `manifests/` | WinGet automation |
| `scripts/` | Build automation |
| `installer/` | Windows installer |
| `debian/` | Linux .deb packages |
| `rpm/` | Linux .rpm packages |
| `.github/workflows/` | CI/CD automation |
| `zpl2pdf.json.example` | Configuration template |
| `CHANGELOG.md` | Version history |
| `CONTRIBUTING.md` | Contribution guide |
| `.cursor/rules/` | AI assistant context |
| `docs/` | All documentation |

### ❌ **JÁ REMOVIDOS (REDUNDANTES):**

| Arquivo | Motivo |
|---------|--------|
| `winget-manifest.yaml` | Formato antigo |
| `README.pt.md` | Duplicado |
| `PR_MERGE_TROUBLESHOOTING.md` | Temporário |
| `DOCKER_RESUMO_PUBLICACAO.md` | Duplicado |

---

## 🎯 **MANUTENÇÃO FUTURA**

### **Quando adicionar novo arquivo, pergunte:**

1. ✅ É essencial para usuários finais?
2. ✅ É essencial para build/distribuição?
3. ✅ É essencial para CI/CD?
4. ❌ É temporário/pontual?
5. ❌ Duplica informação existente?

**Se respondeu SIM para 1-3:** Manter e documentar
**Se respondeu SIM para 4-5:** Deletar ou mover para pasta temporária

---

**Mantenha o projeto limpo e bem documentado!** 🚀
