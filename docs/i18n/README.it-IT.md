# ZPL2PDF - Convertitore da ZPL a PDF

[![Version](https://img.shields.io/badge/version-3.0.2-blue.svg)](https://github.com/brunoleocam/ZPL2PDF/releases)
![GitHub all releases](https://img.shields.io/github/downloads/brunoleocam/ZPL2PDF/total)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![Docker](https://img.shields.io/badge/docker-Alpine%20470MB-blue.svg)](https://hub.docker.com/r/brunoleocam/zpl2pdf)
[![WinGet](https://img.shields.io/badge/winget-brunoleocam.ZPL2PDF-blue)](https://github.com/microsoft/winget-pkgs/tree/master/manifests/b/brunoleocam/ZPL2PDF)

**[English](../../README.md)** | **[Português-BR](README.pt-BR.md)** | **[Español](README.es-ES.md)** | **[Français](README.fr-FR.md)** | **[Deutsch](README.de-DE.md)** | **[Italiano](#)** | **[日本語](README.ja-JP.md)** | **[中文](README.zh-CN.md)**

Un potente strumento da riga di comando multipiattaforma che converte file ZPL (Zebra Programming Language) in documenti PDF di alta qualità. Perfetto per flussi di lavoro di stampa etichette, generazione automatica di documenti e sistemi di gestione etichette aziendali.

---

## 🚀 **Novità nella v3.1.0**

### 🐛 Correzioni
- **Issue #45**: Etichette duplicate o vuote quando `^XA` appare nel payload base64 di `~DGR:` — `^XA` è ora trattato come inizio etichetta solo all'inizio riga o dopo `^XZ`.

### ✨ Nuove Funzionalità
- **Issue #48 – Server TCP**: Modalità stampante Zebra virtuale implementata. Usare `ZPL2PDF server start --port 9101 -o output/`, `server stop` e `server status`.
- **REST API (PR #47)**: Eseguire `ZPL2PDF --api --host localhost --port 5000` per `POST /api/convert` (ZPL in PDF o PNG) e `GET /api/health`. [Guida API](../guides/API_GUIDE.md).

---

## 🚀 **Novità nella v3.1.0**

### 🐛 Correzioni
- **Issue #39**: Elaborazione sequenziale dei grafici per più grafici con lo stesso nome
  - I file ZPL con più grafici `~DGR` ora vengono elaborati correttamente
  - Ogni etichetta usa il grafico corretto in base allo stato sequenziale
  - I comandi di pulizia `^IDR` non generano più pagine vuote
  - Risolve il problema in cui tutte le etichette erano identiche nei file di etichette Shopee

### 🔧 Miglioramenti
- Validazione degli input nei metodi pubblici
- Gestione delle eccezioni migliorata
- Ottimizzazioni delle prestazioni con regex compilato
- Pulizia del codice e rimozione dei metodi non utilizzati

---

## 🚀 **Novità nella v3.1.0**

### 🎉 Principali Nuove Funzionalità
- 🎨 **Integrazione API Labelary** - Rendering ZPL ad alta fedeltà con output PDF vettoriale
- 🖨️ **Modalità Server TCP** - Stampante Zebra virtuale su porta TCP (predefinito: 9101)
- 🔤 **Font Personalizzati** - Carica font TrueType/OpenType con `--fonts-dir` e `--font`
- 📁 **Supporto File Esteso** - Aggiunte estensioni `.zpl` e `.imp`
- 📝 **Denominazione Personalizzata** - Imposta il nome del file di output tramite `^FX FileName:` in ZPL

### 🔧 Opzioni di Rendering
```bash
--renderer offline    # BinaryKits (predefinito, funziona offline)
--renderer labelary   # API Labelary (alta fedeltà, richiede internet)
--renderer auto       # Prova Labelary, fallback su BinaryKits
```

### 🖨️ Server TCP (Stampante Virtuale)
```bash
ZPL2PDF server start --port 9101 -o output/
ZPL2PDF server status
ZPL2PDF server stop
```

### Funzionalità v2.x (Ancora Disponibili)
- 🌍 **Supporto Multi-lingua** - 8 lingue (EN, PT, ES, FR, DE, IT, JA, ZH)
- 🔄 **Modalità Daemon** - Monitoraggio automatico delle cartelle e conversione batch
- 🏗️ **Architettura Pulita** - Completamente rifattorizzato con principi SOLID
- 🌍 **Multipiattaforma** - Supporto nativo per Windows, Linux e macOS
- 📐 **Dimensioni Intelligenti** - Estrazione automatica dimensioni ZPL (`^PW`, `^LL`)
- ⚡ **Alte Prestazioni** - Elaborazione asincrona con meccanismi di retry
- 🐳 **Supporto Docker** - Ottimizzato per Alpine Linux (470MB)
- 📦 **Installer Professionale** - Installer Windows con configurazione multi-lingua

---

## ✨ **Funzionalità Principali**

### 🎯 **Tre Modalità di Operazione**

#### **Modalità Conversione** - Convertire file singoli
```bash
ZPL2PDF -i etichetta.txt -o cartella_output/ -n mia_etichetta.pdf
```

#### **Modalità Daemon** - Monitoraggio automatico cartelle
```bash
ZPL2PDF start -l "C:\Etichette"
```

#### **Modalità Server TCP** - Stampante virtuale
```bash
ZPL2PDF server start --port 9101 -o cartella_output/
```

### 📐 **Gestione Intelligente delle Dimensioni**

- ✅ Estrarre dimensioni dai comandi ZPL (`^PW`, `^LL`)
- ✅ Supporto per più unità (mm, cm, pollici, punti)
- ✅ Fallback automatico a valori predefiniti sensati
- ✅ Risoluzione delle dimensioni basata su priorità

### 🌍 **Interfaccia Multi-lingua**

Imposta la tua lingua preferita:
```bash
# Temporaneo (sessione corrente)
ZPL2PDF --language it-IT status

# Permanente (tutte le sessioni)
ZPL2PDF --set-language it-IT

# Vedere la configurazione
ZPL2PDF --show-language
```

---

## 📦 **Installazione**

### **Windows**

#### Opzione 1: WinGet (Consigliato)
```powershell
winget install brunoleocam.ZPL2PDF
```

#### Opzione 2: Installer
1. Scaricare [ZPL2PDF-Setup-3.0.0.exe](https://github.com/brunoleocam/ZPL2PDF/releases/latest)
2. Eseguire l'installer
3. Scegliere la lingua durante l'installazione
4. Fatto! ✅

### **Linux**

#### Ubuntu/Debian (pacchetto .deb)
```bash
# Scaricare il pacchetto .deb
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v3.1.0/ZPL2PDF-v3.1.0-linux-amd64.deb

# Installare il pacchetto
sudo dpkg -i ZPL2PDF-v3.1.0-linux-amd64.deb

# Correggere le dipendenze se necessario
sudo apt-get install -f

# Verificare l'installazione
zpl2pdf --help
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

### **Convertire con Labelary (Alta Fedeltà)**
```bash
ZPL2PDF -i etichetta.txt -o cartella_output --renderer labelary
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

---

## 📖 **Guida all'Uso**

### **Parametri della Modalità Conversione**

| Parametro | Descrizione | Esempio |
|-----------|-------------|---------|
| `-i <file>` | File ZPL di input (.txt, .prn, .zpl, .imp) | `-i etichetta.zpl` |
| `-z <contenuto>` | Contenuto ZPL come stringa | `-z "^XA...^XZ"` |
| `-o <cartella>` | Cartella di output per PDF | `-o C:\Output` |
| `-n <nome>` | Nome del file PDF di output | `-n risultato.pdf` |
| `-w <larghezza>` | Larghezza dell'etichetta | `-w 10` |
| `-h <altezza>` | Altezza dell'etichetta | `-h 5` |
| `-u <unità>` | Unità (mm, cm, in) | `-u cm` |
| `-d <dpi>` | Densità di stampa (predefinito: 203) | `-d 300` |
| `--renderer` | Motore di rendering (offline/labelary/auto) | `--renderer labelary` |
| `--fonts-dir` | Directory font personalizzati | `--fonts-dir C:\Font` |
| `--font` | Mappare font specifico | `--font "A=arial.ttf"` |

### **Comandi Server TCP**

```bash
ZPL2PDF server start [opzioni]    # Avviare server TCP (stampante virtuale)
ZPL2PDF server stop               # Fermare server TCP
ZPL2PDF server status             # Verificare stato server TCP
```

---

## 🎨 **Motori di Rendering**

### **Offline (BinaryKits)** - Predefinito
- ✅ Funziona senza internet
- ✅ Elaborazione veloce
- ⚠️ Alcuni comandi ZPL potrebbero essere renderizzati diversamente

### **Labelary (API)** - Alta Fedeltà
- ✅ Emulazione esatta stampante Zebra
- ✅ Output PDF vettoriale (file più piccoli)
- ✅ Batching automatico per 50+ etichette
- ⚠️ Richiede connessione internet

### **Auto (Fallback)**
- ✅ Prova prima Labelary
- ✅ Fallback su BinaryKits se offline

---

## 📐 **Supporto ZPL**

### **Comandi Supportati**

- ✅ `^XA` / `^XZ` - Inizio/fine etichetta
- ✅ `^PW<larghezza>` - Larghezza di stampa in punti
- ✅ `^LL<lunghezza>` - Lunghezza dell'etichetta in punti
- ✅ `^FX FileName:` - Nome personalizzato del file di output
- ✅ `^FX !FileName:` - Nome forzato del file (sovrascrive `-n`)
- ✅ Tutti i comandi ZPL standard per testo, grafica e codici a barre

---

## 📚 **Documentazione**

- 📖 [Documentazione Completa](../README.md)
- 🌍 [Configurazione Multi-lingua](../LANGUAGE_CONFIGURATION.md)
- 🐳 [Guida Docker](../DOCKER_GUIDE.md)
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
