# Invictus Dashboard Guide

The Invictus Dashboard provides a centralized interface for monitoring and managing your integration platform.

## Overview

Key features:
- Real-time monitoring of integrations
- Message flow visualization
- Error tracking and alerting
- Performance metrics
- Configuration management

## Installation

### Prerequisites

- Azure Container Apps environment
- Azure SQL Database (for state storage)
- Application Insights (for telemetry)

### Deployment Steps

1. **Create Azure Resources**

```bash
az containerapp env create \
  --name invictus-dashboard-env \
  --resource-group invictus-rg \
  --location westeurope
```

2. **Deploy Dashboard Container**

```bash
az containerapp create \
  --name invictus-dashboard \
  --resource-group invictus-rg \
  --environment invictus-dashboard-env \
  --image invictus.azurecr.io/dashboard:latest \
  --target-port 80 \
  --ingress external
```

3. **Configure Database**

Set connection string in environment variables:

```bash
az containerapp update \
  --name invictus-dashboard \
  --resource-group invictus-rg \
  --set-env-vars "ConnectionStrings__Database=Server=..."
```

## Features

### Monitoring

View real-time integration metrics:
- Messages processed
- Success/failure rates
- Average processing time
- Active integrations

### Message Flow

Visualize message paths:
- Track message journey
- Identify bottlenecks
- Debug transformation issues

### Alerts

Configure alerts for:
- High error rates
- Performance degradation
- System health issues

## Configuration

### User Management

Add users via Azure AD integration:

```json
{
  "AzureAd": {
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "RequiredRoles": ["Integration.Admin", "Integration.Viewer"]
  }
}
```

### Dashboard Settings

Customize appearance and behavior:

```json
{
  "Dashboard": {
    "RefreshInterval": 30,
    "RetentionDays": 90,
    "TimeZone": "UTC"
  }
}
```

## Best Practices

1. **Security**: Use managed identities for Azure resource access
2. **Scaling**: Configure auto-scaling for high-traffic scenarios
3. **Backup**: Regular backups of configuration database
4. **Monitoring**: Enable Application Insights for diagnostics

## Troubleshooting

### Dashboard Not Loading

- Check container logs: `az containerapp logs show`
- Verify network connectivity
- Confirm database connection

### Slow Performance

- Review Application Insights metrics
- Check database query performance
- Consider scaling up container resources

## See Also

- [Installation Guide](./installation.md)
- [Security Configuration](./security.md)
- [Monitoring Best Practices](./monitoring.md)
