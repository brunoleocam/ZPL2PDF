# 📋 PLANO COMPLETO DE IMPLEMENTAÇÃO DE TESTES - ZPL2PDF

## 🎯 OBJETIVO
Implementar cobertura completa de testes unitários para todos os Services da aplicação, garantindo qualidade e confiabilidade do código.

## ⏹️ Quando os testes “acabam”?

Em software, **a suíte de testes não tem um fim absoluto**: sempre que houver código novo ou comportamento novo, o ideal é atualizar ou acrescentar testes. Ainda assim, dá para **fechar um ciclo** com critérios objetivos:

| Critério | Situação no ZPL2PDF (referência) |
|----------|----------------------------------|
| Caminhos críticos (conversão, CLI, daemon, fila, config/PID) | Cobertos por unitários + integração |
| Regressão offline (ZPL conhecidos) | `ZplSuite` + testes de infraestrutura |
| CI verde | `dotnet test` em Unit + Integration |
| Cobertura | Meta do projeto (~90% em `CONTRIBUTING`) é **diretriz**, não obrigação diária; usar Cobertura para **priorizar** o que falta |

**O que fica opcional / contínuo:** Labelary/rede, golden files binários, ramos `catch` raros, flakiness de filesystem — evoluir só quando houver risco ou bug real.

**Resumo:** o “fim” desta frente é **estabilizar o que já está no plano + alvos de cobertura óbvios**; depois, testes entram no **fluxo normal** de cada feature/bugfix.

## 📊 SITUAÇÃO ATUAL (atualizado)
- **Projeto de testes unitários**: `tests/ZPL2PDF.Unit/` (não `ZPL2PDF.Tests`).
- **Testes unitários**: centenas de testes cobrindo Application, Domain, Infrastructure, Presentation, regressão offline (`ZplSuite`), localização, etc. Rodar: `dotnet test tests/ZPL2PDF.Unit/ZPL2PDF.Unit.csproj`.
- **Testes de integração**: `tests/ZPL2PDF.Integration/`. Rodar: `dotnet test tests/ZPL2PDF.Integration/ZPL2PDF.Integration.csproj`.
- **Cobertura**: usar coverlet conforme seção de ferramentas; meta ~90% permanece como objetivo.

## 🏗️ ESTRUTURA DE TESTES (implementada)

```
tests/ZPL2PDF.Unit/
├── UnitTests/
│   ├── Application/
│   │   ├── ConversionServiceTests.cs          ✅
│   │   ├── FileValidationServiceTests.cs      ✅
│   │   ├── FileValidationBundledTestDataTests.cs ✅ (TestData/TestFiles)
│   │   ├── PathServiceTests.cs                ✅
│   │   └── UnitConversionServiceTests.cs      ✅
│   ├── Domain/
│   │   ├── ValueObjects/
│   │   │   ├── ConversionOptionsTests.cs      ✅
│   │   │   ├── FileInfoTests.cs               ✅
│   │   │   ├── LabelDimensionsTests.cs        ✅
│   │   │   └── ProcessingResultTests.cs       ✅
│   │   └── Services/
│   │       └── ZplDimensionExtractorTests.cs  ✅
│   ├── Infrastructure/
│   │   ├── ConfigManagerTests.cs              ✅
│   │   ├── DaemonManagerTests.cs              ✅
│   │   ├── FolderMonitorTests.cs              ✅
│   │   ├── PidManagerTests.cs                 ✅
│   │   ├── ProcessManagerTests.cs             ✅
│   │   ├── ProcessingQueueTests.cs            ✅
│   │   ├── LabelRendererTests.cs              ✅
│   │   └── LabelFileReaderTests.cs            ✅
│   ├── Presentation/
│   │   ├── ArgumentProcessorTests.cs          ✅
│   │   ├── ArgumentParserTests.cs             ✅
│   │   ├── ArgumentValidatorTests.cs          ✅
│   │   └── ModeDetectorTests.cs               ✅
│   ├── Regression/
│   │   ├── ZplSuiteOfflineFileTests.cs        ✅
│   │   └── ZplRegressionMatrixTests.cs        ✅
│   └── Shared/
│       └── LocalizationManagerTests.cs        ✅
├── Mocks/
│   ├── MockConversionService.cs               ✅
│   ├── MockFileValidationService.cs           ✅
│   └── MockPathService.cs                     ✅
└── TestData/
    ├── SampleZplData.cs                       ✅
    ├── ZplSuite/                              ✅ (CopyToOutputDirectory)
    ├── TestFiles/                             ✅ (valid.txt, valid.prn, empty.txt, invalid.doc)
    └── ExpectedResults/                       ✅ README (golden files opcionais)

tests/ZPL2PDF.Integration/
└── IntegrationTests/
    ├── ConversionIntegrationTests.cs          ✅
    ├── DaemonIntegrationTests.cs              ✅
    └── FileProcessingIntegrationTests.cs      ✅
```

