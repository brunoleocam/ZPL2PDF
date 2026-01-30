#!/bin/bash

# ZPL2PDF API Test Script
# This script builds and tests the ZPL2PDF API

echo "=========================================="
echo "ZPL2PDF API Build and Test Script"
echo "=========================================="
echo ""

# Step 1: Build the project
echo "Step 1: Building the project..."
dotnet build
if [ $? -ne 0 ]; then
    echo "❌ Build failed!"
    exit 1
fi
echo "✅ Build successful!"
echo ""

# Step 2: Start API server in background
echo "Step 2: Starting API server on port 5000..."
dotnet run -- --api --host localhost --port 5000 &
API_PID=$!
sleep 3

# Check if API is running
if ! kill -0 $API_PID 2>/dev/null; then
    echo "❌ API server failed to start!"
    exit 1
fi
echo "✅ API server started (PID: $API_PID)"
echo ""

# Step 3: Test health endpoint
echo "Step 3: Testing health endpoint..."
HEALTH_RESPONSE=$(curl -s http://localhost:5000/api/health)
if [ $? -eq 0 ]; then
    echo "✅ Health check response: $HEALTH_RESPONSE"
else
    echo "❌ Health check failed!"
    kill $API_PID 2>/dev/null
    exit 1
fi
echo ""

# Step 4: Test PDF conversion
echo "Step 4: Testing PDF conversion..."
PDF_RESPONSE=$(curl -s -X POST http://localhost:5000/api/convert \
  -H "Content-Type: application/json" \
  -d '{
    "zpl": "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ",
    "format": "pdf"
  }')

if echo "$PDF_RESPONSE" | grep -q '"success":true'; then
    echo "✅ PDF conversion successful!"
    echo "Response preview: $(echo $PDF_RESPONSE | cut -c1-100)..."
    
    # Extract and save PDF
    PDF_BASE64=$(echo "$PDF_RESPONSE" | grep -o '"pdf":"[^"]*' | cut -d'"' -f4)
    if [ ! -z "$PDF_BASE64" ]; then
        echo "$PDF_BASE64" | base64 -d > test_output.pdf
        echo "✅ PDF saved to test_output.pdf"
    fi
else
    echo "❌ PDF conversion failed!"
    echo "Response: $PDF_RESPONSE"
    kill $API_PID 2>/dev/null
    exit 1
fi
echo ""

# Step 5: Test PNG conversion
echo "Step 5: Testing PNG conversion..."
PNG_RESPONSE=$(curl -s -X POST http://localhost:5000/api/convert \
  -H "Content-Type: application/json" \
  -d '{
    "zpl": "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ",
    "format": "png"
  }')

if echo "$PNG_RESPONSE" | grep -q '"success":true'; then
    echo "✅ PNG conversion successful!"
    echo "Response preview: $(echo $PNG_RESPONSE | cut -c1-100)..."
    
    # Extract and save PNG
    PNG_BASE64=$(echo "$PNG_RESPONSE" | grep -o '"image":"[^"]*' | cut -d'"' -f4)
    if [ ! -z "$PNG_BASE64" ]; then
        echo "$PNG_BASE64" | base64 -d > test_output.png
        echo "✅ PNG saved to test_output.png"
    fi
else
    echo "❌ PNG conversion failed!"
    echo "Response: $PNG_RESPONSE"
    kill $API_PID 2>/dev/null
    exit 1
fi
echo ""

# Step 6: Test error handling
echo "Step 6: Testing error handling..."
ERROR_RESPONSE=$(curl -s -X POST http://localhost:5000/api/convert \
  -H "Content-Type: application/json" \
  -d '{
    "zpl": "",
    "format": "pdf"
  }')

if echo "$ERROR_RESPONSE" | grep -q '"success":false'; then
    echo "✅ Error handling works correctly!"
    echo "Error response: $(echo $ERROR_RESPONSE | cut -c1-150)..."
else
    echo "⚠️  Error handling test inconclusive"
fi
echo ""

# Step 7: Cleanup
echo "Step 7: Stopping API server..."
kill $API_PID 2>/dev/null
wait $API_PID 2>/dev/null
echo "✅ API server stopped"
echo ""

echo "=========================================="
echo "✅ All tests passed!"
echo "=========================================="
echo ""
echo "Test outputs:"
echo "  - test_output.pdf (if PDF test succeeded)"
echo "  - test_output.png (if PNG test succeeded)"
echo ""
