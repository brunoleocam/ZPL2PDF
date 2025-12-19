# ZPL2PDF - Convertisseur ZPL vers PDF

[![Version](https://img.shields.io/badge/version-3.0.0-blue.svg)](https://github.com/brunoleocam/ZPL2PDF/releases)
![GitHub all releases](https://img.shields.io/github/downloads/brunoleocam/ZPL2PDF/total)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![Docker](https://img.shields.io/badge/docker-Alpine%20470MB-blue.svg)](https://hub.docker.com/r/brunoleocam/zpl2pdf)
[![WinGet](https://img.shields.io/badge/winget-brunoleocam.ZPL2PDF-blue)](https://github.com/microsoft/winget-pkgs/tree/master/manifests/b/brunoleocam/ZPL2PDF)

**[English](../../README.md)** | **[Português-BR](README.pt-BR.md)** | **[Español](README.es-ES.md)** | **[Français](#)** | **[Deutsch](README.de-DE.md)** | **[Italiano](README.it-IT.md)** | **[日本語](README.ja-JP.md)** | **[中文](README.zh-CN.md)**

Un puissant outil en ligne de commande multiplateforme qui convertit les fichiers ZPL (Zebra Programming Language) en documents PDF de haute qualité. Parfait pour les flux de travail d'impression d'étiquettes, la génération automatisée de documents et les systèmes de gestion d'étiquettes d'entreprise.

---

## 🚀 **Nouveautés v3.0.0**

### 🎉 Principales Nouvelles Fonctionnalités
- 🎨 **Intégration API Labelary** - Rendu ZPL haute fidélité avec sortie PDF vectorielle
- 🖨️ **Mode Serveur TCP** - Imprimante Zebra virtuelle sur port TCP (par défaut: 9101)
- 🔤 **Polices Personnalisées** - Chargez des polices TrueType/OpenType avec `--fonts-dir` et `--font`
- 📁 **Support Étendu de Fichiers** - Ajout des extensions `.zpl` et `.imp`
- 📝 **Nommage Personnalisé** - Définissez le nom du fichier de sortie via `^FX FileName:` dans ZPL

### 🔧 Options de Rendu
```bash
--renderer offline    # BinaryKits (par défaut, fonctionne hors ligne)
--renderer labelary   # API Labelary (haute fidélité, nécessite internet)
--renderer auto       # Essaie Labelary, repli sur BinaryKits
```

### 🖨️ Serveur TCP (Imprimante Virtuelle)
```bash
ZPL2PDF server start --port 9101 -o output/
ZPL2PDF server status
ZPL2PDF server stop
```

### Fonctionnalités v2.x (Toujours Disponibles)
- 🌍 **Support Multi-langue** - 8 langues (EN, PT, ES, FR, DE, IT, JA, ZH)
- 🔄 **Mode Daemon** - Surveillance automatique des dossiers et conversion par lots
- 🏗️ **Architecture Propre** - Entièrement refactorisé avec principes SOLID
- 🌍 **Multiplateforme** - Support natif pour Windows, Linux et macOS
- 📐 **Dimensions Intelligentes** - Extraction automatique des dimensions ZPL (`^PW`, `^LL`)
- ⚡ **Haute Performance** - Traitement asynchrone avec mécanismes de relance
- 🐳 **Support Docker** - Optimisé pour Alpine Linux (470MB)
- 📦 **Installateur Professionnel** - Installateur Windows avec configuration multi-langue

---

## ✨ **Fonctionnalités Principales**

### 🎯 **Trois Modes d'Opération**

#### **Mode Conversion** - Convertir des fichiers individuels
```bash
ZPL2PDF -i etiquette.txt -o dossier_sortie/ -n mon_etiquette.pdf
```

#### **Mode Daemon** - Surveillance automatique des dossiers
```bash
ZPL2PDF start -l "C:\Etiquettes"
```

#### **Mode Serveur TCP** - Imprimante virtuelle
```bash
ZPL2PDF server start --port 9101 -o dossier_sortie/
```

### 📐 **Gestion Intelligente des Dimensions**

- ✅ Extraire les dimensions des commandes ZPL (`^PW`, `^LL`)
- ✅ Support pour plusieurs unités (mm, cm, pouces, points)
- ✅ Repli automatique vers des valeurs par défaut sensées
- ✅ Résolution des dimensions basée sur la priorité

### 🌍 **Interface Multi-langue**

Configurez votre langue préférée:
```bash
# Temporaire (session actuelle)
ZPL2PDF --language fr-FR status

# Permanent (toutes les sessions)
ZPL2PDF --set-language fr-FR

# Voir la configuration
ZPL2PDF --show-language
```

---

## 📦 **Installation**

### **Windows**

#### Option 1: WinGet (Recommandé)
```powershell
winget install brunoleocam.ZPL2PDF
```

#### Option 2: Installateur
1. Télécharger [ZPL2PDF-Setup-3.0.0.exe](https://github.com/brunoleocam/ZPL2PDF/releases/latest)
2. Exécuter l'installateur
3. Choisir la langue pendant l'installation
4. Terminé! ✅

### **Linux**

#### Ubuntu/Debian (paquet .deb)
```bash
# Télécharger le paquet .deb
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v3.0.0/ZPL2PDF-v3.0.0-linux-amd64.deb

# Installer le paquet
sudo dpkg -i ZPL2PDF-v3.0.0-linux-amd64.deb

# Corriger les dépendances si nécessaire
sudo apt-get install -f

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

### **Convertir avec Labelary (Haute Fidélité)**
```bash
ZPL2PDF -i etiquette.txt -o dossier_sortie --renderer labelary
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

---

## 📖 **Guide d'Utilisation**

### **Paramètres du Mode Conversion**

| Paramètre | Description | Exemple |
|-----------|-------------|---------|
| `-i <fichier>` | Fichier ZPL d'entrée (.txt, .prn, .zpl, .imp) | `-i etiquette.zpl` |
| `-z <contenu>` | Contenu ZPL en chaîne | `-z "^XA...^XZ"` |
| `-o <dossier>` | Dossier de sortie pour PDF | `-o C:\Sortie` |
| `-n <nom>` | Nom du fichier PDF de sortie | `-n resultat.pdf` |
| `-w <largeur>` | Largeur de l'étiquette | `-w 10` |
| `-h <hauteur>` | Hauteur de l'étiquette | `-h 5` |
| `-u <unité>` | Unité (mm, cm, in) | `-u cm` |
| `-d <dpi>` | Densité d'impression (par défaut: 203) | `-d 300` |
| `--renderer` | Moteur de rendu (offline/labelary/auto) | `--renderer labelary` |
| `--fonts-dir` | Répertoire de polices personnalisées | `--fonts-dir C:\Polices` |
| `--font` | Mapper une police spécifique | `--font "A=arial.ttf"` |

### **Commandes du Serveur TCP**

```bash
ZPL2PDF server start [options]    # Démarrer le serveur TCP (imprimante virtuelle)
ZPL2PDF server stop               # Arrêter le serveur TCP
ZPL2PDF server status             # Vérifier l'état du serveur TCP
```

---

## 🎨 **Moteurs de Rendu**

### **Offline (BinaryKits)** - Par Défaut
- ✅ Fonctionne sans internet
- ✅ Traitement rapide
- ⚠️ Certaines commandes ZPL peuvent être rendues différemment

### **Labelary (API)** - Haute Fidélité
- ✅ Émulation exacte d'imprimante Zebra
- ✅ Sortie PDF vectorielle (fichiers plus petits)
- ✅ Traitement automatique par lots pour 50+ étiquettes
- ⚠️ Nécessite une connexion internet

### **Auto (Repli)**
- ✅ Essaie Labelary d'abord
- ✅ Repli sur BinaryKits si hors ligne

---

## 📐 **Support ZPL**

### **Commandes Supportées**

- ✅ `^XA` / `^XZ` - Début/fin d'étiquette
- ✅ `^PW<largeur>` - Largeur d'impression en points
- ✅ `^LL<longueur>` - Longueur de l'étiquette en points
- ✅ `^FX FileName:` - Nom personnalisé du fichier de sortie
- ✅ `^FX !FileName:` - Nom forcé du fichier (remplace `-n`)
- ✅ Toutes les commandes ZPL standard pour texte, graphiques et codes-barres

---

## 📚 **Documentation**

- 📖 [Documentation Complète](../README.md)
- 🌍 [Configuration Multi-langue](../LANGUAGE_CONFIGURATION.md)
- 🐳 [Guide Docker](../DOCKER_GUIDE.md)
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