## 📝 DETALHAMENTO DOS TESTES

### 1. APPLICATION LAYER TESTS

#### 1.1 ConversionServiceTests.cs
**Métodos a testar**:
- `ConvertWithExplicitDimensions()`
- `ConvertWithExtractedDimensions()`
- `Convert()`

**Cenários de teste**:
```csharp
// ConvertWithExplicitDimensions
✅ ConvertWithExplicitDimensions_WithValidZpl_ReturnsImageData
✅ ConvertWithExplicitDimensions_WithEmptyZpl_ReturnsEmptyList
✅ ConvertWithExplicitDimensions_WithNullZpl_ThrowsArgumentException
✅ ConvertWithExplicitDimensions_WithInvalidDimensions_ThrowsException

// ConvertWithExtractedDimensions
✅ ConvertWithExtractedDimensions_WithValidZpl_ReturnsImageData
✅ ConvertWithExtractedDimensions_WithZplContainingDimensions_ExtractsCorrectly
✅ ConvertWithExtractedDimensions_WithEmptyZpl_ReturnsEmptyList

// Convert (mixed approach)
✅ Convert_WithExplicitDimensions_UsesExplicitDimensions
✅ Convert_WithoutExplicitDimensions_UsesExtractedDimensions
✅ Convert_WithZeroDimensions_UsesExtractedDimensions
```

#### 1.2 UnitConversionServiceTests.cs
**Métodos a testar**:
- `ConvertUnit()`
- `ConvertMmToPoints()`
- `ConvertPointsToMm()`
- `ConvertToMillimeters()`

**Cenários de teste**:
```csharp
// ConvertUnit
✅ ConvertUnit_FromMmToCm_ReturnsCorrectValue
✅ ConvertUnit_FromCmToMm_ReturnsCorrectValue
✅ ConvertUnit_FromInchToMm_ReturnsCorrectValue
✅ ConvertUnit_FromMmToInch_ReturnsCorrectValue
✅ ConvertUnit_SameUnit_ReturnsSameValue
✅ ConvertUnit_InvalidUnit_ReturnsOriginalValue

// ConvertMmToPoints
✅ ConvertMmToPoints_WithValidMm_ReturnsCorrectPoints
✅ ConvertMmToPoints_WithCustomDpi_ReturnsCorrectPoints
✅ ConvertMmToPoints_WithZeroMm_ReturnsZero

// ConvertPointsToMm
✅ ConvertPointsToMm_WithValidPoints_ReturnsCorrectMm
✅ ConvertPointsToMm_WithCustomDpi_ReturnsCorrectMm
✅ ConvertPointsToMm_WithZeroPoints_ReturnsZero

// ConvertToMillimeters
✅ ConvertToMillimeters_FromMm_ReturnsSameValues
✅ ConvertToMillimeters_FromCm_ReturnsCorrectValues
✅ ConvertToMillimeters_FromInch_ReturnsCorrectValues
```

#### 1.3 PathServiceTests.cs
**Métodos a testar**:
- `EnsureDirectoryExists()`
- `GetDefaultListenFolder()`
- `GetConfigFolder()`
- `GetPidFolder()`
- `Combine()`
- `GetDirectoryName()`

**Cenários de teste**:
```csharp
// EnsureDirectoryExists
✅ EnsureDirectoryExists_WithValidPath_CreatesDirectory
✅ EnsureDirectoryExists_WithExistingDirectory_DoesNothing
✅ EnsureDirectoryExists_WithNullPath_ThrowsArgumentException
✅ EnsureDirectoryExists_WithEmptyPath_ThrowsArgumentException

// GetDefaultListenFolder
✅ GetDefaultListenFolder_ReturnsDocumentsPath
✅ GetDefaultListenFolder_ContainsZPL2PDFFolder

// GetConfigFolder
✅ GetConfigFolder_OnWindows_ReturnsAppDataPath
✅ GetConfigFolder_OnLinux_ReturnsConfigPath

// GetPidFolder
✅ GetPidFolder_OnWindows_ReturnsTempPath
✅ GetPidFolder_OnLinux_ReturnsVarRunPath

// Combine
✅ Combine_WithValidPaths_ReturnsCombinedPath
✅ Combine_WithNullPath_ThrowsException

// GetDirectoryName
✅ GetDirectoryName_WithValidPath_ReturnsDirectoryName
✅ GetDirectoryName_WithRootPath_ReturnsEmpty
```

### 2. DOMAIN LAYER TESTS

