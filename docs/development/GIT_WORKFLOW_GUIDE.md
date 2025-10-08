# 🔀 Git Workflow Guide - ZPL2PDF

## 🎯 **Overview**

This guide explains the Git workflow and branching strategy used in the ZPL2PDF project for contributors and maintainers.

---

## 🌳 **Branching Strategy**

### **Branch Structure**

| Branch | Purpose | Status |
|--------|---------|--------|
| `main` | **Production** - Stable code, releases | Protected |
| `dev` | **Development** - Feature integration | Active |
| `feature/*` | Specific features | Temporary |
| `hotfix/*` | Urgent fixes | Temporary |

### **Recommended Flow (Simplified Git Flow)**

```
main (production)
  ↑
  └── dev (development)
       ↑
       ├── feature/new-feature
       ├── feature/another-feature
       └── hotfix/urgent-fix
```

---

## 🚀 **Development Workflow**

### **1. Setting Up Git Configuration**

```bash
# Configure global user (for all repositories)
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"

# Or configure for this repository only
cd ZPL2PDF
git config user.name "Your Name"
git config user.email "your.email@example.com"

# Verify configuration
git config --list
```

### **2. Starting New Work**

```bash
# Ensure you're on dev branch
git checkout dev
git pull origin dev

# Create feature branch
git checkout -b feature/your-feature-name

# Make your changes...
# Test your changes...
# Commit your work
git add .
git commit -m "feat: add your feature description"
```

### **3. Submitting Changes**

```bash
# Push feature branch
git push origin feature/your-feature-name

# Create Pull Request on GitHub:
# https://github.com/brunoleocam/ZPL2PDF/compare/dev...feature/your-feature-name
```

---

## 📋 **Commit Message Guidelines**

### **Format**
```
<type>: <description>

[optional body]

[optional footer]
```

### **Types**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

### **Examples**
```bash
git commit -m "feat: add multi-language support for Portuguese"
git commit -m "fix: resolve dimension extraction issue"
git commit -m "docs: update README with installation instructions"
git commit -m "refactor: extract common validation logic"
```

---

## 🔄 **Release Process**

### **1. Preparing Release**

```bash
# Update version in files:
# - ZPL2PDF.csproj
# - CHANGELOG.md
# - README.md (if needed)

# Commit version bump
git add .
git commit -m "chore: bump version to 2.1.0"
git push origin dev
```

### **2. Creating Release**

```bash
# Merge dev to main (via PR)
# Create Git tag
git tag -a v2.1.0 -m "Release version 2.1.0"
git push origin v2.1.0

# Create GitHub Release
# - Go to: https://github.com/brunoleocam/ZPL2PDF/releases/new
# - Select tag: v2.1.0
# - Write release notes
# - Upload build artifacts
# - Click "Publish release"
```

### **3. Automated Actions**

When you create a GitHub Release, these workflows run automatically:
- ✅ Run all tests
- ✅ Build all platforms (8 architectures)
- ✅ Build Docker images
- ✅ Publish to Docker Hub + GitHub Container Registry
- ✅ Build Windows installer
- ✅ Upload artifacts to GitHub Release
- ✅ Create WinGet package PR

---

## 🛡️ **Security and Best Practices**

### **Pre-commit Checklist**

```bash
# 1. Check repository status
git status

# 2. Review changes
git diff

# 3. Verify no sensitive files
git ls-files | grep -E "(\.env|\.key|password|secret)"

# 4. Run tests locally
dotnet test

# 5. Build locally
dotnet build
```

### **Protecting Sensitive Information**

```bash
# Never commit these:
.env
*.key
*.pem
secrets.json
config.json (with passwords)

# Add to .gitignore
echo "*.env" >> .gitignore
echo "*.key" >> .gitignore
echo "secrets.json" >> .gitignore
```

### **Undoing Changes**

```bash
# Undo last commit (keep changes)
git reset --soft HEAD~1

# Undo last commit (discard changes)
git reset --hard HEAD~1

# Undo changes in specific file
git checkout -- filename.cs

# Remove file from staging
git reset HEAD filename.cs
```

---

## 🔍 **Reviewing Changes**

### **What to Review**

- ✅ Code quality and style
- ✅ Test coverage
- ✅ Documentation updates
- ✅ Breaking changes
- ✅ Performance impact
- ✅ Security implications

### **Review Process**

1. **Automated Checks**: CI/CD runs tests and builds
2. **Code Review**: Human review of changes
3. **Testing**: Manual testing if needed
4. **Approval**: Merge after approval

---

## 📊 **Branch Protection Rules**

The `main` branch is protected with:
- ✅ Require pull request reviews
- ✅ Require status checks to pass
- ✅ Require branches to be up to date
- ✅ Restrict pushes to main

---

## 🌍 **Contributing to Open Source**

### **For External Contributors**

1. **Fork the repository** on GitHub
2. **Clone your fork** locally
3. **Create feature branch** from `dev`
4. **Make your changes** and test them
5. **Commit with clear messages**
6. **Push to your fork**
7. **Create Pull Request** to `dev` branch

### **Pull Request Guidelines**

- ✅ Clear title and description
- ✅ Reference related issues
- ✅ Include screenshots if UI changes
- ✅ Update documentation if needed
- ✅ Ensure all tests pass

---

## 📚 **Useful Commands**

```bash
# View repository status
git status

# View commit history
git log --oneline -10

# View changes
git diff

# View staged changes
git diff --cached

# View branch structure
git log --oneline --graph --all

# Stash changes temporarily
git stash
git stash pop

# Create and switch to new branch
git checkout -b new-branch

# Switch branches
git checkout branch-name

# Merge branch
git merge branch-name

# Rebase branch
git rebase main
```

---

## 🆘 **Getting Help**

- **Documentation**: [Git Documentation](https://git-scm.com/doc)
- **GitHub Guides**: [GitHub Guides](https://guides.github.com/)
- **Project Issues**: [GitHub Issues](https://github.com/brunoleocam/ZPL2PDF/issues)
- **Discussions**: [GitHub Discussions](https://github.com/brunoleocam/ZPL2PDF/discussions)

---

**This workflow ensures code quality, collaboration, and smooth releases!** 🚀