targetScope = 'subscription'

@description('Short unique name used as a prefix for all resources')
param appName string

@description('Azure region')
param location string = 'centralus'

// ─── Resource Group ───
resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: 'rg-${appName}'
  location: location
  tags: { appName: appName }
}

// ─── Outputs ───
output resourceGroup string = rg.name
