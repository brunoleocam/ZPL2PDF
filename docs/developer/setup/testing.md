# 🧪 Testing Guide

Comprehensive testing strategy and guides for ZPL2PDF development.

---

## 🎯 **Testing Strategy Overview**

ZPL2PDF uses a multi-layered testing approach to ensure reliability across all platforms:

```
┌─────────────────────────────────────┐
│           Manual Testing            │
│  (Human validation & edge cases)    │
└─────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────┐
│         Integration Tests           │
│  (End-to-end scenarios)             │
└─────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────┐
│          Unit Tests                 │
│  (Individual components)            │
└─────────────────────────────────────┘
```

---

## 🔧 **Testing Environment Setup**

### **Prerequisites**

| Software | Purpose | Version |
|----------|---------|---------|
| **.NET SDK** | Test runner | 9.0+ |
| **Docker** | Cross-platform testing | Latest |
| **Git** | Source control | Latest |
| **VS Code** | IDE (optional) | Latest |

### **Test Project Structure**

```
tests/
├── ZPL2PDF.Unit/                 # Unit tests
│   ├── Services/
│   ├── Domain/
│   ├── Infrastructure/
│   └── Presentation/
├── ZPL2PDF.Integration/          # Integration tests
│   ├── ConversionTests/
│   ├── DaemonTests/
│   └── DockerTests/
└── ZPL2PDF.Performance/          # Performance tests
    ├── LoadTests/
    └── MemoryTests/
```

---

## 🧪 **Unit Testing**

### **Running Unit Tests**

```bash
# Run all unit tests
dotnet test tests/ZPL2PDF.Unit/

# Run specific test class
dotnet test tests/ZPL2PDF.Unit/ --filter ClassName=ConversionServiceTests

# Run with coverage
dotnet test tests/ZPL2PDF.Unit/ --collect:"XPlat Code Coverage"

# Run with detailed output
dotnet test tests/ZPL2PDF.Unit/ --verbosity normal
```

### **Test Categories**

| Category | Purpose | Example |
|----------|---------|---------|
| **Services** | Business logic | `ConversionServiceTests` |
| **Domain** | Value objects | `LabelDimensionsTests` |
| **Infrastructure** | External dependencies | `FileValidatorTests` |
| **Presentation** | CLI arguments | `ArgumentProcessorTests` |

### **Example Unit Test**

```csharp
[Test]
public void ConversionService_ConvertZplToPdf_ValidInput_ReturnsSuccess()
{
    // Arrange
    var service = new ConversionService();
    var options = new ConversionOptions
    {
        InputPath = "test.zpl",
        OutputPath = "output.pdf",
        Width = 4,
        Height = 2,
        Unit = Unit.Inch
    };

    // Act
    var result = service.ConvertZplToPdf(options);

    // Assert
    Assert.IsTrue(result.Success);
    Assert.IsNull(result.Error);
    Assert.IsTrue(File.Exists("output.pdf"));
}
```

---

## 🔄 **Integration Testing**

### **Running Integration Tests**

```bash
# Run all integration tests
dotnet test tests/ZPL2PDF.Integration/

# Run specific integration test
dotnet test tests/ZPL2PDF.Integration/ --filter TestCategory=Conversion

# Run with test data
dotnet test tests/ZPL2PDF.Integration/ --settings integration.runsettings
```

### **Integration Test Categories**

| Category | Purpose | Scope |
|----------|---------|-------|
| **Conversion** | ZPL → PDF conversion | End-to-end |
| **Daemon** | File monitoring | Background process |
| **Docker** | Container functionality | Cross-platform |
| **File System** | File operations | I/O operations |

### **Example Integration Test**

```csharp
[Test]
[Category("Integration")]
public void ZplToPdfConversion_ValidFile_ProducesCorrectOutput()
{
    // Arrange
    var inputFile = Path.Combine(TestDataPath, "sample.zpl");
    var outputDir = Path.Combine(TempPath, "integration-test");
    Directory.CreateDirectory(outputDir);

    // Act
    var result = ZPL2PDF.Convert(inputFile, outputDir, "test.pdf", 4, 2, "in");

    // Assert
    Assert.AreEqual(0, result); // Success exit code
    Assert.IsTrue(File.Exists(Path.Combine(outputDir, "test.pdf")));
    
    // Validate PDF content
    var pdfInfo = ValidatePdfFile(Path.Combine(outputDir, "test.pdf"));
    Assert.AreEqual(4, pdfInfo.WidthInches);
    Assert.AreEqual(2, pdfInfo.HeightInches);
}
```

---

## 🐳 **Docker Testing**

### **Testing ZPL2PDF in Docker**

Docker testing ensures ZPL2PDF works correctly in containerized environments across different operating systems.

#### **Test on Different Linux Distributions**

