using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ZPL2PDF.Application.Services;

namespace ZPL2PDF
{
    /// <summary>
    /// TCP server that accepts ZPL over the network and converts to PDF (virtual printer).
    /// </summary>
    public class TcpPrinterServer
    {
        private readonly int _port;
        private readonly string _outputFolder;
        private readonly ConversionService _conversionService;
        private TcpListener? _listener;
        private volatile bool _running;
        private const int ReadTimeoutMs = 30000;
        private const int DefaultDpi = 203;
        private const string DefaultUnit = "mm";

        public TcpPrinterServer(int port, string outputFolder)
        {
            _port = port;
            _outputFolder = outputFolder ?? throw new ArgumentNullException(nameof(outputFolder));
            _conversionService = new ConversionService();
        }

        /// <summary>
        /// Starts the TCP server and blocks until stopped.
        /// </summary>
        public void Run(CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(_outputFolder))
            {
                Directory.CreateDirectory(_outputFolder);
            }

            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            _running = true;

            try
            {
                while (_running && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        using var client = _listener.AcceptTcpClient();
                        client.ReceiveTimeout = ReadTimeoutMs;
                        client.SendTimeout = 5000;
                        ProcessClient(client);
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                    catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"TCP server client error: {ex.Message}");
                    }
                }
            }
            finally
            {
                _running = false;
                _listener?.Stop();
            }
        }

        /// <summary>
        /// Stops the server (stops accepting new connections).
        /// </summary>
        public void Stop()
        {
            _running = false;
            _listener?.Stop();
        }

        private void ProcessClient(TcpClient client)
        {
            using var stream = client.GetStream();
            using var ms = new MemoryStream();
            var buffer = new byte[4096];
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            if (ms.Length == 0)
            {
                return;
            }

            string zplContent = Encoding.UTF8.GetString(ms.ToArray());
            if (string.IsNullOrWhiteSpace(zplContent))
            {
                return;
            }

            try
            {
                var imageDataList = _conversionService.ConvertWithExtractedDimensions(zplContent, DefaultUnit, DefaultDpi);
                if (imageDataList == null || imageDataList.Count == 0)
                {
                    return;
                }

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileName = $"ZPL2PDF_TCP_{timestamp}.pdf";
                var outputPath = Path.Combine(_outputFolder, fileName);
                PdfGenerator.GeneratePdf(imageDataList, outputPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Conversion error: {ex.Message}");
            }
        }
    }
}
