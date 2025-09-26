![License](https://img.shields.io/github/license/brunoleocam/ZPL2PDF)
![Release](https://img.shields.io/github/v/release/brunoleocam/ZPL2PDF)
![GitHub all releases](https://img.shields.io/github/downloads/brunoleocam/ZPL2PDF/total)
[![Tradução: Português-BR](https://img.shields.io/badge/Tradução-Português--BR-green.svg)](https://github.com/brunoleocam/ZPL2PDF/blob/main/docs/README.pt.md)

# ZPL to PDF Converter (ZPL2PDF) – Convert ZPL Labels to PDF

## Introduction

ZPL2PDF is a fast, offline ZPL to PDF converter. It reads Zebra Programming Language (ZPL/ZPL II) labels, renders them in memory, and exports a multi-page PDF where each page is a label. Built on top of the trusted [BinaryKits.Zpl](https://github.com/BinaryKits/BinaryKits.Zpl) stack, it is ideal for ERP/warehouse integrations, bulk label processing, and automated print pipelines.

Keywords: ZPL to PDF, Label to PDF, Zebra label converter, ZPL renderer, Windows CLI, offline conversion, ERP integration.

## Table of Contents

- [Installation](#installation)
- [Quickstart](#quickstart)
- [How to Use](#how-to-use)
- [Features](#features)
- [Execution Flow](#execution-flow)
- [Usage Examples](#usage-examples)
- [CLI Options](#cli-options)
- [Integration with Other Systems](#integration-with-other-systems)
- [Dependencies](#dependencies)
- [Other Languages](#other-languages)

## Building after clone

> [!CAUTION]
> If you clone or fork this repository, run the following commands to generate all required output files:
> 
> ```sh
> dotnet build
> dotnet publish -c Release
> ```
>
> This will automatically recreate all build outputs and dependencies.

## Installation

1. Download the installer [`ZPL2PDF.exe`](https://github.com/brunoleocam/ZPL2PDF/releases) from the **Releases** section.
2. Run the installer.
3. The program will be installed in:

```sh
C:\Program Files\ZPL2PDF
```

![ZPL to PDF converter UI example](Image/example_1.png)

## Quickstart

Convert a ZPL file to PDF in one command:

```powershell
"C:\Program Files\ZPL2PDF\ZPL2PDF.exe" -i "C:\Users\user\Documents\exemple_zpl.txt" -o "C:\Users\user\Documents" -n "exemple_zpl.pdf"
```

## How to Use

1. Open **Command Prompt (cmd)** or **PowerShell**.  
2. Navigate to the installation folder:

```sh
cd "C:\Program Files\ZPL2PDF"
```

3. Run the converter with the parameters:

```sh
.\ZPL2PDF.exe -i "C:\Users\user\Documents\exemple_zpl.txt" -n "exemple_zpl.pdf" -o "C:\Users\user\Documents\"
```

-  **-i** → Path to the input ZPL file
-  **-n** → Name of the output PDF file
-  **-o** → Output directory where the PDF will be saved

![Command-line usage example converting ZPL to PDF](Image/example_2.png)

In the example above, the file **exemple_zpl.pdf** will be generated inside the user’s **Documents** folder.

![Resulting PDF preview of converted ZPL labels](Image/example_3.png)

## Features

- **Accurate ZPL rendering (ZPL/ZPL II):**
   Renders ZPL labels to high-quality images fully in memory, no temp files.

- **Batch conversion to PDF:**
   Multiple labels become a multi-page PDF (one label per page).

- **Windows-friendly CLI:**
   Simple command-line interface suitable for scripts, ERPs, and services.

- **Configurable page size and density:**
   Control width, height, units, and print density to match printers.

## Execution Flow

1. **Receiving Parameters:**  
   The `Main` method analyzes the arguments received:
   - The `-i` parameter specifies the input file path.
   - The `-z` parameter specifies the ZPL content directly.
   - The `-o` parameter specifies the output folder path.
   - The `-n` parameter specifies the output file name (optional).
   - The `-w` and `-h` parameters specify the width and height of the label, respectively.
   - The `-d` parameter specifies the print density in dots per millimeter.
   - The `-u` parameter specifies the unit of measurement for width and height ("in", "cm", "mm").
   - The `-help` parameter displays the help message.

2. **Content reading**  
   The file is read using `LabelFileReader.ReadFile(inputFile)` or the ZPL content is used directly.

3. **Label separation:**  
   The `LabelFileReader.SplitLabels(fileContent)` method splits the content into individual labels, based on the `^XA` and `^XZ` delimiters.

4. **Image rendering**  
   The `LabelRenderer` class processes each label and renders images (in byte[]) with the defined dimensions and density.

5. **PDF generation:**  
   The PDF is generated using the `PdfGenerator.GeneratePdf(imageDataList, outputPdf)` class, where `outputPdf` is built in the specified output folder with the specified output file name or a default name.

## Usage Examples

1. **Specifying the Input and Output:** 

   - Reads the specified file and saves it in the specified output folder.

      ```sh
      ZPL2PDF.exe -i "C:\Path\to\input.txt" -o "C:\Path\to\output"
      ```

2. **Specifying the ZPL Content Directly:**

   - Uses the specified ZPL content and saves it in the specified output folder.

      ```sh
      ZPL2PDF.exe -z "^XA^FO50,50^ADN,36,20^FDHello, World!^FS^XZ" -o "C:\Path\to\output"
      ```

3. **Specifying the Output File Name:**

   - Reads the specified file, saves it in the specified output folder with the specified output file name.

      ```sh
      ZPL2PDF.exe -i "C:\Path\to\input.txt" -o "C:\Path\to\output" -n "output_filename.pdf"
      ```

4. **Specifying Width, Height, and Print Density:**

   - Reads the specified file, sets the width, height, and print density, and saves it in the specified output folder.

      ```sh
      ZPL2PDF.exe -i "C:\Path\to\input.txt" -o "C:\Path\to\output" -w 6 -h 12 -u "cm" -d 8
      ```

5. **Displaying the Help Message:**

   - Displays the help message with the description of the parameters.

      ```sh
      ZPL2PDF.exe -help
      ```

## Integration with Other Systems

The program can be compiled into an executable (ZPL2PDF.exe) and called from another application, such as an ERP, using functions to start processes (e.g., Process.Start in C#) and passing the necessary parameters.

## CLI Options

| Option | Description | Example |
|---|---|---|
| `-i` | Input file with ZPL content | `-i "C:\Path\to\input.txt"` |
| `-z` | Raw ZPL content passed directly | `-z "^XA^FO50,50...^XZ"` |
| `-o` | Output folder for the resulting PDF | `-o "C:\Path\to\output"` |
| `-n` | Output PDF filename | `-n "labels.pdf"` |
| `-w` | Label width (with `-u`) | `-w 4` |
| `-h` | Label height (with `-u`) | `-h 6` |
| `-u` | Unit for width/height: `in`, `cm`, `mm` | `-u "cm"` |
| `-d` | Print density in dots per millimeter | `-d 8` |
| `-help` | Show help | `-help` |

## Dependencies
   
   - **BinaryKits.Zpl:** For analyzing and rendering ZPL labels.
   - **PdfSharpCore:** For creating and manipulating the PDF file.

## Conclusion

ZPL2PDF was developed with a focus on modularization (separate classes for reading, rendering, and generating the PDF) and flexibility, allowing different input options and easy integration with other systems.

## Other Languages

- [Português](README.pt.md)
