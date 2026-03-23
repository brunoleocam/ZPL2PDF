using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ZPL2PDF.Domain.Services;

namespace ZPL2PDF
{
    /// <summary>
    /// Renders ZPL labels using Labelary online API (returns PNG bytes).
    /// </summary>
    public class LabelaryRenderer : ILabelRenderer
    {
        private static readonly HttpClient HttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        private static readonly int[] AllowedDpmm = { 6, 8, 12, 24 };

        private readonly double _labelWidth;
        private readonly double _labelHeight;
        private readonly string _unit;
        private readonly int _dpi;

        public LabelaryRenderer(double labelWidth, double labelHeight, string unit, int dpi)
        {
            _labelWidth = labelWidth;
            _labelHeight = labelHeight;
            _unit = unit;
            _dpi = dpi;
        }

        // Extracted dimensions are already in millimeters.
        public LabelaryRenderer(LabelDimensions dimensions)
        {
            _labelWidth = dimensions.WidthMm;
            _labelHeight = dimensions.HeightMm;
            _unit = "mm";
            _dpi = dimensions.Dpi;
        }

        public (double width, double height, string unit, int dpi) GetDimensions()
            => (_labelWidth, _labelHeight, _unit, _dpi);

        public List<byte[]> RenderLabels(List<string> labels)
        {
            if (labels == null) throw new ArgumentNullException(nameof(labels));

            var images = new List<byte[]>();
            foreach (var label in labels)
            {
                images.Add(RenderSingleLabel(label));
            }
            return images;
        }

        /// <summary>
        /// Renders the entire ZPL template to a PDF directly from Labelary.
        /// This requests <c>application/pdf</c> and omits the label index so Labelary can include all labels.
        /// </summary>
        public byte[] RenderPdf(string zpl)
        {
            if (string.IsNullOrWhiteSpace(zpl))
                throw new ArgumentException("ZPL label content is required.", nameof(zpl));

            var dpmm = MapDpiToDpmm(_dpi);
            var widthInches = ConvertToInches(_labelWidth, _unit);
            var heightInches = ConvertToInches(_labelHeight, _unit);

            // For PDF requests, the index parameter is optional.
            // If omitted, Labelary returns a PDF with all labels (one page per label).
            var width = widthInches.ToString("0.###", CultureInfo.InvariantCulture);
            var height = heightInches.ToString("0.###", CultureInfo.InvariantCulture);

            var url = $"http://api.labelary.com/v1/printers/{dpmm}dpmm/labels/{width}x{height}/";

            using var content = new StringContent(zpl, Encoding.UTF8, "application/x-www-form-urlencoded");
            using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/pdf"));

            using var response = HttpClient.SendAsync(request).GetAwaiter().GetResult();
            var bytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                var msg = Encoding.UTF8.GetString(bytes);
                throw new InvalidOperationException($"Labelary request failed (HTTP {(int)response.StatusCode}): {msg}");
            }

            return bytes;
        }

        private byte[] RenderSingleLabel(string zpl)
        {
            if (string.IsNullOrWhiteSpace(zpl))
                throw new ArgumentException("ZPL label content is required.", nameof(zpl));

            var dpmm = MapDpiToDpmm(_dpi);
            var widthInches = ConvertToInches(_labelWidth, _unit);
            var heightInches = ConvertToInches(_labelHeight, _unit);

            // Labelary expects width/height in inches in the URL (any numeric value is allowed).
            var width = widthInches.ToString("0.###", CultureInfo.InvariantCulture);
            var height = heightInches.ToString("0.###", CultureInfo.InvariantCulture);

            // Labelary docs (service.html): POST http://api.labelary.com/v1/printers/{dpmm}dpmm/labels/{width}x{height}/{index}/
            var url = $"http://api.labelary.com/v1/printers/{dpmm}dpmm/labels/{width}x{height}/0/";

            using var content = new StringContent(zpl, Encoding.UTF8, "application/x-www-form-urlencoded");

            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("image/png"));

            using var response = HttpClient.SendAsync(request).GetAwaiter().GetResult();
            var bytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                // Error bodies are small (typically UTF-8 text).
                var msg = Encoding.UTF8.GetString(bytes);
                throw new InvalidOperationException($"Labelary request failed (HTTP {(int)response.StatusCode}): {msg}");
            }

            return bytes;
        }

        private static double ConvertToInches(double value, string unit)
        {
            if (unit.Equals("in", StringComparison.OrdinalIgnoreCase))
                return value;
            if (unit.Equals("cm", StringComparison.OrdinalIgnoreCase))
                return value / 2.54;
            if (unit.Equals("mm", StringComparison.OrdinalIgnoreCase))
                return value / 25.4;

            // Fallback: treat as mm.
            return value / 25.4;
        }

        private static int MapDpiToDpmm(int dpi)
        {
            // dpmm = dpi / 25.4, but Labelary only accepts 6/8/12/24.
            var dpmmRaw = dpi / 25.4;
            var best = AllowedDpmm[0];
            var bestDelta = Math.Abs(best - dpmmRaw);

            foreach (var candidate in AllowedDpmm)
            {
                var delta = Math.Abs(candidate - dpmmRaw);
                if (delta < bestDelta)
                {
                    best = candidate;
                    bestDelta = delta;
                }
            }

            return best;
        }
    }
}

