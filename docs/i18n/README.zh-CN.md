# ZPL2PDF - ZPL转PDF转换器

[![Version](https://img.shields.io/badge/version-2.0.0-blue.svg)](https://github.com/brunoleocam/ZPL2PDF/releases)
![GitHub all releases](https://img.shields.io/github/downloads/brunoleocam/ZPL2PDF/total)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![Docker](https://img.shields.io/badge/docker-Alpine%20470MB-blue.svg)](https://hub.docker.com/r/brunoleocam/zpl2pdf)

**[English](../../README.md)** | **[Português-BR](README.pt-BR.md)** | **[Español](README.es-ES.md)** | **[Français](README.fr-FR.md)** | **[Deutsch](README.de-DE.md)** | **[Italiano](README.it-IT.md)** | **[日本語](README.ja-JP.md)** | **[中文](#)**

一个强大的跨平台命令行工具，可将ZPL（Zebra Programming Language）文件转换为高质量的PDF文档。非常适合标签打印工作流程、自动化文档生成和企业标签管理系统。

---

## 🚀 **v2.0新功能**

- 🌍 **多语言支持** - 8种语言（EN、PT、ES、FR、DE、IT、JA、ZH）
- 🔄 **守护进程模式** - 自动文件夹监控和批量转换
- 🏗️ **清晰架构** - 完全按照SOLID原则重构
- 🌍 **跨平台** - 原生支持Windows、Linux和macOS
- 📐 **智能尺寸** - 自动提取ZPL尺寸（`^PW`、`^LL`）
- ⚡ **高性能** - 具有重试机制的异步处理
- 🐳 **Docker支持** - Alpine Linux优化（470MB）
- 📦 **专业安装程序** - 带多语言设置的Windows安装程序

---

## 📦 **安装**

### **Windows**

#### 选项1：WinGet（推荐）
```powershell
winget install brunoleocam.ZPL2PDF
```

#### 选项2：安装程序
1. 下载 [ZPL2PDF-Setup-2.0.0.exe](https://github.com/brunoleocam/ZPL2PDF/releases/latest)
2. 运行安装程序
3. 在安装过程中选择语言
4. 完成！ ✅

### **Linux**

#### Ubuntu/Debian（.deb包）
```bash
# 下载 .deb 包
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-amd64.deb

# 安装包
sudo dpkg -i ZPL2PDF-v2.0.0-linux-amd64.deb

# 如需修复依赖关系
sudo apt-get install -f

# 验证安装
zpl2pdf --help
```

#### Fedora/CentOS/RHEL（.tar.gz）
```bash
# 下载压缩包
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz

# 解压到系统
sudo tar -xzf ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz -C /

# 设置可执行权限
sudo chmod +x /usr/bin/ZPL2PDF

# 创建符号链接
sudo ln -s /usr/bin/ZPL2PDF /usr/bin/zpl2pdf

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

### **配置语言**
```bash
# 临时（当前会话）
ZPL2PDF --language zh-CN status

# 永久（所有会话）
ZPL2PDF --set-language zh-CN

# 显示配置
ZPL2PDF --show-language
```

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

