using System;
using System.Collections.Generic;
using ZPL2PDF.Application.Interfaces;

namespace ZPL2PDF.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of IConversionService for testing
    /// </summary>
    public class MockConversionService : IConversionService
    {
        public List<byte[]> ConvertWithExplicitDimensions(string zplContent, double width, double height, string unit, int dpi)
        {
            // Mock implementation - returns fake PDF data
            return new List<byte[]> { GenerateMockPdfData(zplContent, width, height, unit, dpi) };
        }

        public List<byte[]> ConvertWithExtractedDimensions(string zplContent, string unit, int dpi)
        {
            // Mock implementation - returns fake PDF data
            return new List<byte[]> { GenerateMockPdfData(zplContent, 100, 150, unit, dpi) };
        }

        public List<byte[]> Convert(string zplContent, double explicitWidth, double explicitHeight, string unit, int dpi)
        {
            // Mock implementation - returns fake PDF data
            return new List<byte[]> { GenerateMockPdfData(zplContent, explicitWidth, explicitHeight, unit, dpi) };
        }

        private byte[] GenerateMockPdfData(string zplContent, double width, double height, string unit, int dpi)
        {
            // Generate fake PDF data for testing
            var mockPdfContent = $"Mock PDF for ZPL: {zplContent.Substring(0, Math.Min(50, zplContent.Length))}...";
            var mockPdfBytes = System.Text.Encoding.UTF8.GetBytes(mockPdfContent);
            
            // Add some metadata
            var metadata = $"Dimensions: {width}x{height} {unit} @ {dpi} DPI";
            var metadataBytes = System.Text.Encoding.UTF8.GetBytes(metadata);
            
            var result = new byte[mockPdfBytes.Length + metadataBytes.Length + 10];
            Array.Copy(mockPdfBytes, 0, result, 0, mockPdfBytes.Length);
            Array.Copy(metadataBytes, 0, result, mockPdfBytes.Length, metadataBytes.Length);
            
            return result;
        }
    }
}
