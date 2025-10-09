# 🔄 Git Workflow Guide

Complete guide to Git workflow, branching strategy, and commit conventions for ZPL2PDF development.

---

## 🎯 **Branch Protection Overview**

ZPL2PDF uses **branch protection** on the `main` branch to ensure code quality and prevent direct pushes:

```
Developer
    ↓
Create Feature Branch
    ↓
Make Changes & Commit
    ↓
Push Branch
    ↓
Create Pull Request
    ↓
CI/CD Runs Tests
    ↓
Code Review
    ↓
Merge to Main
```

**⚠️ IMPORTANT:** Direct pushes to `main` are **BLOCKED**. All changes must go through Pull Requests.

---

## 🌿 **Branching Strategy**

### **Branch Types**

| Branch Type | Purpose | Naming Convention | Example |
|-------------|---------|-------------------|---------|
| **main** | Production-ready code | `main` | `main` |
| **feature** | New features | `feature/description` | `feature/docker-support` |
| **fix** | Bug fixes | `fix/description` | `fix/memory-leak` |
| **hotfix** | Critical fixes | `hotfix/description` | `hotfix/security-patch` |
| **release** | Release preparation | `release/version` | `release/v2.1.0` |

### **Branch Lifecycle**

```
main
 ├── feature/new-feature
 │   ├── commit 1: "feat: add basic functionality"
 │   ├── commit 2: "test: add unit tests"
 │   └── commit 3: "docs: update documentation"
 │
 ├── fix/bug-fix
 │   ├── commit 1: "fix: resolve memory issue"
 │   └── commit 2: "test: add regression test"
 │
 └── hotfix/critical-issue
     └── commit 1: "fix: patch security vulnerability"
```

---

## 📝 **Commit Convention**

ZPL2PDF follows **Conventional Commits** specification for consistent, automated versioning and changelog generation.

### **Commit Format**

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

### **Commit Types**

| Type | Description | Example |
|------|-------------|---------|
| **feat** | New feature | `feat: add Docker support` |
| **fix** | Bug fix | `fix: resolve memory leak in daemon` |
| **docs** | Documentation changes | `docs: update installation guide` |
| **style** | Code style changes | `style: format code with prettier` |
| **refactor** | Code refactoring | `refactor: extract service classes` |
| **test** | Adding/updating tests | `test: add integration tests` |
| **chore** | Maintenance tasks | `chore: update dependencies` |
| **perf** | Performance improvements | `perf: optimize PDF generation` |
| **ci** | CI/CD changes | `ci: add Docker build workflow` |
| **build** | Build system changes | `build: update Inno Setup script` |

### **Commit Examples**

#### **✅ Good Commits**

```bash
# Feature
git commit -m "feat: add multi-language support for 8 languages"

# Bug fix
git commit -m "fix: resolve file locking issue in daemon mode"

# Documentation
git commit -m "docs: add comprehensive Docker usage guide"

# Test
git commit -m "test: add unit tests for ConversionService"

# Refactor
git commit -m "refactor: extract ZplDimensionExtractor to separate class"

# Performance
git commit -m "perf: optimize memory usage in large file processing"

# Breaking change
git commit -m "feat!: change default output directory structure

BREAKING CHANGE: Output files are now organized by date instead of flat structure"
```

#### **❌ Bad Commits**

```bash
# Too vague
git commit -m "fix stuff"

# No type
git commit -m "added new feature"

# Too long without body
git commit -m "fix: resolve the memory leak issue that was causing the application to crash when processing large files and also improve error handling"

# Mixed changes
git commit -m "fix bug and add feature"
```

---

## 🔄 **Development Workflow**

### **Step 1: Create Feature Branch**

```bash
# Update main branch
git checkout main
git pull origin main

# Create and switch to feature branch
git checkout -b feature/your-feature-name

# Example:
git checkout -b feature/docker-support
git checkout -b fix/memory-leak
git checkout -b docs/update-readme
```

### **Step 2: Make Changes and Commit**

```bash
# Make your changes
# ... edit files ...

# Stage changes
git add .

# Or stage specific files
git add src/Services/ConversionService.cs
git add tests/ZPL2PDF.Unit/Services/ConversionServiceTests.cs

# Commit with conventional format
git commit -m "feat: add Docker support for cross-platform builds"

# Make more commits as needed
git add docs/Docker.md
git commit -m "docs: add Docker installation guide"
```

### **Step 3: Push Branch**

```bash
# Push branch to remote
git push origin feature/your-feature-name

# Example:
git push origin feature/docker-support
```

### **Step 4: Create Pull Request**

