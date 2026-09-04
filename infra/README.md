# Create an App Registration and Service Principal with OIDC

Do this once before running deploy

### 1. Create an App Registration in Entra ID
In the Azure Portal (or CLI):

```ps
az ad app create `
  --display-name "github-actions-oidc" `
  --query id
```

Note the Application ID (client-id) — you'll need it. 

### 2. Create a Service Principal
```ps
az ad sp create --id <client-id> --query id
```

Note the returned id (sp-object-id) — you'll need it. 

### 3. Add a Federated Credential
This is the key step — it tells Entra ID to trust GitHub's OIDC tokens:

```ps
az ad app federated-credential create `
  --id <client-id> `
  --parameters '{
    "name": "github-oidc",
    "issuer": "https://token.actions.githubusercontent.com",
    "subject": "repo:YOUR-ORG/YOUR-REPO:ref:refs/heads/main",
    "audiences": ["api://AzureADTokenExchange"]
  }'
```

The _subject_ must match how the workflow is triggered:

> ! If repo was created after July 15, 2026, GitHub issues tokens with the new immutable subject format that embeds numeric owner and repo IDs
> e.g., repo:YOUR-ORG@012345678/YOUR-REPO@0123456789
> find it at https://github.com/YOUR-ORG/YOUR-REPO/settings/actions/oidc-configuration 

repo:ajbryn@115934368/infrared@1355305578

| Trigger | Subject |
|--|--|
| Push to main | repo:YOUR-ORG@012345678/YOUR-REPO@0123456789:ref:refs/heads/main |
| Push to master | repo:YOUR-ORG@012345678/YOUR-REPO@0123456789:ref:refs/heads/master |
| Any branch | repo:YOUR-ORG@012345678/YOUR-REPO@0123456789:ref_type:branch |
| Tag v* | repo:YOUR-ORG@012345678/YOUR-REPO@0123456789:ref_type:tag |
| GitHub Environment | repo:YOUR-ORG@012345678/YOUR-REPO@0123456789:environment:production |


## 4. Assign a Role (least privilege)
```ps
az role assignment create `
  --assignee <sp-object-id> `
  --role "Contributor" `
  --scope "/subscriptions/<sub-id>/resourceGroups/your-rg"
```

(Go to Azure portal to find the subscription id (sub-id) or use `az account show --query id --output tsv`)

(Optional) After resource group has been created, can delete and recreate to narrow the scope of this permission:
```ps
az role assignment delete `
  --assignee <sp-object-id> `
  --role "Contributor" `
  --scope "/subscriptions/<sub-id>"

az role assignment create `
  --assignee <sp-object-id> `
  --role "Contributor" `
  --scope "/subscriptions/<sub-id>/resourceGroups/YOUR-RG"
```

## 5. Add GitHub Secrets/Variables
Go to Settings → Secrets and variables → Actions and add:

| Name | Value |
|--|--|
| AZURE_CLIENT_ID | The app's client ID (this is not the client-id output from step 1 above, but use that id in this ps command to get the app's client ID `az ad app show --id <client-id> --query displayName -o tsv`) |
| AZURE_TENANT_ID | Your Entra tenant ID (`az account show --query tenantId --output tsv`) |
| AZURE_SUBSCRIPTION_ID | Target subscription ID (`az account show --query id --output tsv`) |

> These are identifiers, not secrets — some people use Variables (vars.AZURE_CLIENT_ID) instead of secrets since nothing here is sensitive.

## 6. Update Your Workflow

```yml
name: Deploy to Azure

on:
  push:
    branches: [main]

permissions:
  contents: read

jobs:
  deploy-infra:
    runs-on: ubuntu-latest
    permissions:
      id-token: write    # ← critical: allows GitHub to mint the OIDC JWT
      contents: read
    steps:
      - uses: actions/checkout@v4

      - name: Azure Login
        uses: azure/login@v2
        with:
          client-id: ${{ secrets.AZURE_CLIENT_ID }}
          tenant-id: ${{ secrets.AZURE_TENANT_ID }}
          subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}

      - name: Verify
        run: az account show
```

