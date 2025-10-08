# 💻 Development Setup

Complete guide for setting up your ZPL2PDF development environment.

## 🎯 Prerequisites

### Required Software
- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Git** - [Download](https://git-scm.com/downloads)
- **Code Editor** - Visual Studio, VS Code, or Rider

### Optional Software
- **Docker** - [Download](https://www.docker.com/products/docker-desktop)
- **Windows Terminal** - For better CLI experience
- **Git GUI** - SourceTree, GitHub Desktop, etc.

---

## 🚀 Quick Start

```bash
# Clone repository
git clone https://github.com/brunoleocam/ZPL2PDF.git
cd ZPL2PDF

# Restore dependencies
dotnet restore

# Build project
dotnet build

# Run tests
dotnet test

# Run application
dotnet run -- --help
```

---

## 🔧 IDE Setup

### Visual Studio 2022
1. Install **Visual Studio 2022** with .NET workload
2. Open `ZPL2PDF.sln`
3. Install recommended extensions:
   - ReSharper (optional)
   - EditorConfig Language Service

### VS Code
1. Install **Visual Studio Code**
2. Install extensions:
   - C# (ms-dotnettools.csharp)
   - C# Dev Kit
   - EditorConfig for VS Code
3. Open folder `ZPL2PDF`
4. Press `F5` to build and run

### JetBrains Rider
1. Install **JetBrains Rider**
2. Open `ZPL2PDF.sln`
3. Configure test runner
4. Enable code analysis

---

## 🧪 Testing Setup

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run specific test category
dotnet test --filter "Category=Unit"

# Watch mode (re-run on changes)
dotnet watch test
```

---

## 🐳 Docker Development

```bash
# Build Docker image
docker build -t zpl2pdf:dev .

# Run container
docker run --rm -v $(pwd):/app/watch zpl2pdf:dev

# Docker Compose
docker-compose up --build
```

---

## 🔗 Related Topics

- [[Architecture Overview]] - Understanding the codebase
- [[Contributing Guidelines]] - Contribution workflow
- [[Testing Guide]] - Writing tests
- [[Build Process]] - Building for production
