# ZPL2PDF - Convertisseur ZPL vers PDF

[![Version](https://img.shields.io/badge/version-2.0.0-blue.svg)](https://github.com/brunoleocam/ZPL2PDF/releases)
![GitHub all releases](https://img.shields.io/github/downloads/brunoleocam/ZPL2PDF/total)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![Docker](https://img.shields.io/badge/docker-Alpine%20470MB-blue.svg)](https://hub.docker.com/r/brunoleocam/zpl2pdf)

**[English](../../README.md)** | **[Português-BR](README.pt-BR.md)** | **[Español](README.es-ES.md)** | **[Français](#)**

Un puissant outil en ligne de commande multiplateforme qui convertit les fichiers ZPL (Zebra Programming Language) en documents PDF de haute qualité. Parfait pour les flux de travail d'impression d'étiquettes, la génération automatisée de documents et les systèmes de gestion d'étiquettes d'entreprise.

---

## 🚀 **Nouveautés v2.0**

- 🌍 **Support Multi-langue** - 8 langues (EN, PT, ES, FR, DE, IT, JA, ZH)
- 🔄 **Mode Daemon** - Surveillance automatique des dossiers et conversion par lots
- 🏗️ **Architecture Propre** - Entièrement refactorisé avec principes SOLID
- 🌍 **Multiplateforme** - Support natif pour Windows, Linux et macOS
- 📐 **Dimensions Intelligentes** - Extraction automatique des dimensions ZPL (`^PW`, `^LL`)
- ⚡ **Haute Performance** - Traitement asynchrone avec mécanismes de relance
- 🐳 **Support Docker** - Optimisé pour Alpine Linux (470MB)
- 📦 **Installateur Professionnel** - Installateur Windows avec configuration multi-langue

---

## 📦 **Installation**

### **Windows**

#### Option 1: WinGet (Recommandé)
```powershell
winget install brunoleocam.ZPL2PDF
```

#### Option 2: Installateur
1. Télécharger [ZPL2PDF-Setup-2.0.0.exe](https://github.com/brunoleocam/ZPL2PDF/releases/latest)
2. Exécuter l'installateur
3. Choisir la langue pendant l'installation
4. Terminé! ✅

### **Linux**

#### Ubuntu/Debian (paquet .deb)
```bash
# Télécharger le paquet .deb
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-amd64.deb

# Installer le paquet
sudo dpkg -i ZPL2PDF-v2.0.0-linux-amd64.deb

# Corriger les dépendances si nécessaire
sudo apt-get install -f

# Vérifier l'installation
zpl2pdf --help
```

#### Fedora/CentOS/RHEL (.tar.gz)
```bash
# Télécharger l'archive
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz

# Extraire vers le système
sudo tar -xzf ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz -C /

# Rendre exécutable
sudo chmod +x /usr/bin/ZPL2PDF

# Créer un lien symbolique
sudo ln -s /usr/bin/ZPL2PDF /usr/bin/zpl2pdf

# Vérifier l'installation
zpl2pdf --help
```

### **Docker**

```bash
docker pull brunoleocam/zpl2pdf:latest
docker run -v ./watch:/app/watch -v ./output:/app/output brunoleocam/zpl2pdf:latest
```

---

## 🚀 **Démarrage Rapide**

### **Convertir un Fichier**
```bash
ZPL2PDF -i etiquette.txt -o dossier_sortie -n mon_etiquette.pdf
```

### **Mode Daemon (Auto-Conversion)**
```bash
# Démarrer avec configuration par défaut
ZPL2PDF start

# Démarrer avec dossier personnalisé
ZPL2PDF start -l "C:\Etiquettes" -w 7.5 -h 15 -u in

# Vérifier l'état
ZPL2PDF status

# Arrêter le daemon
ZPL2PDF stop
```

### **Configurer la Langue**
```bash
# Temporaire (session actuelle)
ZPL2PDF --language fr-FR status

# Permanent (toutes les sessions)
ZPL2PDF --set-language fr-FR

# Voir la configuration
ZPL2PDF --show-language
```

---

## 📚 **Documentation**

- 📖 [Documentation Complète](../README.md)
- 🌍 [Configuration Multi-langue](../guides/LANGUAGE_CONFIGURATION.md)
- 🐳 [Guide Docker](../guides/DOCKER_GUIDE.md)
- 🛠️ [Guide de Contribution](../../CONTRIBUTING.md)
- 📋 [Journal des Modifications](../../CHANGELOG.md)

---

## 🤝 **Contribuer**

Nous acceptons les contributions! Voir [CONTRIBUTING.md](../../CONTRIBUTING.md) pour plus de détails.

---

## 📄 **Licence**

Ce projet est sous licence MIT - voir le fichier [LICENSE](../../LICENSE) pour plus de détails.

---

## 👥 **Contributeurs**

Merci à tous les contributeurs qui ont aidé à améliorer ZPL2PDF!

<a href="https://github.com/brunoleocam/ZPL2PDF/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=brunoleocam/ZPL2PDF" />
</a>

---

**ZPL2PDF** - Convertissez les étiquettes ZPL en PDF facilement et efficacement.

