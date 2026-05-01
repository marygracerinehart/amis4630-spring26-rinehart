# GitHub Actions CI/CD Pipeline Setup

## Overview
Two GitHub Actions workflows automate building and deploying both frontend and backend to Azure:
- **Backend**: `backend-deploy.yml` - .NET build, test, and deploy
- **Frontend**: `frontend-deploy.yml` - React build, test, and deploy

## Workflows

### Backend Workflow (`backend-deploy.yml`)
**Triggers:** Push or PR to `main` affecting `backend/**`

**Steps:**
1. ✅ Checkout code
2. ✅ Setup .NET 10
3. ✅ Restore NuGet dependencies
4. ✅ Build in Release mode
5. ✅ Run unit tests
6. ✅ Publish for deployment
7. ✅ Deploy to Azure App Service (`ndw38rn3iu`)

### Frontend Workflow (`frontend-deploy.yml`)
**Triggers:** Push or PR to `main` affecting `frontend/**`

**Steps:**
1. ✅ Checkout code
2. ✅ Setup Node.js 18
3. ✅ Install npm dependencies
4. ✅ Build React app (with `REACT_APP_API_URL` set to deployed backend)
5. ✅ Run tests (with no-watch mode for CI)
6. ✅ Upload build artifact
7. ✅ Deploy to Azure App Service (`buckeyemarketplace-frontend-05011243`)

---

## Setup Requirements

### 1. Generate Azure Publish Profiles

#### For Backend:

1. Azure Portal → App Services → `ndw38rn3iu` (backend)
2. Download publish profile:
   - Click **Get Publish Profile** (top of Overview)
   - Save as `.PublishSettings` file
3. Open in text editor, copy entire content

#### For Frontend:

1. Azure Portal → App Services → `buckeyemarketplace-frontend-05011243`
2. Click **Get Publish Profile**
3. Open file, copy content

### 2. Add GitHub Secrets

1. GitHub Repo → **Settings** → **Secrets and variables** → **Actions**
2. Click **New repository secret**

Add two secrets:

| Secret Name | Value |
|-------------|-------|
| `AZURE_BACKEND_PUBLISH_PROFILE` | *(paste entire .PublishSettings content for backend)* |
| `AZURE_FRONTEND_PUBLISH_PROFILE` | *(paste entire .PublishSettings content for frontend)* |

### 3. Commit & Push

```bash
git add .github/workflows/
git commit -m "Add CI/CD pipelines"
git push origin main
```

---

## How It Works

### On Push to Main (Backend Files Changed)
1. GitHub Actions triggers `backend-deploy.yml`
2. Builds .NET application in Release mode
3. Runs all unit tests
4. If tests pass, publishes and deploys to Azure
5. Backend is updated at: `https://ndw38rn3iu-hfbzdmfhhkdmajev.centralus-01.azurewebsites.net`

### On Push to Main (Frontend Files Changed)
1. GitHub Actions triggers `frontend-deploy.yml`
2. Installs npm dependencies
3. Builds React app with production optimizations
4. Runs tests
5. If successful, deploys to Azure
6. Frontend is updated at: `https://buckeyemarketplace-frontend-05011243.azurewebsites.net`

---

## Viewing Workflow Runs

1. GitHub Repo → **Actions** tab
2. Select workflow (`Backend CI/CD` or `Frontend CI/CD`)
3. View latest run with status ✅ (success) or ❌ (failure)
4. Click run to see detailed logs

---

## Workflow File Locations

```
.github/workflows/
├── backend-deploy.yml       # Backend CI/CD pipeline
└── frontend-deploy.yml      # Frontend CI/CD pipeline
```

---

## Troubleshooting

### Deployment Fails: "Authentication Failed"
- **Cause**: Publish profile secret not set or incorrect
- **Fix**: Re-download publish profile from Azure, paste entire content into GitHub Secrets

### Tests Fail
- **Backend**: Ensure all tests in `BuckeyeMarketplaceAPI.Tests` pass locally
- **Frontend**: Ensure `npm test` passes locally

### Build Fails: Dependencies Not Found
- **Backend**: Check `.csproj` file paths match workflow
- **Frontend**: Check `package.json` in `frontend/` folder

### Deployment Succeeds but App Doesn't Update
- Clear browser cache (Ctrl+Shift+Del or Cmd+Shift+Delete)
- Hard refresh (Ctrl+Shift+R or Cmd+Shift+R)
- Wait 1-2 minutes for Azure to fully deploy

---

## Environment Variables

### Backend
All configuration from App Service settings (no env vars in workflow)

### Frontend
- `REACT_APP_API_URL`: Set in workflow to deployed backend URL
- `CI=true`: Prevents interactive watch mode in CI environment

---

## Security Notes

- ✅ Publish profiles stored securely in GitHub Secrets (encrypted)
- ✅ Never commit `.PublishSettings` files to git
- ✅ Workflows only deploy on main branch (not on PRs)
- ✅ Tests must pass before deployment

---

## Next Steps

1. Download and add publish profiles to GitHub Secrets
2. Push workflow files to main branch
3. Make a small change (e.g., update README)
4. Watch the GitHub Actions run automatically
5. Verify deployment successful in Azure

---

## Example Workflow Success Output

```
✅ Checkout code
✅ Setup .NET / Node.js
✅ Install dependencies
✅ Build
✅ Run tests (2 passed)
✅ Publish
✅ Deploy to Azure App Service
```

Done! Both frontend and backend now auto-deploy on every push to main! 🎉

---

## ✅ Workflows Active & Ready
- GitHub Secrets configured: `AZURE_BACKEND_PUBLISH_PROFILE`, `AZURE_FRONTEND_PUBLISH_PROFILE`
- Workflows will automatically trigger on push to main
- Deployment Status: Ready for testing
