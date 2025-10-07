# 🏷️ ZPL2PDF - Convertitore da ZPL a PDF

[![Version](https://img.shields.io/badge/version-2.0.0-blue.svg)](https://github.com/brunoleocam/ZPL2PDF/releases)
![GitHub all releases](https://img.shields.io/github/downloads/brunoleocam/ZPL2PDF/total)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![Docker](https://img.shields.io/badge/docker-Alpine%20470MB-blue.svg)](https://hub.docker.com/r/brunoleocam/zpl2pdf)

**[English](../../README.md)** | **[Português-BR](README.pt-BR.md)** | **[Español](README.es-ES.md)** | **[Français](README.fr-FR.md)** | **[Deutsch](README.de-DE.md)** | **[Italiano](#)**

Un potente strumento da riga di comando multipiattaforma che converte file ZPL (Zebra Programming Language) in documenti PDF di alta qualità. Perfetto per flussi di lavoro di stampa etichette, generazione automatica di documenti e sistemi di gestione etichette aziendali.

---

## 🚀 **Novità nella v2.0**

- 🌍 **Supporto Multi-lingua** - 8 lingue (EN, PT, ES, FR, DE, IT, JA, ZH)
- 🔄 **Modalità Daemon** - Monitoraggio automatico delle cartelle e conversione batch
- 🏗️ **Architettura Pulita** - Completamente rifattorizzato con principi SOLID
- 🌍 **Multipiattaforma** - Supporto nativo per Windows, Linux e macOS
- 📐 **Dimensioni Intelligenti** - Estrazione automatica dimensioni ZPL (`^PW`, `^LL`)
- ⚡ **Alte Prestazioni** - Elaborazione asincrona con meccanismi di retry
- 🐳 **Supporto Docker** - Ottimizzato per Alpine Linux (470MB)
- 📦 **Installer Professionale** - Installer Windows con configurazione multi-lingua

---

## 📦 **Installazione**

### **Windows**

#### Opzione 1: WinGet (Consigliato)
```powershell
winget install brunoleocam.ZPL2PDF
```

#### Opzione 2: Installer
1. Scaricare [ZPL2PDF-Setup-2.0.0.exe](https://github.com/brunoleocam/ZPL2PDF/releases/latest)
2. Eseguire l'installer
3. Scegliere la lingua durante l'installazione
4. Fatto! ✅

### **Linux**

```bash
# Scaricare
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-x64.tar.gz

# Estrarre
tar -xzf ZPL2PDF-v2.0.0-linux-x64.tar.gz

# Eseguire
./ZPL2PDF -help
```

### **Docker**

```bash
docker pull brunoleocam/zpl2pdf:latest
docker run -v ./watch:/app/watch -v ./output:/app/output brunoleocam/zpl2pdf:latest
```

---

## 🚀 **Avvio Rapido**

### **Convertire un File**
```bash
ZPL2PDF -i etichetta.txt -o cartella_output -n mia_etichetta.pdf
```

### **Modalità Daemon (Auto-Conversione)**
```bash
# Avviare con configurazione predefinita
ZPL2PDF start

# Avviare con cartella personalizzata
ZPL2PDF start -l "C:\Etichette" -w 7.5 -h 15 -u in

# Verificare lo stato
ZPL2PDF status

# Fermare il daemon
ZPL2PDF stop
```

### **Configurare la Lingua**
```bash
# Temporaneo (sessione corrente)
ZPL2PDF --language it-IT status

# Permanente (tutte le sessioni)
ZPL2PDF --set-language it-IT

# Vedere la configurazione
ZPL2PDF --show-language
```

---

## 📚 **Documentazione**

- 📖 [Documentazione Completa](../README.md)
- 🌍 [Configurazione Multi-lingua](../guides/LANGUAGE_CONFIGURATION.md)
- 🐳 [Guida Docker](../guides/DOCKER_GUIDE.md)
- 🛠️ [Guida al Contributo](../../CONTRIBUTING.md)
- 📋 [Registro delle Modifiche](../../CHANGELOG.md)

---

## 🤝 **Contribuire**

Accettiamo contributi! Vedere [CONTRIBUTING.md](../../CONTRIBUTING.md) per i dettagli.

---

## 📄 **Licenza**

Questo progetto è concesso in licenza sotto la Licenza MIT - vedere il file [LICENSE](../../LICENSE) per i dettagli.

---

## 👥 **Contributori**

Grazie a tutti i contributori che hanno aiutato a migliorare ZPL2PDF!

<a href="https://github.com/brunoleocam/ZPL2PDF/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=brunoleocam/ZPL2PDF" />
</a>

---

**ZPL2PDF** - Converti etichette ZPL in PDF facilmente ed efficientemente.

