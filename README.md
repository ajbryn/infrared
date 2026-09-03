# infrared
Testin


## TODO:

### GitHub Secrets to Configure

Secret	Value

AZURE_CREDENTIALS	Service principal JSON (see below)

GHCR_TOKEN	Fine-grained PAT with read:packages on the repo

SQL_ADMIN_PASSWORD	Strong password for SQL admin

### Create service principal (once in Azure CLI):

```
az ad app create --display-name "gh-deploy-myapp"
az ad sp create --id <appId>
az role assignment create --role "Contributor" \
  --assignee <spObjectId> \
  --scope /subscriptions/<subId>   
```

### Then set AZURE_CREDENTIALS in GitHub repo settings as:
```
{
  "clientId": "<appId>",
  "clientSecret": "<clientSecret>",
  "subscriptionId": "<subId>",
  "tenantId": "<tenantId>"
}   
```


## Scaling to Enterprise
This structure scales cleanly:

- Multi-environment: duplicate the Bicep with different appName values (e.g., myapp-staging, myapp-prod) or parameterize with an environment param
- Separate resource groups per tier: split infra/ into network.bicep, data.bicep, compute.bicep, frontend.bicep as modules
- Replace free tiers: swap SQL S0 → General Purpose P1, Container Apps → Consumption or Dedicated, SWA Free → Standard
- Add Front Door / CDN / WAF: new Bicep module, no changes to app code
- GitHub Environments: gate deploys with manual approval for production branch