1. **Go to GitHub**: https://github.com/brunoleocam/ZPL2PDF/pulls
2. **Click "New pull request"**
3. **Select branches**: `feature/your-feature` → `main`
4. **Write PR title**: Use conventional commit format
5. **Write PR description**: Explain what was changed and why
6. **Click "Create pull request"**

#### **PR Title Examples**

```
feat: add Docker support for cross-platform builds
fix: resolve memory leak in daemon mode
docs: update installation guide with new methods
refactor: extract service classes for better testability
```

#### **PR Description Template**

```markdown
## 📝 Description
Brief description of changes

## 🔄 Type of Change
- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] Documentation update

## ✅ Testing
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Manual testing completed

## 📋 Checklist
- [ ] Code follows project style guidelines
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] No breaking changes (or documented if intentional)
```

### **Step 5: CI/CD and Review**

```bash
# GitHub Actions automatically runs:
# ✅ Build tests
# ✅ Unit tests  
# ✅ Integration tests
# ✅ Code quality checks
# ✅ Security scans

# Wait for:
# ✅ All CI checks to pass
# ✅ Code review approval
# ✅ No merge conflicts
```

### **Step 6: Merge**

Once approved and CI passes:

1. **Click "Merge pull request"**
2. **Choose merge type**:
   - **Create merge commit** (recommended)
   - **Squash and merge** (for clean history)
   - **Rebase and merge** (for linear history)
3. **Click "Confirm merge"**
4. **Delete branch** (optional, recommended)

### **Step 7: Clean Up**

```bash
# Switch back to main
git checkout main

# Pull latest changes
git pull origin main

# Delete local feature branch
git branch -d feature/your-feature-name

# Delete remote branch (if not auto-deleted)
git push origin --delete feature/your-feature-name
```

---

## 🚨 **Special Cases**

### **Hotfix Workflow**

For critical issues that need immediate release:

```bash
# Create hotfix branch from main
git checkout main
git pull origin main
git checkout -b hotfix/critical-security-fix

# Make minimal fix
git add .
git commit -m "fix: patch critical security vulnerability"

# Push and create PR immediately
git push origin hotfix/critical-security-fix

# Create PR with high priority
# Mark as "urgent" in description
# Request immediate review
```

### **Release Workflow**

For preparing releases:

```bash
# Create release branch
git checkout main
git pull origin main
git checkout -b release/v2.1.0

# Update version numbers
# Update CHANGELOG.md
# Update documentation

git add .
git commit -m "chore: bump version to 2.1.0"
git commit -m "docs: update changelog for v2.1.0"

# Push release branch
git push origin release/v2.1.0

# Create PR for release
# After merge, create Git tag
git tag -a v2.1.0 -m "Release version 2.1.0"
git push origin v2.1.0
```

### **Handling Merge Conflicts**

```bash
# If conflicts occur during PR merge
git checkout feature/your-feature
git pull origin main

# Resolve conflicts in your editor
# Edit conflicted files
# Remove conflict markers (<<<<<<< ======= >>>>>>>)

# Stage resolved files
git add resolved-file.cs

# Commit resolution
git commit -m "resolve: merge conflicts with main"

# Push updated branch
git push origin feature/your-feature
```

---

## 🔍 **Code Review Guidelines**

### **For Authors**

#### **Before Creating PR**

- [ ] ✅ **All tests pass** locally
- [ ] ✅ **Code follows style guidelines**
- [ ] ✅ **Self-review completed**
- [ ] ✅ **Documentation updated**
- [ ] ✅ **No debugging code** left in
- [ ] ✅ **Commit messages** follow convention

#### **PR Best Practices**

- [ ] ✅ **Small, focused PRs** (< 400 lines)
- [ ] ✅ **Clear description** of changes
- [ ] ✅ **Link related issues**
- [ ] ✅ **Add screenshots** for UI changes
- [ ] ✅ **Test instructions** for reviewers

### **For Reviewers**

#### **Review Checklist**

- [ ] ✅ **Code quality** and style
- [ ] ✅ **Functionality** works as expected
- [ ] ✅ **Tests** are adequate
- [ ] ✅ **Documentation** is updated
- [ ] ✅ **Performance** implications
- [ ] ✅ **Security** considerations

#### **Review Best Practices**

- [ ] ✅ **Be constructive** and helpful
- [ ] ✅ **Explain reasoning** for suggestions
- [ ] ✅ **Approve** when ready to merge
- [ ] ✅ **Request changes** when needed
- [ ] ✅ **Test changes** locally if complex

---

