# Installing Invictus Framework

This guide covers the complete installation process for Invictus Framework.

## Prerequisites

Before installing Invictus Framework, ensure you have:

- Azure subscription with appropriate permissions
- Azure DevOps organization
- Service Principal with Contributor access
- .NET 8.0 SDK or later

## Build Pipeline Setup

### Step 1: Create Build Pipeline

1. Navigate to Azure DevOps
2. Create a new pipeline from the provided YAML template
3. Configure the following variables:
   - `AzureSubscription`: Your Azure service connection
   - `ResourceGroup`: Target resource group name
   - `Location`: Azure region (e.g., westeurope)

### Step 2: Configure Service Connection

```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

steps:
  - task: AzureCLI@2
    inputs:
      azureSubscription: $(AzureSubscription)
      scriptType: 'bash'
      scriptLocation: 'inlineScript'
      inlineScript: |
        az group create --name $(ResourceGroup) --location $(Location)
```

## Release Pipeline Setup

### Step 1: Create Release Pipeline

1. Create a new release pipeline
2. Add artifact from build pipeline
3. Add deployment stage

### Step 2: Deploy Resources

The release pipeline will:
- Deploy Azure resources using Bicep/ARM templates
- Configure Container Apps for Framework components
- Set up monitoring and logging

## Verification

After deployment, verify the installation:

```bash
az containerapp list --resource-group <ResourceGroup>
```

## Common Issues

### Issue: Deployment Fails

**Solution**: Check service principal permissions and subscription limits.

### Issue: Container App Not Starting

**Solution**: Review logs using `az containerapp logs show`

## Next Steps

- Configure [Framework Components](./framework-components.md)
- Set up [Transco Component](./transco-component.md)
- Enable [Monitoring](./monitoring.md)
