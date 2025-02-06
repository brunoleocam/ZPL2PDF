# Documentação - ETQ2PDF

## Introdução

Este projeto é desenvolvido em cima da biblioteca [BinaryKits.Zpl](https://github.com/BinaryKits/BinaryKits.Zpl).

O ETQ2PDF é um programa para conversão de etiquetas no formato ZPL para um arquivo PDF. Ele processa as etiquetas, renderiza imagens em memória e compila essas imagens em um PDF, onde cada página contém uma etiqueta.

## Funcionalidades

- **Entrada com Dupla Opção:**  
  Permite passar parâmetros de duas formas:
  - **Caminho do Arquivo:** Se o conteúdo não for passado explicitamente, o programa interpreta o primeiro parâmetro como o caminho do arquivo de entrada.
  - **Conteúdo Direto:** Se o primeiro parâmetro for `-c`, o segundo parâmetro é considerado o conteúdo do arquivo.

- **Processamento das Etiquetas:**  
  Utiliza a classe `LabelFileReader` para ler o arquivo e separar as etiquetas com base nos delimitadores `^XA` e `^XZ`.

- **Renderização em Memória:**  
  A classe `LabelRenderer` analisa o conteúdo ZPL e renderiza as etiquetas em imagens, mantendo os dados em memória sem necessidade de salvamento temporário.

- **Geração do PDF:**  
  A classe `PdfGenerator` gera um PDF onde cada imagem é adicionada em uma página. O arquivo PDF é salvo na pasta Downloads do usuário, usando o mesmo nome base do arquivo de entrada (com extensão `.pdf`). Caso um caminho de saída seja especificado, o PDF será salvo nesse caminho.

## Fluxo de Execução

1. **Recebimento dos Parâmetros:**  
   O método `Main` analisa os argumentos recebidos:
   - Se o primeiro argumento for `-c`, o segundo é usado como conteúdo.
   - Caso contrário, o primeiro argumento é interpretado como o caminho para o arquivo de entrada. Se nenhum parâmetro for fornecido, utiliza "C:\input.txt" como padrão.
   - O terceiro parâmetro (ou segundo no modo `-c`) é opcional e define o diretório de saída. Se não for informado, utiliza a pasta Downloads do usuário.

2. **Leitura do Conteúdo:**  
   Se o arquivo for informado, é lido através de `LabelFileReader.ReadFile(inputFile)`.

3. **Separação das Etiquetas:**  
   O método `LabelFileReader.SplitLabels(fileContent)` divide o conteúdo em etiquetas individuais, com base nos delimitadores `^XA` e `^XZ`.

4. **Renderização das Imagens:**  
   A classe `LabelRenderer` processa cada etiqueta e renderiza imagens (em byte[]) com as dimensões e densidade definidas.

5. **Geração do PDF:**  
   O PDF é gerado utilizando a classe `PdfGenerator.GeneratePdf(imageDataList, outputPdf)`, onde `outputPdf` é construído na pasta Downloads do usuário ou no caminho especificado, com o mesmo nome base do arquivo de entrada.

## Exemplo de Uso

1. **Padrão, sem informar nada:**
   - Lê o arquivo `C:\input.txt` e salva em Downloads.

      ~~~sh
      ETQ2PDF.exe
      ~~~

2. **Especificando a Entrada:**

   2.1 **Informando o caminho:**

      - Lê o arquivo especificado e salva em Downloads.

         ~~~sh
         ETQ2PDF.exe "C:\Caminho\para\entrada.txt"
         ~~~

   2.2 **Informando o conteúdo:**

      - Utiliza o conteúdo passado diretamente e salva em Downloads.

         ~~~sh
         ETQ2PDF.exe -c "conteúdo do arquivo em forma de string"
         ~~~

3. **Especificando a Saída:**

   - Informando no 2º parâmetro (ou 3º no modo `-c`), caso não informe, salvará em Downloads.

   3.1 **Com caminho de entrada:**

      ~~~sh
      ETQ2PDF.exe "C:\Caminho\para\entrada.txt" "C:\Caminho\para\saida"
      ~~~

   3.2 **Com conteúdo direto:**

      ~~~sh
      ETQ2PDF.exe -c "conteúdo do arquivo em forma de string" "C:\Caminho\para\saida"
      ~~~


## Integração com Outros Sistemas

O programa pode ser compilado em um executável (ETQ2PDF.exe) e chamado a partir de outro aplicativo, como um ERP, utilizando funções para iniciar processos (por exemplo, `Process.Start` em C#) e passando os parâmetros necessários.

## Dependências

- **BinaryKits.Zpl:** Para análise e renderização das etiquetas ZPL.
- **PdfSharpCore:** Para criação e manipulação do arquivo PDF.

## Conclusão

O ETQ2PDF foi desenvolvido com foco na modularização (classes separadas para leitura, renderização e geração do PDF) e flexibilidade, permitindo diferentes opções de entrada e fácil integração com outros sistemas.