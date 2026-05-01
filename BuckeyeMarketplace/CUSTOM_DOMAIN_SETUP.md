# Custom Domain Configuration Guide

## Overview
This guide covers configuring custom domains for both backend and frontend services on Azure.

---

## Prerequisites
- Registered domain (GoDaddy, Namecheap, Azure Domain Services, etc.)
- Access to domain registrar's DNS settings
- Both backend and frontend deployed to Azure App Service

---

## Backend Custom Domain Setup

### 1. Add Custom Domain in App Service

**Azure Portal:**
1. Navigate to Backend App Service → **Custom domains**
2. Click **Add custom domain**
3. Enter your domain (e.g., `api.yourdomain.com`)
4. Click **Validate**

### 2. Verify Domain Ownership

Azure will display verification options:

**Option A: CNAME Record (Recommended)**
```
Add CNAME in your domain registrar's DNS settings:
Name: api
Points to: <your-app-name>.azurewebsites.net
TTL: 3600
```

**Option B: TXT Record**
```
Add TXT record:
Name: asuid.api
Value: <Azure-provided-verification-id>
TTL: 3600
```

### 3. Configure SSL Certificate

After domain verification:
1. Still in **Custom domains**, click on your domain
2. Click **Add binding**
3. Choose **Create App Service Managed Certificate** (free)
4. Click **Add binding**

Azure automatically:
- Creates a free SSL certificate
- Renews it before expiration
- Enables HTTPS for your custom domain

### 4. Update Environment Variables

Update frontend `REACT_APP_API_URL` to use new custom domain:

**Frontend App Service settings:**
```
REACT_APP_API_URL=https://api.yourdomain.com
```

**Then rebuild and redeploy frontend** (env vars are build-time):
```bash
REACT_APP_API_URL=https://api.yourdomain.com npm run build
```

---

## Frontend Custom Domain Setup

### Option A: Static Web App (Recommended for React)

#### 1. Create Static Web App
```bash
az staticwebapp create \
  --name buckeye-marketplace-frontend \
  --resource-group <your-rg> \
  --source https://github.com/<your-repo> \
  --branch main \
  --app-location "frontend" \
  --output-location "build" \
  --environment-variables "REACT_APP_API_URL=https://api.yourdomain.com"
```

#### 2. Add Custom Domain

**Azure Portal:**
1. Navigate to Static Web App → **Custom domains**
2. Click **Add**
3. Enter domain (e.g., `www.yourdomain.com`)

**DNS Configuration:**

For root domain (`yourdomain.com`):
```
Add ALIAS/ANAME record (if supported) or CNAME:
Name: @
Points to: <staticapp-id>.azurestaticapps.net
```

For www subdomain (`www.yourdomain.com`):
```
Add CNAME record:
Name: www
Points to: <staticapp-id>.azurestaticapps.net
TTL: 3600
```

#### 3. Verify Domain

Follow prompts to verify TXT or CNAME records.

### Option B: App Service (If using App Service for Frontend)

Same process as backend:
1. App Service → **Custom domains**
2. Add domain (e.g., `www.yourdomain.com`)
3. Add CNAME/TXT record for verification
4. Create managed SSL certificate

---

## DNS Configuration Summary

### For Both Backend and Frontend

Create these DNS records in your domain registrar:

| Type | Name | Value | Purpose |
|------|------|-------|---------|
| CNAME | `api` | `backend-app.azurewebsites.net` | Backend API |
| CNAME | `www` | `frontend-app.azurewebsites.net` | Frontend (or static app ID) |
| CNAME | `asuid.api` | `<Azure-verification>` | Domain verification (if needed) |
| CNAME | `asuid.www` | `<Azure-verification>` | Domain verification (if needed) |

**Root domain (`yourdomain.com`):**
- If registrar supports ALIAS/ANAME: use for frontend
- Otherwise: redirect `yourdomain.com` → `www.yourdomain.com`

---

## Update Backend Configuration

After custom domain is configured, update backend CORS settings:

**Backend App Service → Configuration:**
```
FrontendUrl=https://www.yourdomain.com
```

This restricts CORS to your frontend domain only (production security best practice).

---

## SSL/HTTPS Configuration

### Automatic Certificate Management
- **App Service Managed Certificates** (free)
  - Automatically renewed before expiration
  - No configuration needed
  - Recommended for most use cases

- **Azure Key Vault Integration** (advanced)
  - For customer-managed certificates
  - Higher control but requires manual renewal

### Enforce HTTPS Only

**Backend App Service:**
1. **Configuration → General settings**
2. Set **HTTPS Only** to `On`

