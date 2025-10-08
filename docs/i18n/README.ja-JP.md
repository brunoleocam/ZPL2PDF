# ZPL2PDF - ZPLからPDFへのコンバーター

[![Version](https://img.shields.io/badge/version-2.0.0-blue.svg)](https://github.com/brunoleocam/ZPL2PDF/releases)
![GitHub all releases](https://img.shields.io/github/downloads/brunoleocam/ZPL2PDF/total)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![Docker](https://img.shields.io/badge/docker-Alpine%20470MB-blue.svg)](https://hub.docker.com/r/brunoleocam/zpl2pdf)

**[English](../../README.md)** | **[Português-BR](README.pt-BR.md)** | **[Español](README.es-ES.md)** | **[Français](README.fr-FR.md)** | **[Deutsch](README.de-DE.md)** | **[Italiano](README.it-IT.md)** | **[日本語](#)**

ZPL（Zebra Programming Language）ファイルを高品質なPDFドキュメントに変換する強力なクロスプラットフォームコマンドラインツール。ラベル印刷ワークフロー、自動ドキュメント生成、エンタープライズラベル管理システムに最適です。

---

## 🚀 **v2.0の新機能**

- 🌍 **多言語サポート** - 8言語（EN、PT、ES、FR、DE、IT、JA、ZH）
- 🔄 **デーモンモード** - 自動フォルダー監視とバッチ変換
- 🏗️ **クリーンアーキテクチャ** - SOLID原則で完全にリファクタリング
- 🌍 **クロスプラットフォーム** - Windows、Linux、macOSのネイティブサポート
- 📐 **スマート寸法** - ZPL寸法の自動抽出（`^PW`、`^LL`）
- ⚡ **高パフォーマンス** - リトライメカニズムを備えた非同期処理
- 🐳 **Dockerサポート** - Alpine Linux最適化（470MB）
- 📦 **プロフェッショナルインストーラー** - 多言語セットアップ付きWindowsインストーラー

---

## 📦 **インストール**

### **Windows**

#### オプション1：WinGet（推奨）
```powershell
winget install brunoleocam.ZPL2PDF
```

#### オプション2：インストーラー
1. [ZPL2PDF-Setup-2.0.0.exe](https://github.com/brunoleocam/ZPL2PDF/releases/latest)をダウンロード
2. インストーラーを実行
3. インストール中に言語を選択
4. 完了！ ✅

### **Linux**

#### Ubuntu/Debian（.debパッケージ）
```bash
# .debパッケージをダウンロード
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-amd64.deb

# パッケージをインストール
sudo dpkg -i ZPL2PDF-v2.0.0-linux-amd64.deb

# 必要に応じて依存関係を修正
sudo apt-get install -f

# インストールを確認
zpl2pdf --help
```

#### Fedora/CentOS/RHEL（.tar.gz）
```bash
# tarballをダウンロード
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz

# システムに展開
sudo tar -xzf ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz -C /

# 実行可能にする
sudo chmod +x /usr/bin/ZPL2PDF

# シンボリックリンクを作成
sudo ln -s /usr/bin/ZPL2PDF /usr/bin/zpl2pdf

# インストールを確認
zpl2pdf --help
```

### **Docker**

```bash
docker pull brunoleocam/zpl2pdf:latest
docker run -v ./watch:/app/watch -v ./output:/app/output brunoleocam/zpl2pdf:latest
```

---

## 🚀 **クイックスタート**

### **ファイルを変換**
```bash
ZPL2PDF -i label.txt -o output_folder -n my_label.pdf
```

### **デーモンモード（自動変換）**
```bash
# デフォルト設定で開始
ZPL2PDF start

# カスタムフォルダーで開始
ZPL2PDF start -l "C:\Labels" -w 7.5 -h 15 -u in

# ステータスを確認
ZPL2PDF status

# デーモンを停止
ZPL2PDF stop
```

### **言語を設定**
```bash
# 一時的（現在のセッション）
ZPL2PDF --language ja-JP status

# 永続的（すべてのセッション）
ZPL2PDF --set-language ja-JP

# 設定を表示
ZPL2PDF --show-language
```

---

## 📚 **ドキュメント**

- 📖 [完全なドキュメント](../README.md)
- 🌍 [多言語設定](../guides/LANGUAGE_CONFIGURATION.md)
- 🐳 [Dockerガイド](../guides/DOCKER_GUIDE.md)
- 🛠️ [貢献ガイド](../../CONTRIBUTING.md)
- 📋 [変更履歴](../../CHANGELOG.md)

---

## 🤝 **貢献**

貢献を歓迎します！詳細は[CONTRIBUTING.md](../../CONTRIBUTING.md)をご覧ください。

---

## 📄 **ライセンス**

このプロジェクトはMITライセンスの下でライセンスされています - 詳細は[LICENSE](../../LICENSE)ファイルを参照してください。

---

## 👥 **貢献者**

ZPL2PDFの改善に協力してくださったすべての貢献者に感謝します！

<a href="https://github.com/brunoleocam/ZPL2PDF/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=brunoleocam/ZPL2PDF" />
</a>

---

**ZPL2PDF** - ZPLラベルを簡単かつ効率的にPDFに変換。

