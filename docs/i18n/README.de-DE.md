# ZPL2PDF - ZPL zu PDF Konverter

[![Version](https://img.shields.io/badge/version-3.0.2-blue.svg)](https://github.com/brunoleocam/ZPL2PDF/releases)
![GitHub all releases](https://img.shields.io/github/downloads/brunoleocam/ZPL2PDF/total)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![Docker](https://img.shields.io/badge/docker-Alpine%20470MB-blue.svg)](https://hub.docker.com/r/brunoleocam/zpl2pdf)
[![WinGet](https://img.shields.io/badge/winget-brunoleocam.ZPL2PDF-blue)](https://github.com/microsoft/winget-pkgs/tree/master/manifests/b/brunoleocam/ZPL2PDF)

**[English](../../README.md)** | **[Português-BR](README.pt-BR.md)** | **[Español](README.es-ES.md)** | **[Français](README.fr-FR.md)** | **[Deutsch](#)** | **[Italiano](README.it-IT.md)** | **[日本語](README.ja-JP.md)** | **[中文](README.zh-CN.md)**

Ein leistungsstarkes, plattformübergreifendes Befehlszeilenwerkzeug, das ZPL-Dateien (Zebra Programming Language) in hochwertige PDF-Dokumente konvertiert. Perfekt für Etikettendruck-Workflows, automatisierte Dokumentenerstellung und Unternehmens-Etikettenverwaltungssysteme.

---

## 🚀 **Neu in v3.1.0**

### 🐛 Fehlerbehebungen
- **Issue #45**: Doppelte oder leere Etiketten behoben, wenn `^XA` im Base64-Payload von `~DGR:` vorkommt — `^XA` wird nur am Zeilenanfang oder nach `^XZ` als Etikettenanfang gewertet.

### ✨ Neue Funktionen
- **Issue #48 – TCP-Server**: Virtueller Zebra-Druckermodus implementiert. Nutzen Sie `ZPL2PDF server start --port 9101 -o output/`, `server stop` und `server status`.
- **REST API (PR #47)**: Führen Sie `ZPL2PDF --api --host localhost --port 5000` aus für `POST /api/convert` (ZPL zu PDF oder PNG) und `GET /api/health`. [API-Anleitung](../guides/API_GUIDE.md).

---

## 🚀 **Neu in v3.1.0**

### 🐛 Fehlerbehebungen
- **Issue #39**: Sequentielle Grafikverarbeitung für mehrere Grafiken mit gleichem Namen
  - ZPL-Dateien mit mehreren `~DGR`-Grafiken werden jetzt korrekt verarbeitet
  - Jede Etikette verwendet die richtige Grafik basierend auf dem sequentiellen Zustand
  - `^IDR`-Bereinigungsbefehle erzeugen keine leeren Seiten mehr
  - Behebt das Problem, bei dem alle Etiketten in Shopee-Versandetiketten identisch waren

### 🔧 Verbesserungen
- Eingabevalidierung in öffentlichen Methoden
- Verbesserte Ausnahmebehandlung
- Performance-Optimierungen mit kompiliertem Regex
- Codebereinigung und Entfernung ungenutzter Methoden

---

## 🚀 **Neu in v3.1.0**

### 🎉 Wichtige Neue Funktionen
- 🎨 **Labelary API Integration** - Hochauflösendes ZPL-Rendering mit Vektor-PDF-Ausgabe
- 🖨️ **TCP-Server-Modus** - Virtueller Zebra-Drucker auf TCP-Port (Standard: 9101)
- 🔤 **Benutzerdefinierte Schriften** - Laden Sie TrueType/OpenType-Schriften mit `--fonts-dir` und `--font`
- 📁 **Erweiterte Dateiunterstützung** - Erweiterungen `.zpl` und `.imp` hinzugefügt
- 📝 **Benutzerdefinierte Benennung** - Ausgabedateiname über `^FX FileName:` in ZPL festlegen

### 🔧 Rendering-Optionen
```bash
--renderer offline    # BinaryKits (Standard, funktioniert offline)
--renderer labelary   # Labelary API (hohe Qualität, Internet erforderlich)
--renderer auto       # Versucht Labelary, Fallback auf BinaryKits
```

### 🖨️ TCP-Server (Virtueller Drucker)
```bash
ZPL2PDF server start --port 9101 -o output/
ZPL2PDF server status
ZPL2PDF server stop
```

### v2.x Funktionen (Weiterhin Verfügbar)
- 🌍 **Mehrsprachige Unterstützung** - 8 Sprachen (EN, PT, ES, FR, DE, IT, JA, ZH)
- 🔄 **Daemon-Modus** - Automatische Ordnerüberwachung und Stapelkonvertierung
- 🏗️ **Clean Architecture** - Vollständig überarbeitet mit SOLID-Prinzipien
- 🌍 **Plattformübergreifend** - Native Unterstützung für Windows, Linux und macOS
- 📐 **Intelligente Dimensionen** - Automatische ZPL-Dimensionsextraktion (`^PW`, `^LL`)
- ⚡ **Hohe Leistung** - Asynchrone Verarbeitung mit Wiederholungsmechanismen
- 🐳 **Docker-Unterstützung** - Optimiert für Alpine Linux (470MB)
- 📦 **Professioneller Installer** - Windows-Installer mit mehrsprachiger Einrichtung

---

## ✨ **Hauptfunktionen**

### 🎯 **Drei Betriebsmodi**

#### **Konvertierungsmodus** - Einzelne Dateien konvertieren
```bash
ZPL2PDF -i etikett.txt -o ausgabeordner/ -n mein_etikett.pdf
```

#### **Daemon-Modus** - Automatische Ordnerüberwachung
```bash
ZPL2PDF start -l "C:\Etiketten"
```

#### **TCP-Server-Modus** - Virtueller Drucker
```bash
ZPL2PDF server start --port 9101 -o ausgabeordner/
```

### 📐 **Intelligente Dimensionsverwaltung**

- ✅ Dimensionen aus ZPL-Befehlen extrahieren (`^PW`, `^LL`)
- ✅ Unterstützung für mehrere Einheiten (mm, cm, Zoll, Punkte)
- ✅ Automatischer Fallback auf sinnvolle Standardwerte
- ✅ Prioritätsbasierte Dimensionsauflösung

### 🌍 **Mehrsprachige Oberfläche**

Stellen Sie Ihre bevorzugte Sprache ein:
```bash
# Temporär (aktuelle Sitzung)
ZPL2PDF --language de-DE status

# Permanent (alle Sitzungen)
ZPL2PDF --set-language de-DE

# Konfiguration anzeigen
ZPL2PDF --show-language
```

---

## 📦 **Installation**

### **Windows**

#### Option 1: WinGet (Empfohlen)
```powershell
winget install brunoleocam.ZPL2PDF
```

#### Option 2: Installer
1. [ZPL2PDF-Setup-3.0.0.exe](https://github.com/brunoleocam/ZPL2PDF/releases/latest) herunterladen
2. Installer ausführen
3. Sprache während der Installation wählen
4. Fertig! ✅

### **Linux**

#### Ubuntu/Debian (.deb-Paket)
```bash
# .deb-Paket herunterladen
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v3.1.0/ZPL2PDF-v3.1.0-linux-amd64.deb

# Paket installieren
sudo dpkg -i ZPL2PDF-v3.1.0-linux-amd64.deb

# Abhängigkeiten bei Bedarf beheben
sudo apt-get install -f

# Installation überprüfen
zpl2pdf --help
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

### **Mit Labelary Konvertieren (Hohe Qualität)**
```bash
ZPL2PDF -i etikett.txt -o ausgabeordner --renderer labelary
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

---

## 📖 **Benutzerhandbuch**

### **Parameter des Konvertierungsmodus**

| Parameter | Beschreibung | Beispiel |
|-----------|--------------|----------|
| `-i <datei>` | ZPL-Eingabedatei (.txt, .prn, .zpl, .imp) | `-i etikett.zpl` |
| `-z <inhalt>` | ZPL-Inhalt als String | `-z "^XA...^XZ"` |
| `-o <ordner>` | Ausgabeordner für PDF | `-o C:\Ausgabe` |
| `-n <name>` | Name der PDF-Ausgabedatei | `-n ergebnis.pdf` |
| `-w <breite>` | Etikettenbreite | `-w 10` |
| `-h <höhe>` | Etikettenhöhe | `-h 5` |
| `-u <einheit>` | Einheit (mm, cm, in) | `-u cm` |
| `-d <dpi>` | Druckdichte (Standard: 203) | `-d 300` |
| `--renderer` | Rendering-Engine (offline/labelary/auto) | `--renderer labelary` |
| `--fonts-dir` | Verzeichnis für benutzerdefinierte Schriften | `--fonts-dir C:\Schriften` |
| `--font` | Bestimmte Schrift zuordnen | `--font "A=arial.ttf"` |

### **TCP-Server-Befehle**

```bash
ZPL2PDF server start [optionen]    # TCP-Server starten (virtueller Drucker)
ZPL2PDF server stop                # TCP-Server stoppen
ZPL2PDF server status              # TCP-Server-Status prüfen
```

---

## 🎨 **Rendering-Engines**

### **Offline (BinaryKits)** - Standard
- ✅ Funktioniert ohne Internet
- ✅ Schnelle Verarbeitung
- ⚠️ Einige ZPL-Befehle können anders gerendert werden

### **Labelary (API)** - Hohe Qualität
- ✅ Exakte Zebra-Drucker-Emulation
- ✅ Vektor-PDF-Ausgabe (kleinere Dateien)
- ✅ Automatische Stapelverarbeitung für 50+ Etiketten
- ⚠️ Internetverbindung erforderlich

### **Auto (Fallback)**
- ✅ Versucht zuerst Labelary
- ✅ Fallback auf BinaryKits wenn offline

---

## 📐 **ZPL-Unterstützung**

### **Unterstützte Befehle**

- ✅ `^XA` / `^XZ` - Etiketten-Start/Ende
- ✅ `^PW<breite>` - Druckbreite in Punkten
- ✅ `^LL<länge>` - Etikettenlänge in Punkten
- ✅ `^FX FileName:` - Benutzerdefinierter Ausgabedateiname
- ✅ `^FX !FileName:` - Erzwungener Dateiname (überschreibt `-n`)
- ✅ Alle Standard-ZPL-Befehle für Text, Grafiken und Barcodes

---

## 📚 **Dokumentation**

- 📖 [Vollständige Dokumentation](../README.md)
- 🌍 [Mehrsprachige Konfiguration](../LANGUAGE_CONFIGURATION.md)
- 🐳 [Docker-Anleitung](../DOCKER_GUIDE.md)
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
