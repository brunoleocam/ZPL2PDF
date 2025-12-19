# ZPL2PDF - Roadmap de Implementações Futuras

> Este documento lista funcionalidades planejadas para versões futuras do ZPL2PDF.

---

## 📋 Índice

1. [Melhorias na API Labelary](#1-melhorias-na-api-labelary)
2. [Fontes Zebra](#2-fontes-zebra)
3. [Novas Funcionalidades](#3-novas-funcionalidades)
4. [Melhorias de Performance](#4-melhorias-de-performance)
5. [Integrações](#5-integrações)

---

## 1. Melhorias na API Labelary

### 1.1. Rotação de Etiquetas
- **Header**: `X-Rotation`
- **Valores**: `0`, `90`, `180`, `270` (graus, sentido horário)
- **Parâmetro CLI proposto**: `--rotate 90`
- **Prioridade**: Média

### 1.2. Layout de Página PDF
- **Header**: `X-Page-Layout`
- **Valores**: `{colunas}x{linhas}` (ex: `2x3` = 6 etiquetas por página)
- **Parâmetro CLI proposto**: `--page-layout 2x3`
- **Prioridade**: Média

### 1.3. Tamanho de Página PDF
- **Header**: `X-Page-Size`
- **Valores**: `Letter`, `Legal`, `A4`, `A5`, `A6`
- **Parâmetro CLI proposto**: `--page-size A4`
- **Prioridade**: Média

### 1.4. Orientação da Página PDF
- **Header**: `X-Page-Orientation`
- **Valores**: `Portrait`, `Landscape`
- **Parâmetro CLI proposto**: `--page-orientation landscape`
- **Prioridade**: Baixa

### 1.5. Borda das Etiquetas no PDF
- **Header**: `X-Label-Border`
- **Valores**: `Dashed` (padrão), `Solid`, `None`
- **Parâmetro CLI proposto**: `--label-border none`
- **Prioridade**: Baixa

### 1.6. Qualidade de Impressão (PNG)
- **Header**: `X-Quality`
- **Valores**: `Grayscale` (padrão), `Bitonal`
- **Parâmetro CLI proposto**: `--quality bitonal`
- **Uso**: Arquivos menores para impressoras de baixa densidade
- **Prioridade**: Baixa

### 1.7. Linting/Validação de ZPL
- **Header**: `X-Linter: On`
- **Resposta**: `X-Warnings` com warnings em formato pipe-delimited
- **Parâmetro CLI proposto**: `--lint` ou `--validate`
- **Uso**: Validar ZPL antes de imprimir
- **Prioridade**: Alta

### 1.8. Extração de Dados (JSON)
- **Header**: `Accept: application/json`
- **Retorna**: Campos de texto com posições (x, y)
- **Parâmetro CLI proposto**: `--extract-data`
- **Uso**: Extrair dados de etiquetas para processamento
- **Prioridade**: Baixa

### 1.9. Conversão de Imagens para ZPL
- **Endpoint**: `POST http://api.labelary.com/v1/graphics`
- **Parâmetro CLI proposto**: `--image-to-zpl image.png`
- **Uso**: Converter logos/imagens para comandos ZPL (~DG, ~DY)
- **Prioridade**: Média

### 1.10. Conversão de Fontes TTF para ZPL
- **Endpoint**: `POST http://api.labelary.com/v1/fonts`
- **Parâmetros**: `file`, `path`, `name`, `chars`, `unicodes`
- **Parâmetro CLI proposto**: `--font-to-zpl font.ttf`
- **Uso**: Converter fontes TrueType para comandos ZPL
- **Prioridade**: Média

---

## 2. Fontes Zebra

> Referência: [docs/zebra-fonts-research.md](./zebra-fonts-research.md)

### 2.1. Mapeamento de Fontes Internas Zebra

| Font ID | Nome Zebra | Equivalente Open-Source |
|---------|------------|------------------------|
| 0 | Font 0 (Bitmap) | DejaVu Sans Mono |
| A | CG Triumvirate Bold Condensed | Liberation Sans Narrow Bold |
| B | CG Triumvirate | Liberation Sans |
| D | CG Triumvirate Bold | Liberation Sans Bold |
| E | CG Triumvirate Italic | Liberation Sans Italic |
| F | CG Triumvirate Bold Italic | Liberation Sans Bold Italic |
| C | OCR-B | OCR-B (open-source) |
| P | OCR-A | OCR-A Extended |
| Q | MICR E-13B | GnuMICR |

### 2.2. Implementação Proposta

1. **Incluir Liberation Fonts no pacote**
   - Liberation Sans (Regular, Bold, Italic, Bold Italic)
   - Liberation Sans Narrow Bold
   - DejaVu Sans Mono
   - Prioridade: Alta

2. **API de Registro de Fontes**
   - Permitir usuários registrar fontes customizadas
   - Parâmetro: `--fonts-dir /path/to/fonts`
   - Arquivo de configuração: `font-mapping.json`
   - Prioridade: Alta

3. **Ajuste de Métricas**
   - Aplicar fatores de largura/altura para corresponder às métricas Zebra
   - Fator de condensação para fontes A e U: ~0.85
   - Prioridade: Média

4. **PR para BinaryKits.Zpl**
   - Contribuir melhorias de fontes para o projeto upstream
   - Incluir mapeamento de fontes padrão
   - Prioridade: Média

### 2.3. Estrutura do font-mapping.json

```json
{
  "mappings": [
    {
      "zplFont": "A",
      "trueTypeFontFile": "LiberationSansNarrow-Bold.ttf",
      "widthFactor": 0.85
    },
    {
      "zplFont": "B",
      "trueTypeFontFile": "LiberationSans-Regular.ttf"
    },
    {
      "zplFont": "0",
      "trueTypeFontFile": "DejaVuSansMono.ttf"
    }
  ]
}
```

---

## 3. Novas Funcionalidades

### 3.1. Modo Preview (GUI)
- Interface gráfica simples para visualizar etiquetas
- Tecnologia: Avalonia UI (cross-platform)
- Prioridade: Baixa

### 3.2. Modo Watch Avançado
- Monitorar múltiplas pastas
- Configuração via arquivo YAML/JSON
- Prioridade: Baixa

### 3.3. Suporte a Templates
- Arquivos de template com variáveis
- Substituição de variáveis via CLI ou arquivo CSV
- Prioridade: Média

### 3.4. Merge de PDFs
- Combinar múltiplos arquivos ZPL em um único PDF
- Parâmetro: `--merge output.pdf file1.zpl file2.zpl`
- Prioridade: Baixa

### 3.5. Conversão Reversa (PDF para ZPL)
- Extrair imagens de PDF e converter para ZPL
- Complexidade alta, prioridade baixa
- Prioridade: Baixa

---

## 4. Melhorias de Performance

### 4.1. Cache de Renderização
- Cache de etiquetas já renderizadas (hash do ZPL)
- Útil para daemon mode com etiquetas repetidas
- Prioridade: Média

### 4.2. Processamento Paralelo
- Renderizar múltiplas etiquetas em paralelo
- Usar Task Parallel Library (TPL)
- Prioridade: Média

### 4.3. Pool de Conexões HTTP
- Reutilizar conexões para API Labelary
- Já implementado parcialmente com HttpClient singleton
- Prioridade: Baixa

---

## 5. Integrações

### 5.1. Plugin para Visual Studio Code
- Syntax highlighting para ZPL
- Preview de etiquetas no editor
- Prioridade: Baixa

### 5.2. GitHub Action
- Action para validar/renderizar ZPL em CI/CD
- Prioridade: Baixa

### 5.3. NuGet Package
- Publicar como biblioteca NuGet para uso em outros projetos
- Prioridade: Média

### 5.4. Docker Compose com Labelary Local
- Opção de rodar Labelary localmente (se disponível)
- Prioridade: Baixa

---

## 📅 Priorização

### Alta Prioridade
- [ ] Linting/Validação de ZPL (`--lint`)
- [ ] Incluir Liberation Fonts no pacote
- [ ] API de Registro de Fontes (`--fonts-dir`)

### Média Prioridade
- [ ] Rotação de etiquetas (`--rotate`)
- [ ] Layout de página (`--page-layout`)
- [ ] Tamanho de página (`--page-size`)
- [ ] Conversão de imagens para ZPL
- [ ] Conversão de fontes TTF para ZPL
- [ ] Ajuste de métricas de fontes
- [ ] PR para BinaryKits.Zpl
- [ ] Cache de renderização
- [ ] Processamento paralelo
- [ ] NuGet Package

### Baixa Prioridade
- [ ] Orientação da página
- [ ] Borda das etiquetas
- [ ] Qualidade de impressão
- [ ] Extração de dados (JSON)
- [ ] Modo Preview (GUI)
- [ ] Modo Watch avançado
- [ ] Suporte a templates
- [ ] Merge de PDFs
- [ ] Conversão reversa (PDF para ZPL)
- [ ] Plugin VS Code
- [ ] GitHub Action
- [ ] Docker Compose com Labelary local

---

## 📝 Notas

- Documentação da API Labelary: [docs/LABELARY_API.md](./LABELARY_API.md)
- Pesquisa de fontes Zebra: [docs/zebra-fonts-research.md](./zebra-fonts-research.md)
- Changelog: [CHANGELOG.md](../CHANGELOG.md)

---

*Última atualização: Dezembro 2025*