**Frontend (Static Web App):**
- HTTPS is always on; HTTP automatically redirects

---

## After Domain Configuration

### 1. Update Firewall/Security Rules

If your database has IP firewall rules:
- Verify Azure App Service IPs are whitelisted
- Or use Azure SQL firewall rules with managed identity

### 2. Update Frontend API URL

If frontend is in separate deployment:
```bash
# Rebuild frontend with new API URL
REACT_APP_API_URL=https://api.yourdomain.com npm run build
```

### 3. Verify HTTPS Works

```bash
# Test backend
curl -i https://api.yourdomain.com/api/products

# Test frontend
curl -i https://www.yourdomain.com

# Check SSL certificate
openssl s_client -connect api.yourdomain.com:443
```

### 4. Test End-to-End

1. Navigate to `https://www.yourdomain.com`
2. Open browser Developer Tools → **Network** tab
3. Click product or login
4. Verify requests go to `https://api.yourdomain.com/api/...`
5. Check for CORS errors or certificate warnings

---

## Propagation Times

DNS changes may take up to **24-48 hours** to propagate globally:
- CNAME records: Usually 1-6 hours
- Some users may see old records during propagation
- Test with `nslookup` or `dig`:

```bash
# Check CNAME resolution
nslookup api.yourdomain.com
dig api.yourdomain.com

# Should return: <app-name>.azurewebsites.net
```

---

## Troubleshooting

### Certificate Warning in Browser
- **Cause**: SSL certificate not yet bound or DNS not propagated
- **Fix**: Wait 24 hours, clear browser cache, try incognito mode

### CNAME Conflict
- **Cause**: Domain registrar doesn't allow CNAME on root
- **Fix**: Use ALIAS (GoDaddy, DNSimple) or redirect root → www

### 404 on Custom Domain
- **Cause**: App Service not configured for custom domain
- **Fix**: Verify custom domain binding in App Service blade

### Frontend Requests Still Go to Old Domain
- **Cause**: `REACT_APP_API_URL` not updated before build
- **Fix**: Set env var, rebuild, redeploy frontend

### Mixed Content Error
- **Cause**: Frontend on HTTPS but backend on HTTP
- **Fix**: Ensure backend is HTTPS-only; update `REACT_APP_API_URL`

---

## Example: Complete Setup

### Domain: `buckeye-marketplace.com`

**DNS Records:**
```
api.buckeye-marketplace.com  →  backend-app.azurewebsites.net  (CNAME)
www.buckeye-marketplace.com  →  frontend-app.azurewebsites.net (CNAME)
buckeye-marketplace.com      →  www.buckeye-marketplace.com    (redirect or ALIAS)
```

**Backend App Service:**
- Custom Domain: `api.buckeye-marketplace.com`
- HTTPS Only: On
- Managed Certificate: Auto-renewed
- CORS FrontendUrl: `https://www.buckeye-marketplace.com`

**Frontend App Service:**
- Custom Domain: `www.buckeye-marketplace.com`
- HTTPS Only: On
- Environment: `REACT_APP_API_URL=https://api.buckeye-marketplace.com`

**Result:**
- ✅ Frontend: `https://www.buckeye-marketplace.com`
- ✅ Backend: `https://api.buckeye-marketplace.com`
- ✅ CORS restricted to `www.buckeye-marketplace.com`
- ✅ SSL certificates auto-managed
- ✅ HTTP requests auto-redirect to HTTPS

---

## CLI Commands (Alternative to Portal)

### Add Custom Domain (App Service)
```bash
az webapp config hostname add \
  --webapp-name backend-app \
  --resource-group <rg> \
  --hostname api.yourdomain.com
```

### Create Managed Certificate
```bash
az webapp config ssl create \
  --name backend-app \
  --resource-group <rg> \
  --certificate-name buckeye-marketplace-cert
```

### Bind Certificate
```bash
az webapp config ssl bind \
  --name backend-app \
  --resource-group <rg> \
  --certificate-thumbprint <thumb> \
  --ssl-type SNI
```

### Enable HTTPS Only
```bash
az webapp update \
  --name backend-app \
  --resource-group <rg> \
  --set httpsOnly=true
```

---

## Next Steps

1. **Register domain** (if not already done)
2. **Decide on service structure:**
   - Backend: App Service with custom domain
   - Frontend: Static Web App (recommended) or App Service
3. **Add custom domain** in Azure Portal
4. **Configure DNS records** in domain registrar
5. **Wait for propagation** (up to 48 hours)
6. **Update REACT_APP_API_URL** and rebuild frontend
7. **Verify HTTPS** works end-to-end
8. **Set CORS FrontendUrl** in backend for security
