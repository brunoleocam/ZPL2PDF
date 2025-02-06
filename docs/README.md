# Documentation - ETQ2PDF

## Introduction

This project is built on top of the [BinaryKits.Zpl](https://github.com/BinaryKits/BinaryKits.Zpl) library.

ETQ2PDF is a project for converting labels in ZPL format into a PDF file. The program processes the labels, renders images in memory and compiles these images into a PDF, where each page contains a label.


## Features

- **Dual Input Option:** 
   Allows passing parameters in two ways:
  - **File Path:** The program interprets the first parameter as the path to the input file.
  - **Direct Content:** If the first parameter is `-c`, the second parameter is considered the file content.

- **Label Processing:** 
   Uses the `LabelFileReader` class to read the file and separate the labels based on the `^XA` and `^XZ` delimiters.

- **In-Memory Rendering:** 
   The `LabelRenderer` class analyzes the ZPL content and renders the labels into images, keeping the data in memory without the need for temporary storage.

- **PDF Generation:** 
   The `PdfGenerator` class generates a PDF where each image is added to a page. The PDF file is saved in the user's Downloads folder, using the same base name as the input file (with `.pdf` extension). If an output path is specified, the PDF will be saved to that path.

## Execution Flow

1. **Receiving Parameters:**  
   The `Main` method analyzes the arguments received:
   - If the first argument is `-c`, the second is used as the content.
   - Otherwise, the first argument is interpreted as the path to the input file. If no parameter is supplied, it uses “C:\input.txt” as the default.
   - The third parameter (or second in `-c` mode) is optional and defines the output directory. If not given, it uses the user's Downloads folder.

2. **Content reading**  
   If the file is entered, it is read using `LabelFileReader.ReadFile(inputFile)`.

3. **Label separation:**  
   The `LabelFileReader.SplitLabels(fileContent)` method splits the content into individual labels, based on the `^XA` and `^XZ` delimiters.

4. **Image rendering**  
   The `LabelRenderer` class processes each label and renders images (in byte[]) with the defined dimensions and density.

5. **PDF generation:**  
   The PDF is generated using the `PdfGenerator.GeneratePdf(imageDataList, outputPdf)` class, where `outputPdf` is built in the user's Downloads folder or in the specified path, with the same base name as the input file.

## Usage Examples

1. **Default, without specifying anything:**

   - Reads the file `C:\input.txt` and saves it in Downloads.

      ```sh
      ETQ2PDF.exe
      ```

2. **Specifying the Input:** 

   2.1 **Specifying the path:**

   - Reads the specified file and saves it in Downloads.

      ```sh
      ETQ2PDF.exe "C:\Path\to\input.txt"
      ```

   2.2 **Specifying the content:**

   - Uses the directly passed content and saves it in Downloads.

      ```sh
      ETQ2PDF.exe -c "file content as a string"
      ```

3. **Specifying the Output:**

   - Specifying in the 2nd parameter (or 3rd in -c mode), if not specified, it will save in Downloads.

   3.1 **With input path:**

      ```sh
      ETQ2PDF.exe "C:\Path\to\input.txt" "C:\Path\to\output"
      ```

   3.2 **With direct content:**

      ```sh
      ETQ2PDF.exe -c "file content as a string" "C:\Path\to\output"
      ```


## Integration with Other Systems

The program can be compiled into an executable (ETQ2PDF.exe) and called from another application, such as an ERP, using functions to start processes (e.g., Process.Start in C#) and passing the necessary parameters.

## Dependencies
   
   - **BinaryKits.Zpl:** For analyzing and rendering ZPL labels.
   - **PdfSharpCore:** For creating and manipulating the PDF file.

## Conclusion

ETQ2PDF was developed with a focus on modularization (separate classes for reading, rendering, and generating the PDF) and flexibility, allowing different input options and easy integration with other systems.

## Other Languages

- [Português](README.pt.md)
