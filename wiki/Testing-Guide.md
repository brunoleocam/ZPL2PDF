# 🧪 Testing Guide

Comprehensive testing strategies for ZPL2PDF.

## 🎯 Testing Philosophy

- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test component interactions
- **End-to-End Tests**: Test complete workflows
- **Coverage Goal**: > 80% code coverage

---

## 🏗️ Test Structure

```
tests/
├── ZPL2PDF.Unit/              # Unit tests
│   ├── UnitTests/
│   │   ├── Application/       # Application layer tests
│   │   ├── Domain/            # Domain layer tests
│   │   ├── Infrastructure/    # Infrastructure tests
│   │   └── Presentation/      # Presentation tests
│   ├── Mocks/                 # Mock implementations
│   └── TestData/              # Test data files
│
└── ZPL2PDF.Integration/       # Integration tests
    ├── IntegrationTests/
    └── TestData/
```

---

## 🔧 Running Tests

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test tests/ZPL2PDF.Unit

# Run integration tests only
dotnet test tests/ZPL2PDF.Integration

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Run specific test
dotnet test --filter "FullyQualifiedName~ConversionServiceTests"

# Watch mode
dotnet watch test
```

---

## ✅ Writing Unit Tests

### Example: Testing ConversionService
```csharp
public class ConversionServiceTests
{
    [Fact]
    public void Convert_ValidZpl_ReturnsSuccess()
    {
        // Arrange
        var mockRenderer = new Mock<ILabelRenderer>();
        var mockPdfGen = new Mock<IPdfGenerator>();
        var service = new ConversionService(mockRenderer.Object, mockPdfGen.Object);
        
        var options = new ConversionOptions
        {
            InputPath = "test.txt",
            OutputPath = "output",
            Width = 7.5,
            Height = 15,
            Unit = "in"
        };
        
        // Act
        var result = service.Convert(options);
        
        // Assert
        Assert.True(result.Success);
        mockRenderer.Verify(r => r.Render(It.IsAny<string>(), It.IsAny<LabelDimensions>()), Times.Once);
    }
}
```

---

## 🔗 Integration Tests

### Example: End-to-End Conversion
```csharp
[Fact]
public void EndToEnd_ConvertFile_CreatesPdf()
{
    // Arrange
    var testFile = Path.Combine("TestData", "sample.txt");
    var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    Directory.CreateDirectory(outputDir);
    
    // Act
    var result = Program.Main(new[]
    {
        "-i", testFile,
        "-o", outputDir,
        "-n", "output.pdf",
        "-w", "7.5",
        "-h", "15",
        "-u", "in"
    });
    
    // Assert
    Assert.Equal(0, result);
    Assert.True(File.Exists(Path.Combine(outputDir, "output.pdf")));
    
    // Cleanup
    Directory.Delete(outputDir, true);
}
```

---

## 📊 Test Coverage

```bash
# Generate coverage report
dotnet test /p:CollectCoverage=true /p:CoverletOutput=./coverage/ /p:CoverletOutputFormat=opencover

# Generate HTML report (using ReportGenerator)
reportgenerator -reports:coverage/coverage.opencover.xml -targetdir:coverage/html

# View coverage report
open coverage/html/index.html
```

---

## 🔗 Related Topics

- [[Development Setup]] - Setting up test environment
- [[Contributing Guidelines]] - Testing requirements
- [[Architecture Overview]] - Understanding components
