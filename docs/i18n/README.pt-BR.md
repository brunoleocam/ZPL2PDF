# ZPL2PDF - Conversor ZPL para PDF

[![Version](https://img.shields.io/badge/version-3.0.2-blue.svg)](https://github.com/brunoleocam/ZPL2PDF/releases)
![GitHub all releases](https://img.shields.io/github/downloads/brunoleocam/ZPL2PDF/total)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![Docker](https://img.shields.io/badge/docker-Alpine%20470MB-blue.svg)](https://hub.docker.com/r/brunoleocam/zpl2pdf)
[![WinGet](https://img.shields.io/badge/winget-brunoleocam.ZPL2PDF-blue)](https://github.com/microsoft/winget-pkgs/tree/master/manifests/b/brunoleocam/ZPL2PDF)

**[English](../../README.md)** | **[Português-BR](#)** | **[Español](README.es-ES.md)** | **[Français](README.fr-FR.md)** | **[Deutsch](README.de-DE.md)** | **[Italiano](README.it-IT.md)** | **[日本語](README.ja-JP.md)** | **[中文](README.zh-CN.md)**

Uma ferramenta poderosa e multiplataforma que converte arquivos ZPL (Zebra Programming Language) para documentos PDF de alta qualidade. Perfeita para fluxos de trabalho de impressão de etiquetas, geração automatizada de documentos e sistemas empresariais de gerenciamento de etiquetas.

---

## 🚀 **Novidades na v3.0.0**

### 🎉 Principais Novas Funcionalidades
- 🎨 **Integração com API Labelary** - Renderização ZPL de alta fidelidade com saída PDF vetorial
- 🖨️ **Modo Servidor TCP** - Impressora Zebra virtual em porta TCP (padrão: 9101)
- 🔤 **Fontes Personalizadas** - Carregue fontes TrueType/OpenType com `--fonts-dir` e `--font`
- 📁 **Suporte Estendido de Arquivos** - Adicionadas extensões `.zpl` e `.imp`
- 📝 **Nomeação Personalizada** - Defina nome do arquivo de saída via `^FX FileName:` no ZPL

### 🔧 Opções de Renderização
```bash
--renderer offline    # BinaryKits (padrão, funciona offline)
--renderer labelary   # API Labelary (alta fidelidade, requer internet)
--renderer auto       # Tenta Labelary, fallback para BinaryKits
```

### 🖨️ Servidor TCP (Impressora Virtual)
```bash
ZPL2PDF server start --port 9101 -o output/
ZPL2PDF server status
ZPL2PDF server stop
```

### Funcionalidades v2.x (Ainda Disponíveis)
- 🌍 **Suporte Multi-idioma** - 8 idiomas (EN, PT, ES, FR, DE, IT, JA, ZH)
- 🔄 **Modo Daemon** - Monitoramento automático de pastas e conversão em lote
- 🏗️ **Arquitetura Limpa** - Completamente refatorado com princípios SOLID
- 🌍 **Multiplataforma** - Suporte nativo para Windows, Linux e macOS
- 📐 **Dimensões Inteligentes** - Extração automática de dimensões ZPL (`^PW`, `^LL`)
- ⚡ **Alta Performance** - Processamento assíncrono com mecanismos de retry
- 🐳 **Suporte Docker** - Alpine Linux otimizado (470MB)
- 📦 **Instalador Profissional** - Instalador Windows com configuração multi-idioma

---

## ✨ **Principais Recursos**

### 🎯 **Três Modos de Operação**

#### **Modo Conversão** - Converter arquivos individuais
```bash
ZPL2PDF -i etiqueta.txt -o pasta_saida/ -n minha_etiqueta.pdf
```

#### **Modo Daemon** - Monitoramento automático de pastas
```bash
ZPL2PDF start -l "C:\Etiquetas"
```

#### **Modo Servidor TCP** - Impressora virtual
```bash
ZPL2PDF server start --port 9101 -o pasta_saida/
```

### 📐 **Gerenciamento Inteligente de Dimensões**

- ✅ Extrair dimensões dos comandos ZPL (`^PW`, `^LL`)
- ✅ Suporte para múltiplas unidades (mm, cm, polegadas, pontos)
- ✅ Fallback automático para padrões sensatos
- ✅ Resolução de dimensões baseada em prioridade

### 🌍 **Interface Multi-idioma**

Defina seu idioma preferido:
```bash
# Temporário (sessão atual)
ZPL2PDF --language pt-BR status

# Permanente (todas as sessões)
ZPL2PDF --set-language pt-BR

# Verificar configuração
ZPL2PDF --show-language
```

**Idiomas Suportados:**
- 🇺🇸 English (en-US)
- 🇧🇷 Português (pt-BR)
- 🇪🇸 Español (es-ES)
- 🇫🇷 Français (fr-FR)
- 🇩🇪 Deutsch (de-DE)
- 🇮🇹 Italiano (it-IT)
- 🇯🇵 日本語 (ja-JP)
- 🇨🇳 中文 (zh-CN)

---

## 📦 **Instalação**

### **Windows**

#### Opção 1: WinGet (Recomendado)
```powershell
winget install brunoleocam.ZPL2PDF
```

#### Opção 2: Instalador
1. Baixe [ZPL2PDF-Setup-3.0.0.exe](https://github.com/brunoleocam/ZPL2PDF/releases/latest)
2. Execute o instalador
3. Escolha seu idioma durante a instalação
4. Pronto! ✅

### **Linux**

#### Ubuntu/Debian (pacote .deb)
```bash
# Baixar pacote .deb das releases
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v3.0.0/ZPL2PDF-v3.0.0-linux-amd64.deb

# Instalar pacote
sudo dpkg -i ZPL2PDF-v3.0.0-linux-amd64.deb

# Corrigir dependências se necessário
sudo apt-get install -f

# Verificar instalação
zpl2pdf --help
```

#### Docker (Todas as distribuições Linux)
```bash
docker pull brunoleocam/zpl2pdf:latest
docker run -v ./watch:/app/watch -v ./output:/app/output brunoleocam/zpl2pdf:latest
```

---

## 🚀 **Início Rápido**

### **Converter um Arquivo**
```bash
ZPL2PDF -i etiqueta.txt -o pasta_saida -n minha_etiqueta.pdf
```

### **Converter com Dimensões Personalizadas**
```bash
ZPL2PDF -i etiqueta.txt -o pasta_saida -w 10 -h 5 -u cm
```

### **Converter com Labelary (Alta Fidelidade)**
```bash
ZPL2PDF -i etiqueta.txt -o pasta_saida --renderer labelary
```

### **Iniciar Modo Daemon (Conversão Automática)**
```bash
# Iniciar com configurações padrão
ZPL2PDF start

# Iniciar com pasta personalizada
ZPL2PDF start -l "C:\Etiquetas" -w 7.5 -h 15 -u in

# Verificar status
ZPL2PDF status

# Parar daemon
ZPL2PDF stop
```

---

## 📖 **Guia de Uso**

### **Parâmetros do Modo Conversão**

```bash
ZPL2PDF -i <arquivo_entrada> -o <pasta_saida> [opções]
ZPL2PDF -z <conteudo_zpl> -o <pasta_saida> [opções]
```

| Parâmetro | Descrição | Exemplo |
|-----------|-----------|---------|
| `-i <arquivo>` | Arquivo ZPL de entrada (.txt, .prn, .zpl, .imp) | `-i etiqueta.zpl` |
| `-z <conteudo>` | Conteúdo ZPL como string | `-z "^XA...^XZ"` |
| `-o <pasta>` | Pasta de saída para PDF | `-o C:\Saida` |
| `-n <nome>` | Nome do arquivo PDF de saída (opcional) | `-n resultado.pdf` |
| `-w <largura>` | Largura da etiqueta | `-w 10` |
| `-h <altura>` | Altura da etiqueta | `-h 5` |
| `-u <unidade>` | Unidade (mm, cm, in) | `-u cm` |
| `-d <dpi>` | Densidade de impressão (padrão: 203) | `-d 300` |
| `--renderer` | Motor de renderização (offline/labelary/auto) | `--renderer labelary` |
| `--fonts-dir` | Diretório de fontes personalizadas | `--fonts-dir C:\Fontes` |
| `--font` | Mapear fonte específica | `--font "A=arial.ttf"` |

### **Comandos do Modo Daemon**

```bash
ZPL2PDF start [opções]    # Iniciar daemon em background
ZPL2PDF stop              # Parar daemon
ZPL2PDF status            # Verificar status do daemon
ZPL2PDF run [opções]      # Executar daemon em foreground (teste)
```

### **Comandos do Servidor TCP**

```bash
ZPL2PDF server start [opções]    # Iniciar servidor TCP (impressora virtual)
ZPL2PDF server stop              # Parar servidor TCP
ZPL2PDF server status            # Verificar status do servidor TCP
```

| Opção | Descrição | Padrão |
|-------|-----------|--------|
| `--port <porta>` | Porta TCP para escutar | `9101` |
| `-o <pasta>` | Pasta de saída para PDFs | `Documents/ZPL2PDF TCP Output` |
| `--foreground` | Executar em foreground (não background) | Background |
| `--renderer` | Motor de renderização | `offline` |

---

## 🎨 **Motores de Renderização**

### **Offline (BinaryKits)** - Padrão
```bash
ZPL2PDF -i etiqueta.txt -o saida/ --renderer offline
```
- ✅ Funciona sem internet
- ✅ Processamento rápido
- ⚠️ Alguns comandos ZPL podem renderizar diferente

### **Labelary (API)** - Alta Fidelidade
```bash
ZPL2PDF -i etiqueta.txt -o saida/ --renderer labelary
```
- ✅ Emulação exata de impressora Zebra
- ✅ Saída PDF vetorial (arquivos menores)
- ✅ Batching automático para 50+ etiquetas
- ⚠️ Requer conexão com internet

### **Auto (Fallback)**
```bash
ZPL2PDF -i etiqueta.txt -o saida/ --renderer auto
```
- ✅ Tenta Labelary primeiro
- ✅ Fallback para BinaryKits se offline

---

## 📐 **Suporte ZPL**

### **Comandos Suportados**

- ✅ `^XA` / `^XZ` - Início/fim da etiqueta
- ✅ `^PW<largura>` - Largura de impressão em pontos
- ✅ `^LL<comprimento>` - Comprimento da etiqueta em pontos
- ✅ `^FX FileName:` - Nome personalizado do arquivo de saída
- ✅ `^FX !FileName:` - Nome forçado do arquivo (sobrescreve `-n`)
- ✅ Todos os comandos ZPL padrão de texto, gráficos e códigos de barras

### **Extração de Dimensões**

O ZPL2PDF extrai automaticamente as dimensões:

```zpl
^XA
^PW800        ← Largura: 800 pontos
^LL1200       ← Altura: 1200 pontos
^FO50,50^A0N,50,50^FDOlá^FS
^XZ
```

**Conversão:** `mm = (pontos / 203) * 25.4`

---

## 🐳 **Uso com Docker**

### **Início Rápido com Docker**

```bash
# Baixar imagem
docker pull brunoleocam/zpl2pdf:latest

# Executar modo daemon
docker run -d \
  --name zpl2pdf \
  -v ./watch:/app/watch \
  -v ./output:/app/output \
  -e ZPL2PDF_LANGUAGE=pt-BR \
  brunoleocam/zpl2pdf:latest
```

📘 **Guia Completo Docker:** [docs/DOCKER_GUIDE.md](../DOCKER_GUIDE.md)

---

## 📚 **Documentação**

- 📖 [Documentação Completa](../README.md)
- 🌍 [Configuração Multi-idioma](../LANGUAGE_CONFIGURATION.md)
- 🐳 [Guia Docker](../DOCKER_GUIDE.md)
- 🛠️ [Guia de Contribuição](../../CONTRIBUTING.md)
- 📋 [Changelog](../../CHANGELOG.md)

---

## 🤝 **Contribuindo**

Aceitamos contribuições! Consulte [CONTRIBUTING.md](../../CONTRIBUTING.md) para detalhes.

---

## 📄 **Licença**

Este projeto está licenciado sob a Licença MIT - consulte o arquivo [LICENSE](../../LICENSE) para detalhes.

---

## 🙏 **Agradecimentos**

Construído com bibliotecas open-source incríveis:

- [BinaryKits.Zpl](https://github.com/BinaryKits/BinaryKits.Zpl) - Parsing e renderização ZPL
- [PdfSharpCore](https://github.com/empira/PdfSharpCore) - Geração de PDF
- [SkiaSharp](https://github.com/mono/SkiaSharp) - Gráficos multiplataforma

---

## 👥 **Contribuidores**

Obrigado a todos os contribuidores que ajudaram a melhorar o ZPL2PDF!

<a href="https://github.com/brunoleocam/ZPL2PDF/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=brunoleocam/ZPL2PDF" />
</a>

---

**ZPL2PDF** - Converta etiquetas ZPL para PDF de forma fácil e eficiente.