#### 2.1 ValueObjects Tests

##### ConversionOptionsTests.cs
```csharp
✅ Constructor_WithValidParameters_SetsProperties
✅ Constructor_WithInvalidParameters_ThrowsException
✅ Equals_WithSameValues_ReturnsTrue
✅ Equals_WithDifferentValues_ReturnsFalse
✅ GetHashCode_WithSameValues_ReturnsSameHash
```

##### FileInfoTests.cs
```csharp
✅ Constructor_WithValidPath_SetsProperties
✅ Constructor_WithInvalidPath_ThrowsException
✅ IsValid_WithValidFile_ReturnsTrue
✅ IsValid_WithInvalidFile_ReturnsFalse
✅ GetSize_WithValidFile_ReturnsCorrectSize
```

##### LabelDimensionsTests.cs
```csharp
✅ Constructor_WithValidDimensions_SetsProperties
✅ Constructor_WithInvalidDimensions_ThrowsException
✅ ConvertToMm_WithValidUnit_ReturnsCorrectValue
✅ ConvertToMm_WithInvalidUnit_ThrowsException
✅ Equals_WithSameValues_ReturnsTrue
```

##### ProcessingResultTests.cs
```csharp
✅ Constructor_WithSuccess_SetsProperties
✅ Constructor_WithError_SetsProperties
✅ IsSuccess_WithSuccess_ReturnsTrue
✅ IsSuccess_WithError_ReturnsFalse
✅ GetErrorMessage_WithError_ReturnsMessage
```

#### 2.2 Services Tests

##### ZplDimensionExtractorTests.cs
```csharp
✅ ExtractDimensions_WithValidZpl_ReturnsDimensions
✅ ExtractDimensions_WithNoDimensions_ReturnsDefault
✅ ExtractDimensions_WithInvalidZpl_ReturnsDefault
✅ ApplyPriorityLogic_WithExplicitDimensions_UsesExplicit
✅ ApplyPriorityLogic_WithoutExplicit_UsesExtracted
```

### 3. INFRASTRUCTURE LAYER TESTS

#### 3.1 DaemonManagerTests.cs
```csharp
✅ Start_WithValidConfig_StartsSuccessfully
✅ Start_WhenAlreadyRunning_ThrowsException
✅ Stop_WhenRunning_StopsSuccessfully
✅ Stop_WhenNotRunning_ThrowsException
✅ GetStatus_WhenRunning_ReturnsRunning
✅ GetStatus_WhenStopped_ReturnsStopped
```

#### 3.2 FolderMonitorTests.cs
```csharp
✅ Start_WithValidPath_StartsMonitoring
✅ Start_WithInvalidPath_ThrowsException
✅ Stop_WhenMonitoring_StopsMonitoring
✅ OnFileCreated_WithValidFile_ProcessesFile
✅ OnFileCreated_WithInvalidFile_IgnoresFile
```

#### 3.3 ProcessingQueueTests.cs
```csharp
✅ Enqueue_WithValidFile_AddsToQueue
✅ Enqueue_WithInvalidFile_ThrowsException
✅ ProcessQueue_WithValidFiles_ProcessesAll
✅ ProcessQueue_WithLockedFile_Retries
✅ ProcessQueue_WithMaxRetries_RemovesFromQueue
```

### 4. PRESENTATION LAYER TESTS

#### 4.1 ArgumentProcessorTests.cs
```csharp
✅ ParseArguments_WithValidConversionArgs_ReturnsParsedArgs
✅ ParseArguments_WithValidDaemonArgs_ReturnsParsedArgs
✅ ParseArguments_WithInvalidArgs_ThrowsException
✅ ParseArguments_WithMissingRequiredArgs_ThrowsException
```

#### 4.2 ArgumentValidatorTests.cs
```csharp
✅ ValidateConversionArgs_WithValidArgs_ReturnsTrue
✅ ValidateConversionArgs_WithInvalidArgs_ReturnsFalse
✅ ValidateDaemonArgs_WithValidArgs_ReturnsTrue
✅ ValidateDaemonArgs_WithInvalidArgs_ReturnsFalse
```

#### 4.3 ModeDetectorTests.cs
```csharp
✅ DetectMode_WithConversionArgs_ReturnsConversion
✅ DetectMode_WithDaemonArgs_ReturnsDaemon
✅ DetectMode_WithInvalidArgs_ThrowsException
```

### 5. INTEGRATION TESTS

#### 5.1 ConversionIntegrationTests.cs
```csharp
✅ ConvertZplToPdf_EndToEnd_GeneratesValidPdf
✅ ConvertZplToPdf_WithMultipleLabels_GeneratesMultiplePages
✅ ConvertZplToPdf_WithExtractedDimensions_WorksCorrectly
```