## 📊 **Branch Protection Rules**

### **Current Protection Settings**

| Rule | Status | Description |
|------|--------|-------------|
| **Require PR** | ✅ Enabled | All changes must go through PR |
| **Required Reviews** | ✅ 1 required | At least 1 approval needed |
| **Dismiss stale reviews** | ✅ Enabled | Reviews dismissed on new commits |
| **Require status checks** | ✅ Enabled | CI must pass before merge |
| **Require branches up to date** | ✅ Enabled | Branch must be current with main |
| **Restrict pushes** | ✅ Enabled | Only maintainers can push to main |
| **Allow force pushes** | ❌ Disabled | Prevents history rewriting |
| **Allow deletions** | ❌ Disabled | Prevents branch deletion |

### **Required Status Checks**

Before merging, these checks must pass:

| Check | Purpose | Time |
|-------|---------|------|
| **Build & Test (Windows x64)** | Windows compatibility | ~10 min |
| **Build & Test (Linux x64)** | Linux compatibility | ~8 min |
| **Build & Test (macOS x64)** | macOS compatibility | ~12 min |
| **Code Quality** | Code standards | ~3 min |
| **Security Scan** | Vulnerability check | ~5 min |

---

## 🛠️ **Git Commands Reference**

### **Essential Commands**

```bash
# Branch management
git checkout -b feature/new-feature    # Create and switch to branch
git branch -a                         # List all branches
git branch -d feature/old-feature     # Delete local branch
git push origin --delete feature/branch # Delete remote branch

# Staging and committing
git add .                             # Stage all changes
git add filename.cs                   # Stage specific file
git commit -m "feat: add new feature" # Commit with message
git commit --amend                    # Amend last commit

# Syncing with remote
git pull origin main                  # Pull latest changes
git push origin branch-name           # Push branch to remote
git fetch origin                      # Fetch without merging

# History and diff
git log --oneline                     # Show commit history
git log --oneline -10                 # Show last 10 commits
git diff                              # Show unstaged changes
git diff --staged                     # Show staged changes
git show commit-hash                  # Show specific commit

# Undoing changes
git reset --soft HEAD~1               # Undo last commit, keep changes
git reset --hard HEAD~1               # Undo last commit, discard changes
git checkout -- filename.cs           # Discard changes to file
git revert commit-hash                # Create revert commit
```

### **Advanced Commands**

```bash
# Interactive rebase
git rebase -i HEAD~3                  # Interactive rebase last 3 commits

# Stash management
git stash                             # Stash current changes
git stash pop                         # Apply and remove stash
git stash list                        # List stashes

# Cherry-picking
git cherry-pick commit-hash           # Apply specific commit

# Submodules
git submodule update --init --recursive # Initialize submodules
```

---

## 📚 **Best Practices**

### **Commit Best Practices**

1. ✅ **One logical change per commit**
2. ✅ **Write clear, descriptive messages**
3. ✅ **Use conventional commit format**
4. ✅ **Keep commits small and focused**
5. ✅ **Test before committing**

### **Branch Best Practices**

1. ✅ **Use descriptive branch names**
2. ✅ **Keep branches short-lived**
3. ✅ **Regularly sync with main**
4. ✅ **Delete merged branches**
5. ✅ **Use feature flags for large features**

### **PR Best Practices**

1. ✅ **Keep PRs small and focused**
2. ✅ **Write clear descriptions**
3. ✅ **Link related issues**
4. ✅ **Request appropriate reviewers**
5. ✅ **Respond to feedback promptly**

---

## 🚀 **Getting Started**

### **First Time Setup**

```bash
# Clone repository
git clone https://github.com/brunoleocam/ZPL2PDF.git
cd ZPL2PDF

# Configure Git (if not already done)
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"

# Verify branch protection
git checkout main
git pull origin main
```

### **Daily Workflow**

```bash
# Start of day
git checkout main
git pull origin main

# Create feature branch
git checkout -b feature/daily-work

# Make changes and commit
git add .
git commit -m "feat: implement daily feature"

# Push and create PR
git push origin feature/daily-work
# Create PR on GitHub

# End of day - clean up
git checkout main
git pull origin main
git branch -d feature/daily-work
```

---

## 🎯 **Next Steps**

1. ✅ **Understand branch protection** rules
2. ✅ **Practice conventional commits** format
3. ✅ **Create your first feature branch**
4. ✅ **Make changes and commit** following guidelines
5. ✅ **Create your first Pull Request**
6. ✅ **Participate in code reviews**

---

**This workflow ensures high code quality and collaborative development!** 🚀
