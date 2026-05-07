# ZPL2PDF - ZPL转PDF转换器

[![Version](https://img.shields.io/badge/version-3.0.2-blue.svg)](https://github.com/brunoleocam/ZPL2PDF/releases)
![GitHub all releases](https://img.shields.io/github/downloads/brunoleocam/ZPL2PDF/total)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![Docker](https://img.shields.io/badge/docker-Alpine%20470MB-blue.svg)](https://hub.docker.com/r/brunoleocam/zpl2pdf)
[![WinGet](https://img.shields.io/badge/winget-brunoleocam.ZPL2PDF-blue)](https://github.com/microsoft/winget-pkgs/tree/master/manifests/b/brunoleocam/ZPL2PDF)

**[English](../../README.md)** | **[Português-BR](README.pt-BR.md)** | **[Español](README.es-ES.md)** | **[Français](README.fr-FR.md)** | **[Deutsch](README.de-DE.md)** | **[Italiano](README.it-IT.md)** | **[日本語](README.ja-JP.md)** | **[中文](#)**

一个强大的跨平台命令行工具，可将ZPL（Zebra Programming Language）文件转换为高质量的PDF文档。非常适合标签打印工作流程、自动化文档生成和企业标签管理系统。

---

## 🚀 **v3.1.2新功能**

### 🐛 错误修复
- **Issue #45**：当 `^XA` 出现在 `~DGR:` 的 base64 有效负载内时，修复重复或空白标签 — `^XA` 仅在行首或 `^XZ` 之后被视为标签开始。

### ✨ 新功能
- **Issue #48 – TCP 服务器**：已实现虚拟 Zebra 打印机模式。使用 `ZPL2PDF server start --port 9101 -o output/`、`server stop` 和 `server status`。
- **REST API (PR #47)**：运行 `ZPL2PDF --api --host localhost --port 5000` 提供 `POST /api/convert`（ZPL 转 PDF 或 PNG）和 `GET /api/health`。[API 指南](../guides/API_GUIDE.md)。

---

## 🚀 **v3.1.2新功能**

### 🐛 错误修复
- **Issue #39**：同名多个图形的顺序处理
  - 包含多个 `~DGR` 图形的 ZPL 文件现已正确处理
  - 每个标签根据顺序状态使用正确的图形
  - `^IDR` 清理命令不再生成空白页
  - 解决 Shopee 运输标签文件中所有标签相同的问题

### 🔧 改进
- 在公共方法中添加输入验证
- 改进异常处理
- 使用编译正则表达式进行性能优化
- 代码清理和移除未使用方法

---

## 🚀 **v3.1.2新功能**

### 🎉 主要新功能
- 🎨 **Labelary API集成** - 高保真ZPL渲染，矢量PDF输出
- 🖨️ **TCP服务器模式** - TCP端口上的虚拟Zebra打印机（默认：9101）
- 🔤 **自定义字体** - 使用`--fonts-dir`和`--font`加载TrueType/OpenType字体
- 📁 **扩展文件支持** - 添加`.zpl`和`.imp`扩展名
- 📝 **自定义命名** - 通过ZPL中的`^FX FileName:`设置输出文件名

### 🔧 渲染选项
```bash
--renderer offline    # BinaryKits（默认，离线工作）
--renderer labelary   # Labelary API（高保真，需要互联网）
--renderer auto       # 尝试Labelary，回退到BinaryKits
```

### 🖨️ TCP服务器（虚拟打印机）
```bash
ZPL2PDF server start --port 9101 -o output/
ZPL2PDF server status
ZPL2PDF server stop
```

### v2.x功能（仍然可用）
- 🌍 **多语言支持** - 8种语言（EN、PT、ES、FR、DE、IT、JA、ZH）
- 🔄 **守护进程模式** - 自动文件夹监控和批量转换
- 🏗️ **清晰架构** - 完全按照SOLID原则重构
- 🌍 **跨平台** - 原生支持Windows、Linux和macOS
- 📐 **智能尺寸** - 自动提取ZPL尺寸（`^PW`、`^LL`）
- ⚡ **高性能** - 具有重试机制的异步处理
- 🐳 **Docker支持** - Alpine Linux优化（470MB）
- 📦 **专业安装程序** - 带多语言设置的Windows安装程序

---

## ✨ **主要功能**

### 🎯 **三种操作模式**

#### **转换模式** - 转换单个文件
```bash
ZPL2PDF -i label.txt -o output_folder/ -n my_label.pdf
```

#### **守护进程模式** - 自动文件夹监控
```bash
ZPL2PDF start -l "C:\Labels"
```

#### **TCP服务器模式** - 虚拟打印机
```bash
ZPL2PDF server start --port 9101 -o output_folder/
```

### 📐 **智能尺寸管理**

- ✅ 从ZPL命令提取尺寸（`^PW`、`^LL`）
- ✅ 支持多种单位（mm、cm、英寸、点）
- ✅ 自动回退到合理的默认值
- ✅ 基于优先级的尺寸解析

### 🌍 **多语言界面**

设置您的首选语言：
```bash
# 临时（当前会话）
ZPL2PDF --language zh-CN status

# 永久（所有会话）
ZPL2PDF --set-language zh-CN

# 显示配置
ZPL2PDF --show-language
```

---

## 📦 **安装**

### **Windows**

#### 选项1：WinGet（推荐）
```powershell
winget install brunoleocam.ZPL2PDF
```

#### 选项2：安装程序
1. 下载 [ZPL2PDF-Setup-3.0.0.exe](https://github.com/brunoleocam/ZPL2PDF/releases/latest)
2. 运行安装程序
3. 在安装过程中选择语言
4. 完成！ ✅

### **Linux**

#### Ubuntu/Debian（.deb包）
```bash
# 下载 .deb 包
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v3.1.2/ZPL2PDF-v3.1.2-linux-amd64.deb

# 安装包
sudo dpkg -i ZPL2PDF-v3.1.2-linux-amd64.deb

# 如需修复依赖关系
sudo apt-get install -f

# 验证安装
zpl2pdf --help
```

### **Docker**

```bash
docker pull brunoleocam/zpl2pdf:latest
docker run -v ./watch:/app/watch -v ./output:/app/output brunoleocam/zpl2pdf:latest
```

---

## 🚀 **快速开始**

### **转换文件**
```bash
ZPL2PDF -i label.txt -o output_folder -n my_label.pdf
```

### **使用Labelary转换（高保真）**
```bash
ZPL2PDF -i label.txt -o output_folder --renderer labelary
```

### **守护进程模式（自动转换）**
```bash
# 使用默认设置启动
ZPL2PDF start

# 使用自定义文件夹启动
ZPL2PDF start -l "C:\Labels" -w 7.5 -h 15 -u in

# 检查状态
ZPL2PDF status

# 停止守护进程
ZPL2PDF stop
```

---

## 📖 **使用指南**

### **转换模式参数**

| 参数 | 描述 | 示例 |
|------|------|------|
| `-i <文件>` | 输入ZPL文件（.txt、.prn、.zpl、.imp） | `-i label.zpl` |
| `-z <内容>` | ZPL内容字符串 | `-z "^XA...^XZ"` |
| `-o <文件夹>` | PDF输出文件夹 | `-o C:\Output` |
| `-n <名称>` | 输出PDF文件名 | `-n result.pdf` |
| `-w <宽度>` | 标签宽度 | `-w 10` |
| `-h <高度>` | 标签高度 | `-h 5` |
| `-u <单位>` | 单位（mm、cm、in） | `-u cm` |
| `-d <dpi>` | 打印密度（默认：203） | `-d 300` |
| `--renderer` | 渲染引擎（offline/labelary/auto） | `--renderer labelary` |
| `--fonts-dir` | 自定义字体目录 | `--fonts-dir C:\Fonts` |
| `--font` | 映射特定字体 | `--font "A=arial.ttf"` |

### **TCP服务器命令**

```bash
ZPL2PDF server start [选项]    # 启动TCP服务器（虚拟打印机）
ZPL2PDF server stop            # 停止TCP服务器
ZPL2PDF server status          # 检查TCP服务器状态
```

---

## 🎨 **渲染引擎**

### **离线（BinaryKits）** - 默认
- ✅ 无需互联网即可工作
- ✅ 快速处理
- ⚠️ 某些ZPL命令可能渲染不同

### **Labelary（API）** - 高保真
- ✅ 精确的Zebra打印机仿真
- ✅ 矢量PDF输出（更小的文件）
- ✅ 50+标签的自动批处理
- ⚠️ 需要互联网连接

### **自动（回退）**
- ✅ 首先尝试Labelary
- ✅ 离线时回退到BinaryKits

---

## 📐 **ZPL支持**

### **支持的命令**

- ✅ `^XA` / `^XZ` - 标签开始/结束
- ✅ `^PW<宽度>` - 点单位的打印宽度
- ✅ `^LL<长度>` - 点单位的标签长度
- ✅ `^FX FileName:` - 自定义输出文件名
- ✅ `^FX !FileName:` - 强制文件名（覆盖`-n`）
- ✅ 所有标准ZPL文本、图形和条码命令

---

## 📚 **文档**

- 📖 [完整文档](../README.md)
- 🌍 [多语言配置](../guides/LANGUAGE_CONFIGURATION.md)
- 🐳 [Docker指南](../guides/DOCKER_GUIDE.md)
- 🛠️ [贡献指南](../../CONTRIBUTING.md)
- 📋 [更新日志](../../CHANGELOG.md)

---

## 🤝 **贡献**

我们欢迎贡献！详情请参阅[CONTRIBUTING.md](../../CONTRIBUTING.md)。

---

## 📄 **许可证**

本项目采用MIT许可证 - 详情请参阅[LICENSE](../../LICENSE)文件。

---

## 👥 **贡献者**

感谢所有帮助改进ZPL2PDF的贡献者！

<a href="https://github.com/brunoleocam/ZPL2PDF/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=brunoleocam/ZPL2PDF" />
</a>

---

**ZPL2PDF** - 轻松高效地将ZPL标签转换为PDF。
