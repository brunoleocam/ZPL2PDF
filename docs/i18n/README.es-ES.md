# ZPL2PDF - Convertidor ZPL a PDF

[![Version](https://img.shields.io/badge/version-2.0.0-blue.svg)](https://github.com/brunoleocam/ZPL2PDF/releases)
![GitHub all releases](https://img.shields.io/github/downloads/brunoleocam/ZPL2PDF/total)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![Docker](https://img.shields.io/badge/docker-Alpine%20470MB-blue.svg)](https://hub.docker.com/r/brunoleocam/zpl2pdf)

**[English](../../README.md)** | **[Português-BR](README.pt-BR.md)** | **[Español](#)** | **[Français](README.fr-FR.md)**

Una poderosa herramienta multiplataforma de línea de comandos que convierte archivos ZPL (Zebra Programming Language) en documentos PDF de alta calidad. Perfecta para flujos de trabajo de impresión de etiquetas, generación automatizada de documentos y sistemas empresariales de gestión de etiquetas.

---

## 🚀 **Novedades en v2.0**

- 🌍 **Soporte Multi-idioma** - 8 idiomas (EN, PT, ES, FR, DE, IT, JA, ZH)
- 🔄 **Modo Daemon** - Monitoreo automático de carpetas y conversión por lotes
- 🏗️ **Arquitectura Limpia** - Completamente refactorizado con principios SOLID
- 🌍 **Multiplataforma** - Soporte nativo para Windows, Linux y macOS
- 📐 **Dimensiones Inteligentes** - Extracción automática de dimensiones ZPL (`^PW`, `^LL`)
- ⚡ **Alto Rendimiento** - Procesamiento asíncrono con mecanismos de reintento
- 🐳 **Soporte Docker** - Optimizado para Alpine Linux (470MB)
- 📦 **Instalador Profesional** - Instalador Windows con configuración multi-idioma

---

## 📦 **Instalación**

### **Windows**

#### Opción 1: WinGet (Recomendado)
```powershell
winget install brunoleocam.ZPL2PDF
```

#### Opción 2: Instalador
1. Descargar [ZPL2PDF-Setup-2.0.0.exe](https://github.com/brunoleocam/ZPL2PDF/releases/latest)
2. Ejecutar instalador
3. Elegir idioma durante la instalación
4. ¡Listo! ✅

### **Linux**

#### Ubuntu/Debian (paquete .deb)
```bash
# Descargar paquete .deb
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-amd64.deb

# Instalar paquete
sudo dpkg -i ZPL2PDF-v2.0.0-linux-amd64.deb

# Corregir dependencias si es necesario
sudo apt-get install -f

# Verificar instalación
zpl2pdf --help
```

#### Fedora/CentOS/RHEL (.tar.gz)
```bash
# Descargar tarball
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz

# Extraer al sistema
sudo tar -xzf ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz -C /

# Hacer ejecutable
sudo chmod +x /usr/bin/ZPL2PDF

# Crear enlace simbólico
sudo ln -s /usr/bin/ZPL2PDF /usr/bin/zpl2pdf

# Verificar instalación
zpl2pdf --help
```

### **Docker**

```bash
docker pull brunoleocam/zpl2pdf:latest
docker run -v ./watch:/app/watch -v ./output:/app/output brunoleocam/zpl2pdf:latest
```

---

## 🚀 **Inicio Rápido**

### **Convertir un Archivo**
```bash
ZPL2PDF -i etiqueta.txt -o carpeta_salida -n mi_etiqueta.pdf
```

### **Modo Daemon (Auto-Conversión)**
```bash
# Iniciar con configuración predeterminada
ZPL2PDF start

# Iniciar con carpeta personalizada
ZPL2PDF start -l "C:\Etiquetas" -w 7.5 -h 15 -u in

# Verificar estado
ZPL2PDF status

# Detener daemon
ZPL2PDF stop
```

### **Configurar Idioma**
```bash
# Temporal (sesión actual)
ZPL2PDF --language es-ES status

# Permanente (todas las sesiones)
ZPL2PDF --set-language es-ES

# Ver configuración
ZPL2PDF --show-language
```

---

## 📚 **Documentación**

- 📖 [Documentación Completa](../README.md)
- 🌍 [Configuración Multi-idioma](../guides/LANGUAGE_CONFIGURATION.md)
- 🐳 [Guía Docker](../guides/DOCKER_GUIDE.md)
- 🛠️ [Guía de Contribución](../../CONTRIBUTING.md)
- 📋 [Registro de Cambios](../../CHANGELOG.md)

---

## 🤝 **Contribuir**

¡Aceptamos contribuciones! Ver [CONTRIBUTING.md](../../CONTRIBUTING.md) para más detalles.

---

## 📄 **Licencia**

Este proyecto está licenciado bajo la Licencia MIT - ver el archivo [LICENSE](../../LICENSE) para más detalles.

---

## 👥 **Contribuidores**

Gracias a todos los contribuidores que han ayudado a mejorar ZPL2PDF!

<a href="https://github.com/brunoleocam/ZPL2PDF/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=brunoleocam/ZPL2PDF" />
</a>

---

**ZPL2PDF** - Convierta etiquetas ZPL a PDF fácil y eficientemente.

