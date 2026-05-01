# Deployment Guide - BuckeyeMarketplace

## Overview
This guide covers deploying the BuckeyeMarketplace to Azure App Service with proper HTTPS enforcement, environment variable configuration, and security headers.

---

## Backend Deployment (ASP.NET Core API)

### Prerequisites
- Azure App Service (Windows or Linux)
- Azure SQL Database or SQL Server
- Connection string for database

### Required App Service Settings

Set these in **App Service → Configuration → Application settings**:

| Key | Value | Description |
|-----|-------|-------------|
| `ConnectionStrings__DefaultConnection` | `Server=tcp:<server>.database.windows.net,1433;Initial Catalog=<db>;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=Active Directory Default;` | SQL database connection (Active Directory recommended for Azure) |
| `Jwt__Key` | *(256-bit base64 secret)* | JWT signing key for token generation |
| `Jwt__Issuer` | `BuckeyeMarketplaceAPI` | JWT issuer claim |
| `Jwt__Audience` | `BuckeyeMarketplaceClient` | JWT audience claim |
| `Jwt__ExpirationInMinutes` | `60` | Access token expiration (minutes) |
| `Jwt__RefreshTokenExpirationInDays` | `7` | Refresh token expiration (days) |
| `FrontendUrl` | `https://<frontend-domain>.azurewebsites.net` | Frontend URL for CORS (optional; defaults to allow-all in production) |

### HTTPS Configuration (Automatic)
- **Automatic redirects**: `http://` → `https://` in production (via `UseHttpsRedirection()`)
- **HSTS header** (`Strict-Transport-Security`): Added for 1 year by `UseHsts()`
- **Development**: HTTPS enforcement disabled in dev environment

### Database Migration
Migrations run automatically on startup. If this fails:

```bash
cd backend/BuckeyeMarketplaceAPI
dotnet ef database update --connection "your-connection-string"
```

### JWT Key Generation (Required)
Generate a secure key:

```powershell
# PowerShell
[Convert]::ToBase64String((New-Object Random).GetBytes(32))

# Bash
openssl rand -base64 32
```

---

## Frontend Deployment (React)

### Build-Time Configuration
The frontend is built **once** with environment variables baked in. React requires `REACT_APP_*` prefixed variables at build time.

### Required Build Settings
Before building, ensure this environment variable is set:

```bash
export REACT_APP_API_URL=https://<backend>.azurewebsites.net
npm run build
```

### Azure App Service Settings (Frontend)

| Key | Value | Description |
|-----|-------|-------------|
| `REACT_APP_API_URL` | `https://<backend>.azurewebsites.net` | Backend API URL (must include https://) |

### Deployment Process

1. **Build locally** (or in CI/CD pipeline):
   ```bash
   cd frontend
   REACT_APP_API_URL=https://<backend>.azurewebsites.net npm run build
   ```

2. **Deploy build folder** to App Service:
   - Deploy contents of `frontend/build/` to App Service's `/home/site/wwwroot`
   - Or use Azure Static Web Apps (recommended for React)

3. **Important**: After changing `REACT_APP_API_URL`:
   - Clear browser cache
   - Hard refresh (Ctrl+Shift+R or Cmd+Shift+R)
   - Check Network tab to confirm requests go to correct backend URL

### HTTPS Enforcement
- Browsers block mixed content (HTTP requests from HTTPS pages)
- Frontend must use HTTPS URL for backend (same domain or cross-origin)
- Frontend is automatically served over HTTPS by App Service

---

## HTTPS & Security Settings

### Backend Security Headers
Automatically added by middleware:
- `Strict-Transport-Security: max-age=31536000` (1 year)
- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `Content-Security-Policy: default-src 'none'; frame-ancestors 'none'`
- `Referrer-Policy: strict-origin-when-cross-origin`

### CORS Configuration
**Development**: Allows `http://localhost:3000` (frontend dev server)

**Production**: 
- If `FrontendUrl` configured in App Settings: allows only that origin
- If not configured: allows all origins (for flexibility; should be restricted in production)

Set `FrontendUrl` in App Service settings to restrict:
```
FrontendUrl=https://<frontend>.azurewebsites.net
```

### Database Connection Security
- `Encrypt=True` enforces encrypted connection
- `TrustServerCertificate=False` validates certificate
- Use Azure Active Directory authentication when possible

---

## Verification Checklist

- [ ] Backend App Service is running
- [ ] Database migrations completed successfully
- [ ] `REACT_APP_API_URL` set in App Service settings before frontend build
- [ ] Frontend `npm run build` executed with `REACT_APP_API_URL` set
- [ ] Frontend deployed to App Service static hosting or Azure Static Web Apps
- [ ] Browser Network tab shows requests to `https://<backend>/api/...`
- [ ] `/api/products` returns 200 with product JSON
- [ ] Login/auth endpoints work without CORS errors
- [ ] JWT tokens are properly signed and validated
- [ ] HTTPS redirect works: `http://<app>.azurewebsites.net` → `https://...`

---

## Common Issues & Fixes

### "Failed to fetch products" on Azure Frontend
- **Cause**: `REACT_APP_API_URL` not set or set to localhost
- **Fix**: Set `REACT_APP_API_URL=https://<backend>.azurewebsites.net` in App Service settings
- **Then**: Rebuild frontend and redeploy

### CORS errors
- **Cause**: Frontend domain not in backend CORS whitelist
- **Fix**: Set `FrontendUrl` in backend App Service settings or configure CORS policy

### JWT token validation fails
- **Cause**: `Jwt:Key` mismatch between services
- **Fix**: Use same `Jwt__Key` value across environments

### Database connection timeout
- **Cause**: Connection string format or firewall rules
- **Fix**: Verify `ConnectionStrings__DefaultConnection` format and database firewall allows App Service IP

### Mixed content warning (HTTPS frontend, HTTP backend)
- **Cause**: Frontend using `http://` backend URL
- **Fix**: Ensure all API calls use `https://`

---

## Local Development Setup

### Backend
```bash
cd backend/BuckeyeMarketplaceAPI
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\mssqlocaldb;Database=BuckeyeMarketplace;Trusted_Connection=true;"
dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 32)"
dotnet run
```

### Frontend
```bash
cd frontend
REACT_APP_API_URL=http://localhost:5107 npm start
```

---

## Environment Variable Reference

### Backend (ASP.NET Core)
Uses ASP.NET Core configuration hierarchy:
1. User Secrets (development)
2. appsettings.json
3. appsettings.{Environment}.json
4. Environment Variables (App Service settings)

Prefix: `Jwt__Key` (double underscore for colon separator)

### Frontend (React)
- **Build-time only** (baked into JavaScript bundle)
- Must be prefixed `REACT_APP_`
- Read from `.env` file during `npm run build`
- Azure Static Web Apps: Set in "Environment" settings before build

---

## Next Steps

1. Generate JWT key and store securely
2. Configure SQL connection string
3. Set all required App Service settings
4. Build frontend with correct `REACT_APP_API_URL`
5. Deploy both backend and frontend
6. Run verification checklist