```bash
# Test on Ubuntu
docker run --rm -v $(pwd):/workspace ubuntu:22.04 bash -c "
  apt update && apt install -y dotnet-runtime-9.0
  cd /workspace
  ./bin/Release/linux-x64/ZPL2PDF --help
"

# Test on Alpine (production image)
docker run --rm -v $(pwd):/workspace alpine:3.19 bash -c "
  apk add --no-cache dotnet-runtime-9.0
  cd /workspace  
  ./bin/Release/linux-x64/ZPL2PDF --help
"

# Test on CentOS
docker run --rm -v $(pwd):/workspace centos:8 bash -c "
  dnf install -y dotnet-runtime-9.0
  cd /workspace
  ./bin/Release/linux-x64/ZPL2PDF --help
"

# Test on Debian
docker run --rm -v $(pwd):/workspace debian:12 bash -c "
  apt update && apt install -y dotnet-runtime-9.0
  cd /workspace
  ./bin/Release/linux-x64/ZPL2PDF --help
"
```

#### **Test Multi-Architecture Support**

```bash
# Test ARM64 on x64 host (requires emulation)
docker run --rm --platform linux/arm64 -v $(pwd):/workspace alpine:3.19 bash -c "
  apk add --no-cache dotnet-runtime-9.0
  cd /workspace
  ./bin/Release/linux-arm64/ZPL2PDF --help
"
```

#### **Test Docker Compose Scenarios**

```bash
# Start test environment
docker-compose -f docker-compose.test.yml up -d

# Run conversion test
docker-compose -f docker-compose.test.yml exec zpl2pdf ZPL2PDF -i /test/sample.zpl -o /output -n test.pdf -w 4 -h 2 -u in

# Verify output
docker-compose -f docker-compose.test.yml exec zpl2pdf ls -la /output/

# Clean up
docker-compose -f docker-compose.test.yml down
```

#### **Test Production Docker Image**

```bash
# Build production image
docker build -t zpl2pdf:test .

# Test basic functionality
docker run --rm zpl2pdf:test --help

# Test with volumes
docker run --rm -v /path/to/zpl:/app/watch -v /path/to/output:/app/output zpl2pdf:test run -l /app/watch -o /app/output

# Test daemon mode
docker run -d --name zpl2pdf-daemon -v /path/to/zpl:/app/watch -v /path/to/output:/app/output zpl2pdf:test run -l /app/watch -o /app/output

# Check daemon status
docker logs zpl2pdf-daemon

# Stop daemon
docker stop zpl2pdf-daemon && docker rm zpl2pdf-daemon
```

---

## 🚀 **Performance Testing**

### **Load Testing**

```bash
# Run performance tests
dotnet test tests/ZPL2PDF.Performance/ --filter TestCategory=Load

# Run with specific parameters
dotnet test tests/ZPL2PDF.Performance/ --filter TestCategory=Load --logger "console;verbosity=detailed"
```

### **Memory Testing**

```bash
# Run memory tests
dotnet test tests/ZPL2PDF.Performance/ --filter TestCategory=Memory

# Monitor memory usage
dotnet test tests/ZPL2PDF.Performance/ --collect:"XPlat Code Coverage" --collect:"XPlat Memory"
```

### **Example Performance Test**

```csharp
[Test]
[Category("Performance")]
public void ConversionService_LargeFile_PerformsWithinTimeLimit()
{
    // Arrange
    var largeZplFile = GenerateLargeZplFile(10000); // 10k lines
    var stopwatch = Stopwatch.StartNew();

    // Act
    var result = ConversionService.Convert(largeZplFile, options);
    stopwatch.Stop();

    // Assert
    Assert.IsTrue(result.Success);
    Assert.Less(stopwatch.ElapsedMilliseconds, 5000); // Should complete in < 5 seconds
}
```

---

## 📊 **Test Data Management**

### **Test Data Structure**

```
tests/TestData/
├── zpl/                    # ZPL test files
│   ├── simple.zpl         # Basic label
│   ├── complex.zpl        # Complex label with graphics
│   ├── barcode.zpl        # Barcode label
│   └── multiline.zpl      # Multi-line text label
├── expected/               # Expected outputs
│   ├── pdfs/              # Expected PDF files
│   └── images/            # Expected images
└── samples/               # Sample files for integration tests
    ├── input/
    └── output/
```

### **Creating Test Data**

```bash
# Create test ZPL file
cat > tests/TestData/zpl/test.zpl << 'EOF'
^XA
^FO50,50^A0N,30,30^FDTest Label^FS
^FO50,100^BY3,2,100^BCN,,N,N,N^FD1234567890^FS
^XZ
EOF

# Create expected PDF
ZPL2PDF -i tests/TestData/zpl/test.zpl -o tests/TestData/expected/pdfs -n test.pdf -w 4 -h 2 -u in
```

---

## 🔍 **Test Coverage**

### **Measuring Coverage**

```bash
# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"

# View coverage report (requires ReportGenerator)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"tests/**/coverage.cobertura.xml" -targetdir:"coverage" -reporttypes:Html
```

### **Coverage Targets**

| Component | Target Coverage | Current |
|-----------|----------------|---------|
| **Services** | 90%+ | ✅ |
| **Domain** | 95%+ | ✅ |
| **Infrastructure** | 80%+ | ✅ |
| **Presentation** | 85%+ | ✅ |
| **Overall** | 85%+ | ✅ |

