using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZPL2PDF.Infrastructure.Rendering
{
    /// <summary>
    /// Online label renderer using Labelary API (http://labelary.com).
    /// Provides high-fidelity ZPL rendering that closely matches real Zebra printers.
    /// Generates PDF directly from the API for optimal quality (vectorial output).
    /// </summary>
    /// <remarks>
    /// API Limits (Free Tier):
    /// - 3 requests per second
    /// - 5,000 requests per day
    /// - 50 labels per request (batching is handled automatically)
    /// - 1 MB body size
    /// - 15 inches max dimensions
    /// </remarks>
    public class LabelaryRenderer : ILabelRenderer
    {
        private readonly HttpClient _httpClient;
        private readonly SemaphoreSlim _rateLimiter;
        private DateTime _lastRequestTime;
        private const string BaseUrl = "http://api.labelary.com/v1/printers";
        private const int MinRequestIntervalMs = 334; // ~3 requests per second
        private const int TimeoutSeconds = 60;
        private const int MaxLabelsPerRequest = 50; // Labelary API limit

        /// <summary>
        /// Gets the name of this renderer.
        /// </summary>
        public string Name => "Labelary";

        /// <summary>
        /// Indicates that Labelary can render directly to PDF (vectorial output).
        /// </summary>
        public bool CanRenderDirectToPdf => true;

        /// <summary>
        /// Initializes a new instance of the LabelaryRenderer.
        /// </summary>
        public LabelaryRenderer()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(TimeoutSeconds)
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();

            _rateLimiter = new SemaphoreSlim(1, 1);
            _lastRequestTime = DateTime.MinValue;
        }

        /// <summary>
        /// Checks if the Labelary API is available by making a test request.
        /// </summary>
        /// <returns>True if the API is reachable and responding, false otherwise.</returns>
        public bool IsAvailable()
        {
            try
            {
                // Simple test with minimal ZPL
                var testZpl = "^XA^XZ";
                var url = $"{BaseUrl}/8dpmm/labels/1x1/0/";

                using var content = new StringContent(testZpl, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = _httpClient.PostAsync(url, content).GetAwaiter().GetResult();

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Renders ZPL labels to images (PNG) synchronously.
        /// For PDF output, use RenderLabelsToPdfAsync.
        /// </summary>
        /// <param name="labels">List of ZPL label strings to render.</param>
        /// <param name="widthMm">Label width in millimeters.</param>
        /// <param name="heightMm">Label height in millimeters.</param>
        /// <param name="dpi">Print density in DPI.</param>
        /// <returns>List of rendered images as byte arrays.</returns>
        public List<byte[]> RenderLabels(List<string> labels, double widthMm, double heightMm, int dpi)
        {
            return RenderLabelsAsync(labels, widthMm, heightMm, dpi).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Renders ZPL labels to images (PNG) asynchronously using Labelary API.
        /// </summary>
        /// <param name="labels">List of ZPL label strings to render.</param>
        /// <param name="widthMm">Label width in millimeters.</param>
        /// <param name="heightMm">Label height in millimeters.</param>
        /// <param name="dpi">Print density in DPI.</param>
        /// <returns>Task containing list of rendered images as byte arrays.</returns>
        public async Task<List<byte[]>> RenderLabelsAsync(List<string> labels, double widthMm, double heightMm, int dpi)
        {
            var results = new List<byte[]>();
            var (dpmm, widthInches, heightInches) = PrepareApiParameters(widthMm, heightMm, dpi);

            foreach (var zpl in labels)
            {
                var imageData = await RenderSingleAsync(zpl, widthInches, heightInches, dpmm, "image/png");
                results.Add(imageData);
            }

            return results;
        }

        /// <summary>
        /// Renders ZPL labels directly to PDF using Labelary API.
        /// This generates a vectorial PDF with optimal quality.
        /// Handles batching automatically for more than 50 labels.
        /// </summary>
        /// <param name="labels">List of ZPL label strings to render.</param>
        /// <param name="widthMm">Label width in millimeters.</param>
        /// <param name="heightMm">Label height in millimeters.</param>
        /// <param name="dpi">Print density in DPI.</param>
        /// <returns>List of PDF byte arrays (one per batch of up to 50 labels).</returns>
        public List<byte[]> RenderLabelsToPdf(List<string> labels, double widthMm, double heightMm, int dpi)
        {
            return RenderLabelsToPdfAsync(labels, widthMm, heightMm, dpi).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Renders ZPL labels directly to PDF asynchronously using Labelary API.
        /// This generates a vectorial PDF with optimal quality.
        /// Handles batching automatically for more than 50 labels.
        /// </summary>
        /// <param name="labels">List of ZPL label strings to render.</param>
        /// <param name="widthMm">Label width in millimeters.</param>
        /// <param name="heightMm">Label height in millimeters.</param>
        /// <param name="dpi">Print density in DPI.</param>
        /// <returns>Task containing list of PDF byte arrays (one per batch of up to 50 labels).</returns>
        public async Task<List<byte[]>> RenderLabelsToPdfAsync(List<string> labels, double widthMm, double heightMm, int dpi)
        {
            var results = new List<byte[]>();
            var (dpmm, widthInches, heightInches) = PrepareApiParameters(widthMm, heightMm, dpi);

            // Process labels in batches of 50 (Labelary API limit)
            var batches = SplitIntoBatches(labels, MaxLabelsPerRequest);

            foreach (var batch in batches)
            {
                // Combine all ZPL labels in the batch into a single string
                var combinedZpl = string.Join("\n", batch);
                var pdfData = await RenderBatchToPdfAsync(combinedZpl, widthInches, heightInches, dpmm);
                results.Add(pdfData);
            }

            return results;
        }

        /// <summary>
        /// Renders a batch of ZPL labels to a single PDF.
        /// </summary>
        private async Task<byte[]> RenderBatchToPdfAsync(string combinedZpl, double widthInches, double heightInches, int dpmm)
        {
            return await RenderSingleAsync(combinedZpl, widthInches, heightInches, dpmm, "application/pdf");
        }

        /// <summary>
        /// Renders ZPL content using the Labelary API with rate limiting.
        /// </summary>
        private async Task<byte[]> RenderSingleAsync(string zpl, double widthInches, double heightInches, int dpmm, string acceptHeader)
        {
            await _rateLimiter.WaitAsync();

            try
            {
                // Enforce rate limiting (min 334ms between requests)
                var timeSinceLastRequest = DateTime.Now - _lastRequestTime;
                if (timeSinceLastRequest.TotalMilliseconds < MinRequestIntervalMs)
                {
                    var delayMs = MinRequestIntervalMs - (int)timeSinceLastRequest.TotalMilliseconds;
                    await Task.Delay(delayMs);
                }

                // Build API URL: /printers/{dpmm}dpmm/labels/{width}x{height}/
                // Note: For PDF with multiple labels, we don't use the index
                // Use InvariantCulture to ensure decimal point (not comma) in URL
                var widthStr = widthInches.ToString("F2", CultureInfo.InvariantCulture);
                var heightStr = heightInches.ToString("F2", CultureInfo.InvariantCulture);
                var url = $"{BaseUrl}/{dpmm}dpmm/labels/{widthStr}x{heightStr}/";

                using var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(acceptHeader));
                request.Content = new StringContent(zpl, Encoding.UTF8, "application/x-www-form-urlencoded");

                var response = await _httpClient.SendAsync(request);

                _lastRequestTime = DateTime.Now;

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new LabelaryException($"Labelary API error ({response.StatusCode}): {errorContent}");
                }

                return await response.Content.ReadAsByteArrayAsync();
            }
            finally
            {
                _rateLimiter.Release();
            }
        }

        /// <summary>
        /// Prepares API parameters by converting dimensions and DPI.
        /// </summary>
        private (int dpmm, double widthInches, double heightInches) PrepareApiParameters(double widthMm, double heightMm, int dpi)
        {
            // Convert DPI to DPMM for Labelary API
            int dpmm = dpi switch
            {
                152 => 6,
                203 => 8,
                300 => 12,
                600 => 24,
                _ => 8 // Default to 8 dpmm (203 DPI)
            };

            // Convert mm to inches for Labelary API
            double widthInches = widthMm / 25.4;
            double heightInches = heightMm / 25.4;

            // Validate dimensions (max 15 inches)
            if (widthInches > 15 || heightInches > 15)
            {
                throw new ArgumentException($"Dimensions exceed Labelary limit of 15 inches. Width: {widthInches:F2}in, Height: {heightInches:F2}in");
            }

            return (dpmm, widthInches, heightInches);
        }

        /// <summary>
        /// Splits a list into batches of specified size.
        /// </summary>
        private static List<List<T>> SplitIntoBatches<T>(List<T> source, int batchSize)
        {
            var batches = new List<List<T>>();
            for (int i = 0; i < source.Count; i += batchSize)
            {
                var batch = source.GetRange(i, Math.Min(batchSize, source.Count - i));
                batches.Add(batch);
            }
            return batches;
        }

        /// <summary>
        /// Releases resources used by the renderer.
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
            _rateLimiter?.Dispose();
        }
    }

    /// <summary>
    /// Exception thrown when Labelary API returns an error.
    /// </summary>
    public class LabelaryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the LabelaryException.
        /// </summary>
        /// <param name="message">Error message describing the API failure.</param>
        public LabelaryException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LabelaryException with inner exception.
        /// </summary>
        /// <param name="message">Error message describing the API failure.</param>
        /// <param name="innerException">The inner exception that caused this error.</param>
        public LabelaryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
