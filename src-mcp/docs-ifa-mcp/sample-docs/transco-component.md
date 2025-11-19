# Transco Component

The Transco (Transformation and Conversion) component provides message transformation capabilities for the Invictus Framework.

## Overview

Transco allows you to:
- Transform message formats (JSON, XML, CSV)
- Apply XSLT transformations
- Validate message schemas
- Route messages based on content

## Configuration

### Basic Configuration

```json
{
  "name": "transco-endpoint",
  "type": "transco",
  "transformationType": "xslt",
  "configuration": {
    "xsltPath": "/transformations/order-transform.xslt",
    "inputFormat": "xml",
    "outputFormat": "json"
  }
}
```

### Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `name` | string | Yes | Unique endpoint identifier |
| `transformationType` | string | Yes | Type: `xslt`, `liquid`, `jq` |
| `inputFormat` | string | Yes | Input format: `xml`, `json`, `csv` |
| `outputFormat` | string | Yes | Output format: `xml`, `json`, `csv` |
| `xsltPath` | string | Conditional | Required for XSLT transformations |

## Example Usage

### XML to JSON Transformation

**Input:**
```xml
<?xml version="1.0"?>
<order>
  <id>12345</id>
  <customer>John Doe</customer>
  <amount>99.99</amount>
</order>
```

**Output:**
```json
{
  "orderId": "12345",
  "customerName": "John Doe",
  "totalAmount": 99.99
}
```

### XSLT Transformation

Create an XSLT file:

```xml
<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/order">
    <result>
      <orderId><xsl:value-of select="id"/></orderId>
      <customer><xsl:value-of select="customer"/></customer>
    </result>
  </xsl:template>
</xsl:stylesheet>
```

## Request/Response

### Request Format

```http
POST /api/transco/transform
Content-Type: application/xml

<order>...</order>
```

### Response Format

```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "orderId": "12345",
  "customerName": "John Doe"
}
```

## Error Handling

Common error codes:

- `400`: Invalid input format
- `422`: Transformation failed
- `500`: Internal server error

## Best Practices

1. **Validate Input**: Always validate messages before transformation
2. **Cache Transformations**: Reuse compiled XSLT for better performance
3. **Handle Large Messages**: Use streaming for files > 10MB
4. **Monitor Performance**: Track transformation times

## See Also

- [XML/JSON Converter](./xml-json-converter.md)
- [Message Validation](./validation.md)
- [Performance Tuning](./performance.md)
