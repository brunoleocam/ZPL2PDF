using System;
using System.IO;
using BinaryKits.Zpl.Viewer;
using BinaryKits.Zpl.Viewer.ElementDrawers;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

namespace ZPL2PDF {
    /// <summary>
    /// Programa principal de conversão de ZPL para PDF.
    /// 
    /// O programa suporta duas formas de entrada:
    /// 1. Conteúdo direto: Se o primeiro argumento for "-c", o segundo argumento será tratado como o conteúdo do arquivo.
    /// 2. Caminho do arquivo: Caso contrário, o primeiro argumento é interpretado como o caminho do arquivo de entrada. Se nenhum parâmetro for fornecido, utiliza "C:\input.txt" como padrão.
    /// 
    /// O caminho de saída para o PDF pode ser especificado como:
    /// - Terceiro parâmetro se no modo "-c", ou segundo parâmetro se no modo de caminho.
    /// Caso não seja informado, o PDF será salvo na pasta Downloads do usuário.
    /// O nome do arquivo PDF será o mesmo nome do arquivo de input (ou "input.pdf" no modo "-c").
    /// </summary>
    class Program {
        static void Main(string[] args) {
            try {
                string fileContent = "";
                string inputFileNameWithoutExtension = "input";
                string outputFolder = "";
                
                // Modo de conteúdo direto (-c) ou caminho de arquivo
                if (args.Length > 0) {
                    if (args[0].Equals("-c", StringComparison.OrdinalIgnoreCase)) {
                        // Modo de conteúdo direto: primeiro argumento é "-c", segundo é o conteúdo
                        if (args.Length < 2) {
                            throw new ArgumentException("Parâmetro '-c' informado, mas nenhum conteúdo foi passado.");
                        }
                        fileContent = args[1];
                        inputFileNameWithoutExtension = "input";
                        // Se existir terceiro parâmetro, trata como caminho de output
                        outputFolder = args.Length > 2 ? args[2] : "";
                    } else {
                        // Modo de caminho de arquivo: primeiro parâmetro é o caminho do input
                        string inputFile = args[0];
                        inputFileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFile);
                        fileContent = LabelFileReader.ReadFile(inputFile);
                        // Se existir segundo parâmetro, trata como caminho de output
                        outputFolder = args.Length > 1 ? args[1] : "";
                    }
                }
                else {
                    // Sem argumentos: usa padrão
                    string inputFile = @"C:\input.txt";
                    inputFileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFile);
                    fileContent = LabelFileReader.ReadFile(inputFile);
                    outputFolder = "";
                }

                var labels = LabelFileReader.SplitLabels(fileContent);
                var renderer = new LabelRenderer();
                var imageDataList = renderer.RenderLabels(labels);

                // Define o caminho de output: se não informado, utiliza a pasta Downloads do usuário
                if (string.IsNullOrEmpty(outputFolder)) {
                    outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                }
                // Garante que a pasta de output exista
                if (!Directory.Exists(outputFolder)) {
                    Directory.CreateDirectory(outputFolder);
                }
                string outputPdf = Path.Combine(outputFolder, inputFileNameWithoutExtension + ".pdf");
                PdfGenerator.GeneratePdf(imageDataList, outputPdf);
                Console.WriteLine($"PDF gerado com sucesso em {outputPdf}.");
            }
            catch (Exception ex) {
                Console.Error.WriteLine($"Erro durante a execução: {ex.Message}");
            }
        }
    }
}