#### 5.2 DaemonIntegrationTests.cs
```csharp
✅ StartDaemon_ProcessFile_ConvertsSuccessfully
✅ StartDaemon_StopDaemon_StopsCorrectly
✅ Daemon_WithInvalidFile_HandlesError
```

#### 5.3 FileProcessingIntegrationTests.cs
```csharp
✅ ProcessFile_WithValidZpl_ConvertsToPdf
✅ ProcessFile_WithInvalidZpl_HandlesError
✅ ProcessFile_WithLockedFile_RetriesAndSucceeds
```

## 🚀 IMPLEMENTAÇÃO POR FASES

### FASE 1: Application Layer — **concluída** (evoluir conforme novas APIs)
### FASE 2: Domain Layer — **concluída** (inclui `LabelDimensionsTests` + `ZPL2PDF.Domain.ValueObjects.LabelDimensions.GetHashCode` alinhado a `Equals`)
### FASE 3: Infrastructure Layer — **concluída**
### FASE 4: Presentation Layer — **concluída** (inclui `ArgumentParserTests`)
### FASE 5: Integration Tests — **concluída** (`ZPL2PDF.Integration`)
### FASE 6: Mocks e TestData — **concluída**
- Mocks em `tests/ZPL2PDF.Unit/Mocks/`
- `TestData/TestFiles/` + `FileValidationBundledTestDataTests`
- `TestData/ZplSuite/` + testes de regressão offline
- `TestData/ExpectedResults/README.md` para golden files opcionais (sem binários obrigatórios no repo)

## 📊 MÉTRICAS DE SUCESSO

### Cobertura de Código
- **Atual**: ~10%
- **Fase 1**: ~40%
- **Fase 2**: ~60%
- **Fase 3**: ~75%
- **Fase 4**: ~85%
- **Fase 5**: ~90%
- **Fase 6**: ~95%

### Quantidade de Testes
- **Totais atuais**: obter com `dotnet test tests/ZPL2PDF.Unit/ZPL2PDF.Unit.csproj` e `dotnet test tests/ZPL2PDF.Integration/ZPL2PDF.Integration.csproj`.
- As projeções por fase abaixo são do **plano histórico** (quando o projeto tinha ~6 testes).

## 🛠️ FERRAMENTAS E CONFIGURAÇÕES

### Pacotes NuGet Necessários
```xml
<PackageReference Include="coverlet.collector" Version="6.0.2" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
<PackageReference Include="xunit" Version="2.9.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
<PackageReference Include="Moq" Version="4.20.69" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
```

### Configuração de Cobertura
```xml
<PropertyGroup>
  <CollectCoverage>true</CollectCoverage>
  <CoverletOutputFormat>cobertura</CoverletOutputFormat>
  <CoverletOutput>./coverage/</CoverletOutput>
  <Exclude>[*.Tests]*</Exclude>
</PropertyGroup>
```

## 📋 CHECKLIST DE IMPLEMENTAÇÃO

### Para cada arquivo de teste:
- [ ] Criar classe de teste
- [ ] Implementar métodos de teste
- [ ] Adicionar documentação XML
- [ ] Verificar cobertura de código
- [ ] Executar testes localmente
- [ ] Validar resultados esperados

### Para cada fase:
- [ ] Implementar todos os testes da fase
- [ ] Executar suite completa de testes
- [ ] Verificar cobertura de código
- [ ] Documentar resultados
- [ ] Marcar fase como concluída

## 🎯 PRÓXIMOS PASSOS

1. **Cobertura**: comando em `CONTRIBUTING.md` (XPlat + Cobertura em `coverage-out/`). **Feito:** `ConfigManagerTests`, `PidManagerTests`, `ProcessManagerTests`, `LabelRendererTests`.
2. **Próximos alvos típicos** (por relatório Cobertura): ramos catch raros em I/O; opcionalmente mais cenários de `HandleFileEvent` (ficheiro bloqueado, fila nula). **Feito:** testes dedicados a `ProcessExistingFiles` e a `PollForNewFiles` em `FolderMonitorTests`.
3. **Regressão binária opcional**: baselines em `TestData/ExpectedResults/` (hash ou bytes) quando quiser travar saída PDF/PNG.
4. **Labelary / rede**: testes separados, fora do caminho crítico offline.

## 📝 NOTAS IMPORTANTES

- **Manter compatibilidade** com arquitetura existente
- **Usar dados de teste** consistentes
- **Implementar mocks** para dependências externas
- **Testar cenários de erro** e casos extremos
- **Documentar** cada teste com propósito claro
- **Manter testes** simples e focados
- **Executar testes** frequentemente durante desenvolvimento
