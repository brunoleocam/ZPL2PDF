# ZPL2PDF - Convertidor ZPL a PDF

[![Version](https://img.shields.io/badge/version-3.0.2-blue.svg)](https://github.com/brunoleocam/ZPL2PDF/releases)
![GitHub all releases](https://img.shields.io/github/downloads/brunoleocam/ZPL2PDF/total)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![Docker](https://img.shields.io/badge/docker-Alpine%20470MB-blue.svg)](https://hub.docker.com/r/brunoleocam/zpl2pdf)
[![WinGet](https://img.shields.io/badge/winget-brunoleocam.ZPL2PDF-blue)](https://github.com/microsoft/winget-pkgs/tree/master/manifests/b/brunoleocam/ZPL2PDF)

**[English](../../README.md)** | **[Português-BR](README.pt-BR.md)** | **[Español](#)** | **[Français](README.fr-FR.md)** | **[Deutsch](README.de-DE.md)** | **[Italiano](README.it-IT.md)** | **[日本語](README.ja-JP.md)** | **[中文](README.zh-CN.md)**

Una poderosa herramienta multiplataforma de línea de comandos que convierte archivos ZPL (Zebra Programming Language) en documentos PDF de alta calidad. Perfecta para flujos de trabajo de impresión de etiquetas, generación automatizada de documentos y sistemas empresariales de gestión de etiquetas.

---

## 🚀 **Novedades en v3.1.0**

### 🐛 Correcciones
- **Issue #45**: Corregidas etiquetas duplicadas o en blanco cuando `^XA` aparece dentro del payload base64 de `~DGR:` — `^XA` ahora se trata como inicio de etiqueta solo al inicio de línea o después de `^XZ`.

### ✨ Nuevas Funcionalidades
- **Issue #48 – Servidor TCP**: Modo impresora Zebra virtual implementado. Use `ZPL2PDF server start --port 9101 -o output/`, `server stop` y `server status`.
- **REST API (PR #47)**: Ejecute `ZPL2PDF --api --host localhost --port 5000` para `POST /api/convert` (ZPL a PDF o PNG) y `GET /api/health`. [Guía de la API](../guides/API_GUIDE.md).

---

## 🚀 **Novedades en v3.1.0**

### 🐛 Correcciones
- **Issue #39**: Procesamiento secuencial de gráficos para múltiples gráficos con el mismo nombre
  - Los archivos ZPL con múltiples gráficos `~DGR` ahora se procesan correctamente
  - Cada etiqueta usa el gráfico correcto según el estado secuencial
  - Los comandos de limpieza `^IDR` ya no generan páginas en blanco
  - Resuelve el problema donde todas las etiquetas eran idénticas en archivos de etiquetas Shopee

### 🔧 Mejoras
- Validación de entrada en métodos públicos
- Mejor manejo de excepciones
- Optimizaciones de rendimiento con regex compilado
- Limpieza de código y eliminación de métodos no utilizados

---

## 🚀 **Novedades en v3.1.0**

### 🎉 Principales Nuevas Funcionalidades
- 🎨 **Integración con API Labelary** - Renderizado ZPL de alta fidelidad con salida PDF vectorial
- 🖨️ **Modo Servidor TCP** - Impresora Zebra virtual en puerto TCP (predeterminado: 9101)
- 🔤 **Fuentes Personalizadas** - Cargue fuentes TrueType/OpenType con `--fonts-dir` y `--font`
- 📁 **Soporte Extendido de Archivos** - Añadidas extensiones `.zpl` e `.imp`
- 📝 **Nomenclatura Personalizada** - Defina nombre del archivo de salida vía `^FX FileName:` en ZPL

### 🔧 Opciones de Renderizado
```bash
--renderer offline    # BinaryKits (predeterminado, funciona offline)
--renderer labelary   # API Labelary (alta fidelidad, requiere internet)
--renderer auto       # Intenta Labelary, fallback a BinaryKits
```

### 🖨️ Servidor TCP (Impresora Virtual)
```bash
ZPL2PDF server start --port 9101 -o output/
ZPL2PDF server status
ZPL2PDF server stop
```

### Funcionalidades v2.x (Aún Disponibles)
- 🌍 **Soporte Multi-idioma** - 8 idiomas (EN, PT, ES, FR, DE, IT, JA, ZH)
- 🔄 **Modo Daemon** - Monitoreo automático de carpetas y conversión por lotes
- 🏗️ **Arquitectura Limpia** - Completamente refactorizado con principios SOLID
- 🌍 **Multiplataforma** - Soporte nativo para Windows, Linux y macOS
- 📐 **Dimensiones Inteligentes** - Extracción automática de dimensiones ZPL (`^PW`, `^LL`)
- ⚡ **Alto Rendimiento** - Procesamiento asíncrono con mecanismos de reintento
- 🐳 **Soporte Docker** - Optimizado para Alpine Linux (470MB)
- 📦 **Instalador Profesional** - Instalador Windows con configuración multi-idioma

---

## ✨ **Características Principales**

### 🎯 **Tres Modos de Operación**

#### **Modo Conversión** - Convertir archivos individuales
```bash
ZPL2PDF -i etiqueta.txt -o carpeta_salida/ -n mi_etiqueta.pdf
```

#### **Modo Daemon** - Monitoreo automático de carpetas
```bash
ZPL2PDF start -l "C:\Etiquetas"
```

#### **Modo Servidor TCP** - Impresora virtual
```bash
ZPL2PDF server start --port 9101 -o carpeta_salida/
```

### 📐 **Gestión Inteligente de Dimensiones**

- ✅ Extraer dimensiones de comandos ZPL (`^PW`, `^LL`)
- ✅ Soporte para múltiples unidades (mm, cm, pulgadas, puntos)
- ✅ Fallback automático a valores predeterminados sensatos
- ✅ Resolución de dimensiones basada en prioridad

### 🌍 **Interfaz Multi-idioma**

Configure su idioma preferido:
```bash
# Temporal (sesión actual)
ZPL2PDF --language es-ES status

# Permanente (todas las sesiones)
ZPL2PDF --set-language es-ES

# Ver configuración
ZPL2PDF --show-language
```

---

## 📦 **Instalación**

### **Windows**

#### Opción 1: WinGet (Recomendado)
```powershell
winget install brunoleocam.ZPL2PDF
```

#### Opción 2: Instalador
1. Descargar [ZPL2PDF-Setup-3.0.0.exe](https://github.com/brunoleocam/ZPL2PDF/releases/latest)
2. Ejecutar instalador
3. Elegir idioma durante la instalación
4. ¡Listo! ✅

### **Linux**

#### Ubuntu/Debian (paquete .deb)
```bash
# Descargar paquete .deb
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v3.1.0/ZPL2PDF-v3.1.0-linux-amd64.deb

# Instalar paquete
sudo dpkg -i ZPL2PDF-v3.1.0-linux-amd64.deb

# Corregir dependencias si es necesario
sudo apt-get install -f

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

### **Convertir con Labelary (Alta Fidelidad)**
```bash
ZPL2PDF -i etiqueta.txt -o carpeta_salida --renderer labelary
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

---

## 📖 **Guía de Uso**

### **Parámetros del Modo Conversión**

| Parámetro | Descripción | Ejemplo |
|-----------|-------------|---------|
| `-i <archivo>` | Archivo ZPL de entrada (.txt, .prn, .zpl, .imp) | `-i etiqueta.zpl` |
| `-z <contenido>` | Contenido ZPL como string | `-z "^XA...^XZ"` |
| `-o <carpeta>` | Carpeta de salida para PDF | `-o C:\Salida` |
| `-n <nombre>` | Nombre del archivo PDF de salida | `-n resultado.pdf` |
| `-w <ancho>` | Ancho de la etiqueta | `-w 10` |
| `-h <alto>` | Alto de la etiqueta | `-h 5` |
| `-u <unidad>` | Unidad (mm, cm, in) | `-u cm` |
| `-d <dpi>` | Densidad de impresión (predeterminado: 203) | `-d 300` |
| `--renderer` | Motor de renderizado (offline/labelary/auto) | `--renderer labelary` |
| `--fonts-dir` | Directorio de fuentes personalizadas | `--fonts-dir C:\Fuentes` |
| `--font` | Mapear fuente específica | `--font "A=arial.ttf"` |

### **Comandos del Servidor TCP**

```bash
ZPL2PDF server start [opciones]    # Iniciar servidor TCP (impresora virtual)
ZPL2PDF server stop                # Detener servidor TCP
ZPL2PDF server status              # Verificar estado del servidor TCP
```

---

## 🎨 **Motores de Renderizado**

### **Offline (BinaryKits)** - Predeterminado
- ✅ Funciona sin internet
- ✅ Procesamiento rápido
- ⚠️ Algunos comandos ZPL pueden renderizarse diferente

### **Labelary (API)** - Alta Fidelidad
- ✅ Emulación exacta de impresora Zebra
- ✅ Salida PDF vectorial (archivos más pequeños)
- ✅ Procesamiento automático por lotes para 50+ etiquetas
- ⚠️ Requiere conexión a internet

### **Auto (Fallback)**
- ✅ Intenta Labelary primero
- ✅ Fallback a BinaryKits si está offline

---

## 📐 **Soporte ZPL**

### **Comandos Soportados**

- ✅ `^XA` / `^XZ` - Inicio/fin de etiqueta
- ✅ `^PW<ancho>` - Ancho de impresión en puntos
- ✅ `^LL<largo>` - Largo de la etiqueta en puntos
- ✅ `^FX FileName:` - Nombre personalizado del archivo de salida
- ✅ `^FX !FileName:` - Nombre forzado del archivo (sobrescribe `-n`)
- ✅ Todos los comandos ZPL estándar de texto, gráficos y códigos de barras

---

## 📚 **Documentación**

- 📖 [Documentación Completa](../README.md)
- 🌍 [Configuración Multi-idioma](../LANGUAGE_CONFIGURATION.md)
- 🐳 [Guía Docker](../DOCKER_GUIDE.md)
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
