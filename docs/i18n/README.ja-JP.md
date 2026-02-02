# ZPL2PDF - ZPLからPDFへのコンバーター

[![Version](https://img.shields.io/badge/version-3.0.2-blue.svg)](https://github.com/brunoleocam/ZPL2PDF/releases)
![GitHub all releases](https://img.shields.io/github/downloads/brunoleocam/ZPL2PDF/total)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![Docker](https://img.shields.io/badge/docker-Alpine%20470MB-blue.svg)](https://hub.docker.com/r/brunoleocam/zpl2pdf)
[![WinGet](https://img.shields.io/badge/winget-brunoleocam.ZPL2PDF-blue)](https://github.com/microsoft/winget-pkgs/tree/master/manifests/b/brunoleocam/ZPL2PDF)

**[English](../../README.md)** | **[Português-BR](README.pt-BR.md)** | **[Español](README.es-ES.md)** | **[Français](README.fr-FR.md)** | **[Deutsch](README.de-DE.md)** | **[Italiano](README.it-IT.md)** | **[日本語](#)** | **[中文](README.zh-CN.md)**

ZPL（Zebra Programming Language）ファイルを高品質なPDFドキュメントに変換する強力なクロスプラットフォームコマンドラインツール。ラベル印刷ワークフロー、自動ドキュメント生成、エンタープライズラベル管理システムに最適です。

---

## 🚀 **v3.0.0の新機能**

### 🎉 主要な新機能
- 🎨 **Labelary API統合** - ベクターPDF出力による高忠実度ZPLレンダリング
- 🖨️ **TCPサーバーモード** - TCPポート上の仮想Zebraプリンター（デフォルト：9101）
- 🔤 **カスタムフォント** - `--fonts-dir`と`--font`でTrueType/OpenTypeフォントをロード
- 📁 **拡張ファイルサポート** - `.zpl`と`.imp`拡張子を追加
- 📝 **カスタム命名** - ZPL内の`^FX FileName:`で出力ファイル名を設定

### 🔧 レンダリングオプション
```bash
--renderer offline    # BinaryKits（デフォルト、オフラインで動作）
--renderer labelary   # Labelary API（高忠実度、インターネット必要）
--renderer auto       # Labelaryを試行、BinaryKitsにフォールバック
```

### 🖨️ TCPサーバー（仮想プリンター）
```bash
ZPL2PDF server start --port 9101 -o output/
ZPL2PDF server status
ZPL2PDF server stop
```

### v2.x機能（引き続き利用可能）
- 🌍 **多言語サポート** - 8言語（EN、PT、ES、FR、DE、IT、JA、ZH）
- 🔄 **デーモンモード** - 自動フォルダー監視とバッチ変換
- 🏗️ **クリーンアーキテクチャ** - SOLID原則で完全にリファクタリング
- 🌍 **クロスプラットフォーム** - Windows、Linux、macOSのネイティブサポート
- 📐 **スマート寸法** - ZPL寸法の自動抽出（`^PW`、`^LL`）
- ⚡ **高パフォーマンス** - リトライメカニズムを備えた非同期処理
- 🐳 **Dockerサポート** - Alpine Linux最適化（470MB）
- 📦 **プロフェッショナルインストーラー** - 多言語セットアップ付きWindowsインストーラー

---

## ✨ **主な機能**

### 🎯 **3つの操作モード**

#### **変換モード** - 個別ファイルを変換
```bash
ZPL2PDF -i label.txt -o output_folder/ -n my_label.pdf
```

#### **デーモンモード** - 自動フォルダー監視
```bash
ZPL2PDF start -l "C:\Labels"
```

#### **TCPサーバーモード** - 仮想プリンター
```bash
ZPL2PDF server start --port 9101 -o output_folder/
```

### 📐 **インテリジェントな寸法管理**

- ✅ ZPLコマンドから寸法を抽出（`^PW`、`^LL`）
- ✅ 複数の単位をサポート（mm、cm、インチ、ポイント）
- ✅ 適切なデフォルト値への自動フォールバック
- ✅ 優先度ベースの寸法解決

### 🌍 **多言語インターフェース**

お好みの言語を設定:
```bash
# 一時的（現在のセッション）
ZPL2PDF --language ja-JP status

# 永続的（すべてのセッション）
ZPL2PDF --set-language ja-JP

# 設定を表示
ZPL2PDF --show-language
```

---

## 📦 **インストール**

### **Windows**

#### オプション1：WinGet（推奨）
```powershell
winget install brunoleocam.ZPL2PDF
```

#### オプション2：インストーラー
1. [ZPL2PDF-Setup-3.0.0.exe](https://github.com/brunoleocam/ZPL2PDF/releases/latest)をダウンロード
2. インストーラーを実行
3. インストール中に言語を選択
4. 完了！ ✅

### **Linux**

#### Ubuntu/Debian（.debパッケージ）
```bash
# .debパッケージをダウンロード
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v3.0.0/ZPL2PDF-v3.0.0-linux-amd64.deb

# パッケージをインストール
sudo dpkg -i ZPL2PDF-v3.0.0-linux-amd64.deb

# 必要に応じて依存関係を修正
sudo apt-get install -f

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

### **Labelaryで変換（高忠実度）**
```bash
ZPL2PDF -i label.txt -o output_folder --renderer labelary
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

---

## 📖 **使用ガイド**

### **変換モードのパラメーター**

| パラメーター | 説明 | 例 |
|-------------|------|-----|
| `-i <ファイル>` | 入力ZPLファイル（.txt、.prn、.zpl、.imp） | `-i label.zpl` |
| `-z <内容>` | 文字列としてのZPL内容 | `-z "^XA...^XZ"` |
| `-o <フォルダー>` | PDF出力フォルダー | `-o C:\Output` |
| `-n <名前>` | 出力PDFファイル名 | `-n result.pdf` |
| `-w <幅>` | ラベル幅 | `-w 10` |
| `-h <高さ>` | ラベル高さ | `-h 5` |
| `-u <単位>` | 単位（mm、cm、in） | `-u cm` |
| `-d <dpi>` | 印刷密度（デフォルト：203） | `-d 300` |
| `--renderer` | レンダリングエンジン（offline/labelary/auto） | `--renderer labelary` |
| `--fonts-dir` | カスタムフォントディレクトリ | `--fonts-dir C:\Fonts` |
| `--font` | 特定のフォントをマップ | `--font "A=arial.ttf"` |

### **TCPサーバーコマンド**

```bash
ZPL2PDF server start [オプション]    # TCPサーバーを開始（仮想プリンター）
ZPL2PDF server stop                  # TCPサーバーを停止
ZPL2PDF server status                # TCPサーバーのステータスを確認
```

---

## 🎨 **レンダリングエンジン**

### **オフライン（BinaryKits）** - デフォルト
- ✅ インターネットなしで動作
- ✅ 高速処理
- ⚠️ 一部のZPLコマンドは異なってレンダリングされる場合があります

### **Labelary（API）** - 高忠実度
- ✅ 正確なZebraプリンターエミュレーション
- ✅ ベクターPDF出力（小さいファイル）
- ✅ 50以上のラベルの自動バッチ処理
- ⚠️ インターネット接続が必要

### **自動（フォールバック）**
- ✅ 最初にLabelaryを試行
- ✅ オフラインの場合はBinaryKitsにフォールバック

---

## 📐 **ZPLサポート**

### **サポートされているコマンド**

- ✅ `^XA` / `^XZ` - ラベル開始/終了
- ✅ `^PW<幅>` - ポイント単位の印刷幅
- ✅ `^LL<長さ>` - ポイント単位のラベル長さ
- ✅ `^FX FileName:` - カスタム出力ファイル名
- ✅ `^FX !FileName:` - 強制ファイル名（`-n`を上書き）
- ✅ テキスト、グラフィック、バーコード用のすべての標準ZPLコマンド

---

## 📚 **ドキュメント**

- 📖 [完全なドキュメント](../README.md)
- 🌍 [多言語設定](../LANGUAGE_CONFIGURATION.md)
- 🐳 [Dockerガイド](../DOCKER_GUIDE.md)
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
