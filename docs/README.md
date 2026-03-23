# 📚 ZPL2PDF - Documentation Index

Welcome to the ZPL2PDF documentation! This index helps you find the right documentation for your needs.

---

## 🌍 **Documentation by Language**

### Translated READMEs

- 🇺🇸 [English](../README.md) - Main README (root folder)
- 🇧🇷 [Português (Brasil)](i18n/README.pt-BR.md)
- 🇪🇸 [Español](i18n/README.es-ES.md)
- 🇫🇷 [Français](i18n/README.fr-FR.md)
- 🇩🇪 [Deutsch](i18n/README.de-DE.md)
- 🇮🇹 [Italiano](i18n/README.it-IT.md)
- 🇯🇵 [日本語](i18n/README.ja-JP.md)
- 🇨🇳 [中文 (简体)](i18n/README.zh-CN.md)

**Want to contribute improvements?** See [CONTRIBUTING.md](../CONTRIBUTING.md)

---

## 📖 **User Guides**

Documentation for end-users:

### Getting Started
- 🚀 [Quick Start Guide](../README.md#quick-start) - Get up and running in 5 minutes
- 📦 [Installation Guide](../README.md#installation) - All installation methods

### Usage Guides
- 🐳 [Docker Guide](guides/DOCKER_GUIDE.md) - Complete Docker usage and deployment
- 🧪 [Docker Testing Guide](guides/DOCKER_TESTING.md) - Test ZPL2PDF on all platforms with Docker
- 🌍 [Multi-Language Configuration](guides/LANGUAGE_CONFIGURATION.md) - Set up your preferred language
- 📦 [Windows Installer Guide](guides/INNO_SETUP_GUIDE.md) - Professional Windows installation
- 📦 [Linux Packages Guide](guides/LINUX_PACKAGES.md) - Install on Linux (DEB, RPM)
- 🔌 [REST API Guide](guides/API_GUIDE.md) - REST API usage and examples

### Advanced Topics
- ⚙️ [Configuration Reference](../zpl2pdf.json.example) - All configuration options
- 🔧 [Troubleshooting](../README.md#troubleshooting) - Common issues and solutions
- 📊 [Performance Optimization](../README.md#performance) - Benchmarks and tips

---

## 🛠️ **Developer Documentation**

Documentation for contributors and developers:

### Development Workflow
- 🤝 [Contributing Guide](../CONTRIBUTING.md) - How to contribute
- 📋 [Changelog](../CHANGELOG.md) - Version history
- 🔄 [CI/CD Workflow](development/CI_CD_WORKFLOW.md) - Automated build and deployment

### Build & Deployment
- 🔨 [Build Scripts Documentation](../scripts/README.md) - Build for all platforms
- 🐳 [Docker Publishing Guide](development/DOCKER_PUBLISH_GUIDE.md) - Publish Docker images
- 📦 [Installer Documentation](../installer/README.md) - Windows installer creation
- 📦 [WinGet Guide](development/WINGET_GUIDE.md) - WinGet package submission
- 🔄 [Git Workflow](development/GIT_WORKFLOW_GUIDE.md) - Git workflow and branching strategy

### Internal Documentation
- 🏗️ [Architecture Overview](../wiki/Architecture-Overview.md)
- 📚 [API Documentation](development/API.md) *(Coming soon)*

---

## 🎯 **Quick Links by Task**

### "I want to..."

| Task | Documentation |
|------|--------------|
| **Install ZPL2PDF** | [Installation Guide](../README.md#installation) |
| **Convert a ZPL file** | [Quick Start](../README.md#quick-start) |
| **Set up auto-conversion** | [Daemon Mode Guide](../README.md#daemon-mode) |
| **Use Docker** | [Docker Guide](guides/DOCKER_GUIDE.md) |
| **Use REST API** | [REST API Guide](guides/API_GUIDE.md) |
| **Change language** | [Language Configuration](guides/LANGUAGE_CONFIGURATION.md) |
| **Build from source** | [Development Setup](../CONTRIBUTING.md#development-setup) |
| **Contribute code** | [Contributing Guide](../CONTRIBUTING.md) |
| **Report a bug** | [Bug Reports](../CONTRIBUTING.md#bug-reports) |
| **Request a feature** | [Feature Requests](../CONTRIBUTING.md#feature-requests) |
| **Publish a release** | [CI/CD Workflow](development/CI_CD_WORKFLOW.md) |
| **Test on Linux** | [Docker Testing](guides/DOCKER_TESTING.md) |

---

## 📁 **Documentation Structure**

```
docs/
├── README.md                      (This file - Documentation index)
├── i18n/                          (Internationalization)
│   ├── README.pt-BR.md            (Portuguese)
│   ├── README.es-ES.md            (Spanish)
│   └── README.fr-FR.md            (French)
├── guides/                        (User guides - for end users)
│   ├── DOCKER_GUIDE.md            (Docker usage and deployment)
│   ├── DOCKER_TESTING.md          (Cross-platform testing with Docker)
│   ├── INNO_SETUP_GUIDE.md        (Windows installer guide)
│   ├── LANGUAGE_CONFIGURATION.md  (Multi-language setup)
│   ├── LINUX_PACKAGES.md          (Linux package installation)
│   └── API_GUIDE.md               (REST API usage and examples)
├── development/                   (Developer docs - for contributors)
│   ├── CI_CD_WORKFLOW.md          (CI/CD automation workflow)
│   ├── GIT_WORKFLOW_GUIDE.md      (Git workflow and branching)
│   ├── DOCKER_PUBLISH_GUIDE.md    (Docker image publishing)
│   └── WINGET_GUIDE.md            (WinGet package submission)
├── Image/                         (Screenshots & icons)
│   ├── ZPL2PDF.ico
│   ├── ZPL2PDF.png
│   └── example_*.png
└── Sample/                        (Sample ZPL files)
    ├── example.txt
    ├── test-*.txt
    └── *.prn
```

---

## 🔍 **Search Documentation**

Can't find what you're looking for? Try:

1. **Search the Wiki**: [GitHub Wiki](https://github.com/brunoleocam/ZPL2PDF/wiki)
2. **Browse Issues**: [GitHub Issues](https://github.com/brunoleocam/ZPL2PDF/issues)
3. **Ask in Discussions**: [GitHub Discussions](https://github.com/brunoleocam/ZPL2PDF/discussions)

---

## 🆕 **Recently Updated**

- 📅 **2025-01-30**: Release 3.0.2 — Issue #45 (labels with ~DGR), Issue #48 (TCP Server), PR #47 (REST API)
- 📅 **2025-01-07**: Docker Publishing Guide
- 📅 **2025-01-07**: Multi-language Configuration Guide
- 📅 **2025-01-07**: CI/CD Workflow Documentation
- 📅 **2025-01-07**: Inno Setup Guide

---

## 🤝 **Contributing to Documentation**

Found a typo or want to improve the docs?

1. Fork the repository
2. Edit the documentation
3. Submit a Pull Request

See [Contributing Guide](../CONTRIBUTING.md#documentation) for details.

---

**Need help?** Open an [Issue](https://github.com/brunoleocam/ZPL2PDF/issues) or start a [Discussion](https://github.com/brunoleocam/ZPL2PDF/discussions)!
