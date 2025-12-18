# Labelary API - Documentação de Referência

> Documentação completa da API Labelary para renderização de ZPL.
> Fonte: [labelary.com/service.html](http://labelary.com/service.html)

## Índice

1. [Introdução](#1-introdução)
2. [Parâmetros](#2-parâmetros)
3. [Limites](#3-limites)
4. [Planos e Preços](#4-planos-e-preços)
5. [Formatos de Saída](#5-formatos-de-saída)
6. [Exemplos](#6-exemplos)
7. [Funcionalidades Avançadas](#7-funcionalidades-avançadas)
8. [Outras Funcionalidades](#8-outras-funcionalidades)

---

## 1. Introdução

O motor de renderização ZPL do Labelary está disponível como um serviço online via API RESTful simples:

### Método GET

```
GET http://api.labelary.com/v1/printers/{dpmm}/labels/{width}x{height}/{index}/{zpl}
```

### Método POST

```
POST http://api.labelary.com/v1/printers/{dpmm}/labels/{width}x{height}/{index}
```

O método POST é útil quando:
- O ZPL é muito grande (URLs são limitadas a ~3.000 caracteres)
- Há problemas de codificação de caracteres
- O ZPL contém dados binários embutidos
- O ZPL contém informações sensíveis (URLs podem ser logadas por proxies)

### Content-Types Suportados (POST)

| Content-Type | Descrição |
|--------------|-----------|
| `application/x-www-form-urlencoded` | Body contém o ZPL raw |
| `multipart/form-data` | ZPL no parâmetro `file` |

---

## 2. Parâmetros

| Parâmetro | Descrição | Valores |
|-----------|-----------|---------|
| `dpmm` | Densidade de impressão (dots per mm) | `6dpmm`, `8dpmm`, `12dpmm`, `24dpmm` |
| `width` | Largura da etiqueta em **polegadas** | Qualquer valor numérico |
| `height` | Altura da etiqueta em **polegadas** | Qualquer valor numérico |
| `index` | Índice da etiqueta (base 0) | Inteiro (opcional para PDF) |
| `zpl` | Código ZPL a renderizar | String ZPL |

### Conversão de DPI para DPMM

| DPI | DPMM | Uso Típico |
|-----|------|------------|
| 152 | 6 | Impressoras de baixa resolução |
| 203 | 8 | **Padrão** - maioria das impressoras |
| 300 | 12 | Impressoras de alta resolução |
| 600 | 24 | Impressoras de altíssima resolução |

### Conversão de Unidades para Polegadas

| De | Para Polegadas | Fórmula |
|----|----------------|---------|
| mm | in | `mm / 25.4` |
| cm | in | `cm / 2.54` |
| in | in | direto |

---

## 3. Limites

| Limite | Free | Plus | Business | Erro |
|--------|------|------|----------|------|
| Requisições/segundo | 3 | 6 | 10 | HTTP 429 |
| Requisições/dia | 5.000 | 20.000 | 40.000 | HTTP 429 |
| **Etiquetas/requisição** | **50** | **50** | **50** | HTTP 413 |
| Tamanho do body | 1 MB | 1 MB | 1 MB | HTTP 413 |
| Dimensões da etiqueta | 15 in | 15 in | 15 in | HTTP 400 |
| Dimensões de imagem embutida | 2.000 px | 2.000 px | 2.000 px | HTTP 400 |
| Tamanho de objeto embutido | 2 MB | 2 MB | 2 MB | HTTP 400 |
| Memória da impressora | 2 MB | 2 MB | 2 MB | HTTP 400 |
| Buffer de imagem PNG | 10 MB | 10 MB | 10 MB | HTTP 400 |

---

## 4. Planos e Preços

| Recurso | Free | Plus ($90/mês) | Business ($228/mês) |
|---------|------|----------------|---------------------|
| Deployment | SaaS | SaaS | SaaS |
| Suporte | Nenhum | Básico | Premium |
| SLA de Disponibilidade | Nenhum | 99% | 99.9% |
| Retenção de Dados | 60 dias | Nunca | Nunca |
| API Key | Nenhuma | Via email | Via email |
| Servidor | api.labelary.com | Privado | Privado |

---

## 5. Formatos de Saída

Controlados pelo header `Accept`:

| Formato | Header Accept | Descrição |
|---------|---------------|-----------|
| **PNG** | `image/png` ou omitir | Padrão |
| **PDF** | `application/pdf` | Vetorial, múltiplas etiquetas |
| IPL | `application/ipl` | Intermec |
| EPL | `application/epl` | Eltron |
| DPL | `application/dpl` | Datamax |
| SBPL | `application/sbpl` | SATO |
| PCL 5 | `application/pcl5` | HP |
| PCL 6 | `application/pcl6` | HP |
| JSON | `application/json` | Extração de dados |

---

## 6. Exemplos

### 6.1. curl

```bash
# GET - PNG
curl --get http://api.labelary.com/v1/printers/8dpmm/labels/4x6/0/ \
  --data-urlencode "^xa^cfa,50^fo100,100^fdHello World^fs^xz" > label.png

# POST - PNG
curl --request POST http://api.labelary.com/v1/printers/8dpmm/labels/4x6/0/ \
  --data "^xa^cfa,50^fo100,100^fdHello World^fs^xz" > label.png

# POST - PDF
curl --request POST http://api.labelary.com/v1/printers/8dpmm/labels/4x6/0/ \
  --form file=@label.zpl \
  --header "Accept: application/pdf" > label.pdf
```

### 6.2. PowerShell

```powershell
# PNG
Invoke-RestMethod `
  -Method Post `
  -Uri http://api.labelary.com/v1/printers/8dpmm/labels/4x6/0/ `
  -ContentType "application/x-www-form-urlencoded" `
  -InFile label.zpl `
  -OutFile label.png

# PDF
Invoke-RestMethod `
  -Method Post `
  -Uri http://api.labelary.com/v1/printers/8dpmm/labels/4x6/0/ `
  -ContentType "application/x-www-form-urlencoded" `
  -Headers @{"Accept" = "application/pdf"} `
  -InFile label.zpl `
  -OutFile label.pdf
```

### 6.3. C#

```csharp
byte[] zpl = Encoding.UTF8.GetBytes("^xa^cfa,50^fo100,100^fdHello World^fs^xz");

var request = (HttpWebRequest) WebRequest.Create(
    "http://api.labelary.com/v1/printers/8dpmm/labels/4x6/0/");
request.Method = "POST";
request.Accept = "application/pdf"; // omitir para PNG
request.ContentType = "application/x-www-form-urlencoded";
request.ContentLength = zpl.Length;

var requestStream = request.GetRequestStream();
requestStream.Write(zpl, 0, zpl.Length);
requestStream.Close();

try {
    var response = (HttpWebResponse) request.GetResponse();
    var responseStream = response.GetResponseStream();
    var fileStream = File.Create("label.pdf");
    responseStream.CopyTo(fileStream);
    responseStream.Close();
    fileStream.Close();
} catch (WebException e) {
    Console.WriteLine("Error: {0}", e.Status);
}
```

### 6.4. Python

```python
import requests
import shutil

zpl = '^xa^cfa,50^fo100,100^fdHello World^fs^xz'
url = 'http://api.labelary.com/v1/printers/8dpmm/labels/4x6/0/'
files = {'file': zpl}
headers = {'Accept': 'application/pdf'}  # omitir para PNG

response = requests.post(url, headers=headers, files=files, stream=True)

if response.status_code == 200:
    response.raw.decode_content = True
    with open('label.pdf', 'wb') as out_file:
        shutil.copyfileobj(response.raw, out_file)
else:
    print('Error: ' + response.text)
```

---

## 7. Funcionalidades Avançadas

### 7.1. Rotação de Etiquetas

Header: `X-Rotation`  
Valores: `0`, `90`, `180`, `270` (graus, sentido horário)

```bash
curl -H "X-Rotation: 90" ...
```

### 7.2. PDF com Múltiplas Etiquetas

Para incluir **todas** as etiquetas em um único PDF, **omita o índice** da URL:

```
# Uma etiqueta (índice 0)
http://api.labelary.com/v1/printers/8dpmm/labels/4x6/0/

# Todas as etiquetas (sem índice)
http://api.labelary.com/v1/printers/8dpmm/labels/4x6/
```

> ⚠️ Limite de **50 etiquetas** por requisição!

### 7.3. Tamanho e Orientação da Página PDF

| Header | Valores | Descrição |
|--------|---------|-----------|
| `X-Page-Size` | `Letter`, `Legal`, `A4`, `A5`, `A6` | Tamanho da página |
| `X-Page-Orientation` | `Portrait`, `Landscape` | Orientação |

### 7.4. Layout da Página PDF

| Header | Valores | Descrição |
|--------|---------|-----------|
| `X-Page-Layout` | `2x3` (colunas x linhas) | Múltiplas etiquetas por página |
| `X-Page-Align` | `Left`, `Right`, `Center`, `Justify` | Alinhamento horizontal |
| `X-Page-Vertical-Align` | `Top`, `Bottom`, `Center`, `Justify` | Alinhamento vertical |

### 7.5. Borda das Etiquetas no PDF

Header: `X-Label-Border`  
Valores: `Dashed` (padrão), `Solid`, `None`

### 7.6. Contagem de Etiquetas

O header de resposta `X-Total-Count` indica quantas etiquetas foram geradas.

### 7.7. Qualidade de Impressão

Header: `X-Quality`

| Valor | Descrição |
|-------|-----------|
| `Grayscale` | 8-bit grayscale (padrão, melhor visualização) |
| `Bitonal` | 1-bit monocromático (menor tamanho, impressoras de baixa densidade) |

> Apenas para PNG

### 7.8. Linting (Validação de ZPL)

Header: `X-Linter: On`

Warnings retornados no header `X-Warnings` (máx. 20), formato pipe-delimited:

```
byte_index|byte_size|command|param_number|message
```

Exemplo:
```
303|1|^GB|2|Value 1 is less than minimum value 3; used 3 instead|591|3|||Ignored unrecognized content
```

### 7.9. Extração de Dados (JSON)

Header: `Accept: application/json`

Retorna campos de texto em formato JSON:

```json
{
    "labels": [
        { 
            "fields": [ 
                { "x": 50, "y": 50, "data": "Field 1" }, 
                { "x": 50, "y": 150, "data": "Field 2" } 
            ] 
        }
    ]
}
```

---

## 8. Outras Funcionalidades

### 8.1. Converter Imagens para ZPL Graphics

```
POST http://api.labelary.com/v1/graphics
Content-Type: multipart/form-data
```

Parâmetro: `file` (imagem)

Formatos de saída: ZPL (padrão), JSON, EPL, IPL, DPL, SBPL, PCL 5, PCL 6

```bash
curl --request POST http://api.labelary.com/v1/graphics \
  --form file=@image.png > image.zpl
```

### 8.2. Converter Fontes TTF para ZPL

```
POST http://api.labelary.com/v1/fonts
Content-Type: multipart/form-data
```

| Parâmetro | Obrigatório | Descrição |
|-----------|-------------|-----------|
| `file` | Sim | Arquivo TTF |
| `path` | Não | Caminho na impressora (ex: `R:MYFONT.TTF`) |
| `name` | Não | Nome curto da fonte (`I`, `K`, `M`, `O`, `W`, `X`, `Y`, `Z`) |
| `chars` | Não | Subset de caracteres (ex: `0123456789`) |
| `unicodes` | Não | Ranges Unicode (ex: `0030-0039,0041-005A`) |

```bash
# Converter fonte completa
curl --request POST http://api.labelary.com/v1/fonts \
  --form file=@Montserrat-Bold.ttf > font.zpl

# Converter com subset (apenas maiúsculas)
curl --request POST http://api.labelary.com/v1/fonts \
  --form file=@Montserrat-Bold.ttf \
  --form name=Z \
  --form chars=ABCDEFGHIJKLMNOPQRSTUVWXYZ > font.zpl
```

> ⚠️ Verifique a licença da fonte antes de usar!

---

## Uso no ZPL2PDF

O ZPL2PDF utiliza a API Labelary quando o modo `--renderer labelary` é especificado:

```bash
# Usar Labelary para renderização
ZPL2PDF.exe -i etiqueta.txt -o output --renderer labelary

# Automático (tenta Labelary, fallback para BinaryKits)
ZPL2PDF.exe -i etiqueta.txt -o output --renderer auto
```

### Mapeamento de Parâmetros

| ZPL2PDF | Labelary |
|---------|----------|
| `-w 100 -u mm` | `width = 3.94` (100/25.4) |
| `-h 150 -u mm` | `height = 5.91` (150/25.4) |
| `-d 203` | `8dpmm` |
| `-d 300` | `12dpmm` |

### Batching Automático

Quando há mais de 50 etiquetas, o ZPL2PDF automaticamente:
1. Divide em lotes de 50 etiquetas
2. Faz múltiplas requisições à API
3. Mescla os PDFs resultantes em um único arquivo

---

## Links Úteis

- [Labelary Online Viewer](http://labelary.com/viewer.html)
- [Labelary API Documentation](http://labelary.com/service.html)
- [Postman Collection](http://labelary.com/postman.html)
- Suporte: support@labelary.com