---

## 🧪 **Manual Testing**

### **Manual Test Scenarios**

#### **Conversion Mode Testing**

```bash
# Test basic conversion
ZPL2PDF -i sample.zpl -o output -n test.pdf -w 4 -h 2 -u in

# Test with different units
ZPL2PDF -i sample.zpl -o output -n test_mm.pdf -w 100 -h 50 -u mm
ZPL2PDF -i sample.zpl -o output -n test_cm.pdf -w 10 -h 5 -u cm

# Test with string input
ZPL2PDF -z "^XA^FO50,50^A0N,30,30^FDHello^FS^XZ" -o output -n string.pdf -w 4 -h 2 -u in

# Test invalid input
ZPL2PDF -i nonexistent.zpl -o output -n test.pdf -w 4 -h 2 -u in
# Expected: Error message

# Test invalid parameters
ZPL2PDF -i sample.zpl -o output -n test.pdf -w -1 -h 2 -u in
# Expected: Error message
```

#### **Daemon Mode Testing**

```bash
# Start daemon
ZPL2PDF run -l /tmp/watch -o /tmp/output

# Copy ZPL file to watch folder
cp sample.zpl /tmp/watch/

# Verify PDF created in output folder
ls -la /tmp/output/

# Stop daemon
ZPL2PDF stop
```

#### **Multi-Language Testing**

```bash
# Test different languages
ZPL2PDF --set-language pt-BR
ZPL2PDF --help  # Should show Portuguese help

ZPL2PDF --set-language es-ES
ZPL2PDF --help  # Should show Spanish help

ZPL2PDF --set-language fr-FR
ZPL2PDF --help  # Should show French help
```

---

## 🐛 **Troubleshooting Tests**

### **Common Test Issues**

#### **Issue: Tests fail on CI but pass locally**
```bash
# Solution: Check environment differences
dotnet --version
dotnet test --logger "console;verbosity=detailed"
```

#### **Issue: Docker tests fail**
```bash
# Solution: Check Docker is running
docker --version
docker ps

# Check Docker build context
docker build --no-cache -t test .
```

#### **Issue: Integration tests timeout**
```bash
# Solution: Increase timeout
dotnet test --logger "console;verbosity=detailed" -- RunConfiguration.Timeout=300000
```

#### **Issue: File permission errors (Linux/macOS)**
```bash
# Solution: Fix permissions
chmod +x bin/Release/linux-x64/ZPL2PDF
chmod 755 tests/TestData/
```

---

## 📋 **Testing Checklist**

### **Before Committing**

- [ ] ✅ **All unit tests pass** (`dotnet test tests/ZPL2PDF.Unit/`)
- [ ] ✅ **All integration tests pass** (`dotnet test tests/ZPL2PDF.Integration/`)
- [ ] ✅ **Code coverage > 85%** (`dotnet test --collect:"XPlat Code Coverage"`)
- [ ] ✅ **No warnings** in build output
- [ ] ✅ **Manual testing** completed for changed features

### **Before Release**

- [ ] ✅ **All tests pass** on all platforms
- [ ] ✅ **Docker tests pass** on multiple distributions
- [ ] ✅ **Performance tests** meet requirements
- [ ] ✅ **Manual testing** completed end-to-end
- [ ] ✅ **Cross-platform testing** completed

---

## 🚀 **Continuous Integration**

### **GitHub Actions Testing**

The CI pipeline automatically runs tests on every push:

```yaml
# .github/workflows/ci.yml
- name: Run Tests
  run: |
    dotnet test --no-build --verbosity normal
    dotnet test --collect:"XPlat Code Coverage"
```

### **Test Results**

Test results are available in:
- **GitHub Actions**: https://github.com/brunoleocam/ZPL2PDF/actions
- **Coverage Reports**: Generated automatically
- **Test Artifacts**: Available for download

---

## 📚 **Testing Best Practices**

### **Unit Testing**

1. ✅ **Test one thing at a time**
2. ✅ **Use descriptive test names**
3. ✅ **Follow AAA pattern** (Arrange, Act, Assert)
4. ✅ **Mock external dependencies**
5. ✅ **Test edge cases and error conditions**

### **Integration Testing**

1. ✅ **Test real scenarios**
2. ✅ **Use actual file system**
3. ✅ **Test cross-platform compatibility**
4. ✅ **Clean up test data**
5. ✅ **Use realistic test data**

### **Performance Testing**

1. ✅ **Set realistic performance targets**
2. ✅ **Test with realistic data sizes**
3. ✅ **Monitor memory usage**
4. ✅ **Test concurrent operations**
5. ✅ **Profile performance bottlenecks**

---

## 🎯 **Next Steps**

1. ✅ **Set up testing environment** with prerequisites
2. ✅ **Run existing tests** to verify setup
3. ✅ **Write new tests** for new features
4. ✅ **Add integration tests** for complex scenarios
5. ✅ **Set up performance testing** for critical paths
6. ✅ **Configure CI/CD** for automated testing

---

**Comprehensive testing ensures ZPL2PDF works reliably across all platforms and scenarios!** 🚀
