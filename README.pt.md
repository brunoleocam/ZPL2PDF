# ZPL2PDF - Conversor ZPL para PDF

[![Versão](https://img.shields.io/badge/versão-2.0.0-blue.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Plataforma](https://img.shields.io/badge/plataforma-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![Licença](https://img.shields.io/badge/licença-MIT-green.svg)](LICENSE)

Uma ferramenta poderosa e multiplataforma que converte arquivos ZPL (Zebra Programming Language) para documentos PDF de alta qualidade. Perfeita para fluxos de trabalho de impressão de etiquetas, geração automatizada de documentos e sistemas empresariais de gerenciamento de etiquetas.

## 🚀 Novidades na v2.0

- **🔄 Modo Daemon**: Monitoramento automático de pastas e conversão em lote
- **🏗️ Arquitetura Limpa**: Completamente refatorado com princípios SOLID
- **🌍 Multiplataforma**: Suporte nativo para Windows, Linux e macOS
- **📐 Dimensões Inteligentes**: Extração automática de dimensões ZPL (`^PW`, `^LL`)
- **⚡ Alta Performance**: Processamento assíncrono com mecanismos de retry
- **🔧 Pronto para Empresa**: Gerenciamento de PID, arquivos de configuração e logging

## ✨ Principais Recursos

### 🎯 **Modos de Operação Duplos**
- **Modo Conversão**: Converter arquivos individuais ou strings ZPL
- **Modo Daemon**: Monitorar pastas e converter arquivos automaticamente

### 📐 **Gerenciamento Inteligente de Dimensões**
- Extrair dimensões diretamente dos comandos ZPL (`^PW`, `^LL`)
- Suporte para múltiplas unidades (mm, cm, polegadas, pontos)
- Fallback automático para padrões sensatos
- Resolução de dimensões baseada em prioridade

### 🏗️ **Arquitetura Empresarial**
- Arquitetura Limpa com separação de responsabilidades
- Injeção de dependência e princípios SOLID
- Tratamento abrangente de erros e logging
- Mecanismos de retry para cenários de bloqueio de arquivos

### 🌍 **Suporte Multiplataforma**
- Windows (x64, x86)
- Linux (x64, ARM64, ARM)
- macOS (x64, ARM64)
- Executáveis autocontidos

## 📦 Instalação

### Windows (Winget)
```bash
winget install ZPL2PDF
```

### Linux (Em Breve)
```bash
# Ubuntu/Debian
sudo apt install zpl2pdf

# CentOS/RHEL
sudo yum install zpl2pdf
```

### Instalação Manual
Baixe a versão mais recente para sua plataforma na página [Releases](https://github.com/brunoleocam/ZPL2PDF/releases).

## 🚀 Início Rápido

### Conversão Básica
```bash
# Converter um arquivo único
ZPL2PDF.exe -i etiqueta.txt -o pasta_saida -n minha_etiqueta.pdf

# Converter com dimensões personalizadas
ZPL2PDF.exe -i etiqueta.txt -o pasta_saida -w 10 -h 5 -u cm

# Converter string ZPL diretamente
ZPL2PDF.exe -z "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ" -o pasta_saida
```

### Modo Daemon (Conversão Automática)
```bash
# Iniciar daemon com configurações padrão
ZPL2PDF.exe start

# Iniciar com pasta e dimensões personalizadas
ZPL2PDF.exe start -l "C:\Etiquetas" -w 7.5 -h 15 -u in

# Verificar status do daemon
ZPL2PDF.exe status

# Parar daemon
ZPL2PDF.exe stop
```

## 📖 Guia de Uso

### Modo Conversão

Converter arquivos ZPL individuais ou strings para PDF:

```bash
ZPL2PDF.exe -i <arquivo_entrada> -o <pasta_saida> [opções]
ZPL2PDF.exe -z <conteudo_zpl> -o <pasta_saida> [opções]
```

**Parâmetros:**
- `-i <arquivo>`: Arquivo ZPL de entrada (.txt ou .prn)
- `-z <conteudo>`: Conteúdo ZPL como string
- `-o <pasta>`: Pasta de saída para PDF
- `-n <nome>`: Nome do arquivo PDF de saída (opcional)
- `-w <largura>`: Largura da etiqueta
- `-h <altura>`: Altura da etiqueta
- `-u <unidade>`: Unidade (mm, cm, in)
- `-d <dpi>`: Densidade de impressão (203, 300, etc.)

### Modo Daemon

Monitorar pastas e converter arquivos automaticamente:

```bash
ZPL2PDF.exe start [opções]    # Iniciar daemon
ZPL2PDF.exe stop              # Parar daemon
ZPL2PDF.exe status            # Verificar status
```

**Opções do Daemon:**
- `-l <pasta>`: Pasta para monitorar (padrão: Documents/ZPL2PDF Auto Converter)
- `-w <largura>`: Largura fixa para todas as conversões
- `-h <altura>`: Altura fixa para todas as conversões
- `-u <unidade>`: Unidade de medida
- `-d <dpi>`: Densidade de impressão

## 🏗️ Arquitetura

O ZPL2PDF segue os princípios da Arquitetura Limpa com separação clara de responsabilidades:

```
src/
├── Application/          # Casos de Uso e Serviços
│   ├── Services/         # Serviços de lógica de negócio
│   └── Interfaces/       # Contratos de serviços
├── Domain/              # Entidades e regras de negócio
│   ├── ValueObjects/    # Objetos de dados imutáveis
│   └── Services/        # Interfaces de domínio
├── Infrastructure/      # Preocupações externas
│   ├── FileSystem/      # Operações de arquivo
│   ├── Rendering/       # Geração de PDF
│   └── Processing/      # Gerenciamento de filas
└── Presentation/        # CLI e interface do usuário
    ├── Program.cs       # Ponto de entrada
    └── Handlers/        # Manipuladores de modo
```

## 🔧 Configuração

### Arquivo de Configuração (`zpl2pdf.json`)
```json
{
  "defaultWatchFolder": "C:\\Users\\usuario\\Documents\\ZPL2PDF Auto Converter",
  "labelWidth": 7.5,
  "labelHeight": 15,
  "unit": "in",
  "dpi": 203,
  "logLevel": "Info",
  "retryDelay": 2000,
  "maxRetries": 3
}
```

### Variáveis de Ambiente
- `ZPL2PDF_LANGUAGE`: Definir idioma da aplicação
- `ZPL2PDF_LOG_LEVEL`: Definir nível de logging
- `ZPL2PDF_CONFIG_PATH`: Caminho personalizado do arquivo de configuração

## 📐 Suporte ZPL

### Comandos Suportados
- `^XA` / `^XZ`: Início/fim da etiqueta
- `^PW<largura>`: Largura de impressão em pontos
- `^LL<comprimento>`: Comprimento da etiqueta em pontos
- Todos os comandos ZPL padrão de texto, gráficos e códigos de barras

### Extração de Dimensões
A ferramenta extrai automaticamente as dimensões dos comandos ZPL:
- `^PW<largura>` → Largura da etiqueta
- `^LL<comprimento>` → Altura da etiqueta
- Converte pontos para milímetros: `mm = (pontos / 203) * 25.4`

### Lógica de Prioridade
1. **Comandos ZPL**: Extrair de `^PW` e `^LL`
2. **Parâmetros Explícitos**: Usar valores `-w` e `-h`
3. **Valores Padrão**: Fallback para 100mm × 150mm

## 🐳 Suporte Docker

### Executar com Docker
```bash
# Construir imagem
docker build -t zpl2pdf .

# Executar modo daemon
docker run -d -v /caminho/para/etiquetas:/app/watch zpl2pdf start

# Executar conversão
docker run -v /caminho/entrada:/app/input -v /caminho/saida:/app/output zpl2pdf -i /app/input/etiqueta.txt -o /app/output
```

### Docker Compose
```yaml
version: '3.8'
services:
  zpl2pdf:
    build: .
    volumes:
      - ./etiquetas:/app/watch
      - ./saida:/app/output
    command: start -l /app/watch -o /app/output
```

## 🧪 Testes

### Executar Testes
```bash
# Testes unitários
dotnet test tests/ZPL2PDF.Unit/

# Testes de integração
dotnet test tests/ZPL2PDF.Integration/

# Todos os testes com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Cobertura de Testes
- **Testes Unitários**: Meta de 90%+ de cobertura
- **Testes de Integração**: Fluxos de trabalho end-to-end
- **Multiplataforma**: Windows, Linux, macOS

## 🤝 Contribuindo

Aceitamos contribuições! Consulte nosso [Guia de Contribuição](CONTRIBUTING.md) para detalhes.

### Configuração de Desenvolvimento
```bash
# Clonar repositório
git clone https://github.com/brunoleocam/ZPL2PDF.git
cd ZPL2PDF

# Restaurar dependências
dotnet restore

# Construir solução
dotnet build

# Executar testes
dotnet test
```

### Processo de Pull Request
1. Fazer fork do repositório
2. Criar uma branch de feature
3. Fazer suas alterações
4. Adicionar testes para nova funcionalidade
5. Garantir que todos os testes passem
6. Enviar um pull request

## 📊 Performance

### Benchmarks
- **Etiqueta Única**: ~50ms tempo de conversão
- **Processamento em Lote**: 100+ etiquetas/minuto
- **Uso de Memória**: <50MB típico
- **Tamanho do Arquivo**: ~100KB por PDF de etiqueta

### Recursos de Otimização
- Processamento assíncrono com concorrência configurável
- Mecanismos de retry para arquivos bloqueados
- Processamento de imagem eficiente em memória
- Geração de PDF otimizada

## 🐛 Solução de Problemas

### Problemas Comuns

**Erro de Arquivo Bloqueado**
```
Erro: Arquivo em uso, aguardando: etiqueta.txt
```
- **Solução**: O arquivo está sendo escrito. Aguarde o processo completar.

**Conteúdo ZPL Inválido**
```
Erro: Nenhuma etiqueta ZPL encontrada no arquivo
```
- **Solução**: Certifique-se de que o arquivo contém comandos ZPL válidos (`^XA...^XZ`).

**Permissão Negada**
```
Erro: Acesso ao caminho foi negado
```
- **Solução**: Execute com permissões apropriadas ou verifique o acesso à pasta.

### Modo Debug
```bash
# Habilitar logging verboso
ZPL2PDF.exe -i etiqueta.txt -o saida --log-level Debug
```

## 📄 Licença

Este projeto está licenciado sob a Licença MIT - consulte o arquivo [LICENSE](LICENSE) para detalhes.

## 🙏 Agradecimentos

- [BinaryKits.Zpl](https://github.com/BinaryKits/BinaryKits.Zpl) - Parsing e renderização ZPL
- [PdfSharpCore](https://github.com/empira/PdfSharpCore) - Geração de PDF
- [SkiaSharp](https://github.com/mono/SkiaSharp) - Gráficos multiplataforma

## 📞 Suporte

- **Documentação**: [Wiki](https://github.com/brunoleocam/ZPL2PDF/wiki)
- **Problemas**: [GitHub Issues](https://github.com/brunoleocam/ZPL2PDF/issues)
- **Discussões**: [GitHub Discussions](https://github.com/brunoleocam/ZPL2PDF/discussions)

---
