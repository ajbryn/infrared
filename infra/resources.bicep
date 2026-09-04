targetScope = 'resourceGroup'

@description('Short unique name used as a prefix for all resources')
param appName string

@description('Azure region')
param location string = 'southcentralus'

@description('GHCR image for the backend container app')
param backendImage string

@description('GHCR registry URL')
param ghcrRegistry string = 'ghcr.io'

@description('GHCR username (GitHub actor)')
param ghcrUsername string

@description('GHCR token (read:packages)')
@secure()
param ghcrPassword string

@description('GitHub repo URL for Static Web App')
param githubRepoUrl string

@description('GitHub PAT with repo read access for SWA')
@secure()
param githubRepoToken string

param sqlAdminLogin string

@secure()
param sqlAdminPassword string


// // ─── Key Vault ─── 3 cents per 10,000 operations
// resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
//   name: 'kv-${appName}'
//   location: location
//   properties: {
//     tenantId: subscription().tenantId
//     sku: { name: 'standard' }
//     accessPolicies: []
//     enabledForTemplateDeployment: true
//   }
// }



// SQL Database - free

resource sqlServer 'Microsoft.Sql/servers@2025-01-01' = {
  name: 'sql-${appName}'
  location: location
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
    minimalTlsVersion: '1.2'
  }
}

resource sqlDb 'Microsoft.Sql/servers/databases@2025-01-01' = {
  parent: sqlServer
  name: 'myFreeDb'
  location: location
  sku: {
    name: 'GP_S_Gen5'
    tier: 'GeneralPurpose'
    family: 'Gen5'
    capacity: 2
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 34359738368
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: false
    readScale: 'Disabled'
    autoPauseDelay: 60
    requestedBackupStorageRedundancy: 'Local'
    minCapacity: 1
    isLedgerOn: false
    useFreeLimit: true
    freeLimitExhaustionBehavior: 'AutoPause'
  }
}

// // ─── SQL Server + Database ─── $$?
// resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
//   name: 'sql-${appName}'
//   location: rg.location
//   parent: rg
//   properties: {
//     administratorLogin: 'admin-${replace(appName, '-', '')}'
//     administratorLoginPassword: sqlAdminPassword
//     firewallRules: [
//       {
//         name: 'AllowAll'
//         properties: {
//           startIpAddress: '0.0.0.0'
//           endIpAddress: '0.0.0.0'
//         }
//       }
//     ]
//   }
// }

// @secure()
// param sqlAdminPassword string

// resource sqlDb 'Microsoft.Sql/servers/databases@2021-11-01' = {
//   name: 'db-${appName}'
//   location: rg.location
//   parent: [sqlServer, 'databases']
//   sku: {
//     name: 'GP_S0'
//     tier: 'General Purpose'
//   }
//   kind: 'v12.0'
//   properties: {
//     collation: 'SQL_Latin1_General_CP1_CI_AS'
//   }
// }

// ─── Container Apps Environment ───
resource containerEnv 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: 'caenv-${appName}'
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        // customerResourceId: logAnalytics.id
        customerId: logAnalytics.id
      }
    }
  }
}

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: 'log-${appName}'
  location: location
  properties: {
    sku: { name: 'PerGB2018' }
  }
}

// ─── Container App (Backend API) ───
resource containerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: 'api-${appName}'
  location: location
  properties: {
    environmentId: containerEnv.id
    configuration: {
      registries: [
        {
          server: ghcrRegistry
          username: ghcrUsername
          passwordSecretRef: 'ghcr-password'
        }
      ]
      secrets: [
        {
          name: 'ghcr-password'
          value: ghcrPassword
        }
        {
          name: 'db-connection-string'
          // value: 'Server=tcp:${sqlServer.fqdn},1433;Initial Catalog=${sqlDb.name};User ID=${sqlServer.properties.administratorLogin};Password=${sqlAdminPassword};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
          value: 'Server=tcp:${sqlServer.name},1433;Initial Catalog=${sqlDb.name};User ID=${sqlServer.properties.administratorLogin};Password=${sqlAdminPassword};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
        }
      ]
      ingress: {
        external: true
        targetPort: 8080
        transport: 'auto'
        traffic: [
          { latestRevision: true, weight: 100 }
        ]
      }
      activeRevisionsMode: 'Single'
    }
    template: {
      containers: [
        {
          name: 'backend'
          image: backendImage
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
          env: [
            { name: 'ASPNETCORE_ENVIRONMENT', value: 'Production' }
            { name: 'ConnectionStrings__Default', secretRef: 'db-connection-string' }
          ]
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 3
      }
    }
  }
}

// ─── Static Web App (Frontend) ───
resource staticWebApp 'Microsoft.Web/staticSites@2022-09-01' = {
  name: 'swa-${appName}'
  location: location
  sku: { name: 'Free' }
  properties: {
    repositoryUrl: githubRepoUrl
    branch: 'master'
    repositoryToken: githubRepoToken
    buildProperties: {
      appLocation: '/apps/infrared-web'
      appBuildCommand: 'npm run build'
      outputLocation: '/web/dist'
    }
    stagingEnvironmentPolicy: 'Enabled'
  }
}

// ─── Outputs ───
// output resourceGroup string = rg.name
output containerAppFqdn string = containerApp.properties.configuration.ingress.fqdn
output staticWebAppUrl string = staticWebApp.properties.defaultHostname
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName   
