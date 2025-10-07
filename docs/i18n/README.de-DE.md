# 🏷️ ZPL2PDF - ZPL zu PDF Konverter

[![Version](https://img.shields.io/badge/version-2.0.0-blue.svg)](https://github.com/brunoleocam/ZPL2PDF/releases)
![GitHub all releases](https://img.shields.io/github/downloads/brunoleocam/ZPL2PDF/total)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![Docker](https://img.shields.io/badge/docker-Alpine%20470MB-blue.svg)](https://hub.docker.com/r/brunoleocam/zpl2pdf)

**[English](../../README.md)** | **[Português-BR](README.pt-BR.md)** | **[Español](README.es-ES.md)** | **[Français](README.fr-FR.md)** | **[Deutsch](#)**

Ein leistungsstarkes, plattformübergreifendes Befehlszeilenwerkzeug, das ZPL-Dateien (Zebra Programming Language) in hochwertige PDF-Dokumente konvertiert. Perfekt für Etikettendruck-Workflows, automatisierte Dokumentenerstellung und Unternehmens-Etikettenverwaltungssysteme.

---

## 🚀 **Neu in v2.0**

- 🌍 **Mehrsprachige Unterstützung** - 8 Sprachen (EN, PT, ES, FR, DE, IT, JA, ZH)
- 🔄 **Daemon-Modus** - Automatische Ordnerüberwachung und Stapelkonvertierung
- 🏗️ **Clean Architecture** - Vollständig überarbeitet mit SOLID-Prinzipien
- 🌍 **Plattformübergreifend** - Native Unterstützung für Windows, Linux und macOS
- 📐 **Intelligente Dimensionen** - Automatische ZPL-Dimensionsextraktion (`^PW`, `^LL`)
- ⚡ **Hohe Leistung** - Asynchrone Verarbeitung mit Wiederholungsmechanismen
- 🐳 **Docker-Unterstützung** - Optimiert für Alpine Linux (470MB)
- 📦 **Professioneller Installer** - Windows-Installer mit mehrsprachiger Einrichtung

---

## 📦 **Installation**

### **Windows**

#### Option 1: WinGet (Empfohlen)
```powershell
winget install brunoleocam.ZPL2PDF
```

#### Option 2: Installer
1. [ZPL2PDF-Setup-2.0.0.exe](https://github.com/brunoleocam/ZPL2PDF/releases/latest) herunterladen
2. Installer ausführen
3. Sprache während der Installation wählen
4. Fertig! ✅

### **Linux**

```bash
# Herunterladen
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-x64.tar.gz

# Extrahieren
tar -xzf ZPL2PDF-v2.0.0-linux-x64.tar.gz

# Ausführen
./ZPL2PDF -help
```

### **Docker**

```bash
docker pull brunoleocam/zpl2pdf:latest
docker run -v ./watch:/app/watch -v ./output:/app/output brunoleocam/zpl2pdf:latest
```

---

## 🚀 **Schnellstart**

### **Eine Datei Konvertieren**
```bash
ZPL2PDF -i etikett.txt -o ausgabeordner -n mein_etikett.pdf
```

### **Daemon-Modus (Auto-Konvertierung)**
```bash
# Mit Standardeinstellungen starten
ZPL2PDF start

# Mit benutzerdefiniertem Ordner starten
ZPL2PDF start -l "C:\Etiketten" -w 7.5 -h 15 -u in

# Status prüfen
ZPL2PDF status

# Daemon stoppen
ZPL2PDF stop
```

### **Sprache Konfigurieren**
```bash
# Temporär (aktuelle Sitzung)
ZPL2PDF --language de-DE status

# Permanent (alle Sitzungen)
ZPL2PDF --set-language de-DE

# Konfiguration anzeigen
ZPL2PDF --show-language
```

---

## 📚 **Dokumentation**

- 📖 [Vollständige Dokumentation](../README.md)
- 🌍 [Mehrsprachige Konfiguration](../guides/LANGUAGE_CONFIGURATION.md)
- 🐳 [Docker-Anleitung](../guides/DOCKER_GUIDE.md)
- 🛠️ [Beitragsanleitung](../../CONTRIBUTING.md)
- 📋 [Änderungsprotokoll](../../CHANGELOG.md)

---

## 🤝 **Beitragen**

Wir begrüßen Beiträge! Siehe [CONTRIBUTING.md](../../CONTRIBUTING.md) für Details.

---

## 📄 **Lizenz**

Dieses Projekt ist unter der MIT-Lizenz lizenziert - siehe [LICENSE](../../LICENSE) für Details.

---

## 👥 **Mitwirkende**

Danke an alle Mitwirkenden, die geholfen haben, ZPL2PDF besser zu machen!

<a href="https://github.com/brunoleocam/ZPL2PDF/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=brunoleocam/ZPL2PDF" />
</a>

---

**ZPL2PDF** - Konvertieren Sie ZPL-Etiketten einfach und effizient in PDF.

