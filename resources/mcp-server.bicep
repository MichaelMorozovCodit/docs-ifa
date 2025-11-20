param location string = resourceGroup().location
param containerAppName string = 'mcp-server-app'
param containerImage string = 'invictusmcp.azurecr.io/mcp:latest'
param envName string = '${containerAppName}-env'

resource acr 'Microsoft.ContainerRegistry/registries@2025-11-01' existing = {
  name: 'invictusmcp'
}

module mcpIdentity 'br/public:avm/res/managed-identity/user-assigned-identity:0.4.2' = {
  name: 'mcp-identity'
  params: {
    name: 'mcp-identity'
  }
}

resource acrPull 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(mcpIdentity.name, 'acrPull')
  scope: acr
  properties: {
    principalId: mcpIdentity.outputs.principalId
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      '7f951dda-4ed3-4680-a7ca-43fe172d538d'
    )
    principalType: 'ServicePrincipal'
  }
}

module env 'br/public:avm/res/app/managed-environment:0.11.3' = {
  name: envName
  params: {
    name: envName
    location: location
    zoneRedundant: false
    internal: false
  }
}

module app 'br/public:avm/res/app/container-app:0.19.0' = {
  name: containerAppName
  params: {
    name: containerAppName
    location: location
    environmentResourceId: env.outputs.resourceId
    ingressAllowInsecure: true
    ingressExternal: true
    ingressTargetPort: 8080
    registries: [
      {
        server: acr.properties.loginServer
        identity: mcpIdentity.outputs.resourceId
      }
    ]
    containers: [
      {
        name: containerAppName
        image: containerImage
        resources: {
          cpu: json('0.5')
          memory: '1.0Gi'
        }
      }
    ]
    corsPolicy: {
      allowCredentials: true
      allowedOrigins: [
        '*'
      ]
      allowedMethods: [
        '*'
      ]
    }
    managedIdentities: {
      userAssignedResourceIds: [
        mcpIdentity.outputs.resourceId
      ]
    }
  }
  dependsOn: [
    acrPull
  ]
}
