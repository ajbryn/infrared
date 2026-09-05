using './resources.bicep'

param appName = 'myapp'
param location = 'centralus'
param backendImage = 'ghcr.io/ajbryn/infrared/backend:latest'
param ghcrRegistry = 'ghcr.io'
param ghcrUsername = 'ajbryn'
param githubRepoUrl = 'https://github.com/ajbryn/infrared'

// Secrets — these get overridden by the pipeline at deploy time.
// For local testing you can hardcode them (never commit real secrets):
param ghcrPassword = ''
param githubRepoToken = ''
param sqlAdminLogin = 'admin'   
param sqlAdminPassword = 'Password123$'   
