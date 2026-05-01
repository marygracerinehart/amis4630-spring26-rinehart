# amis4630-spring26-rinehart
AMIS 4630 Buckeye Marketplace Project

# Buckeye Marketplace
A full-stack marketplace web application for Ohio State students, staff, fans, and parents to buy and sell items. Built with an ASP.NET Core REST API backend and a React frontend, deployed to Microsoft Azure.

#Admin Credentials
Email	admin@buckeyemarketplace.com
Password	Admin123

## How to Run
#.NET API
1. Open terminal and navigate to BuckeyeMarketplace API through backend folder -  cd /Users/marygracerinehart/amis4630-spring26-rinehart/BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI
2. Run:  dotnet run
3. API available at http://localhost:5107/swagger
	- /api/products - returns all products
	- /api/products/id - returns a single product by ID

# React App
1. Open a second terminal and go to frontend folder - cd /Users/marygracerinehart/amis4630-spring26-rinehart/BuckeyeMarketplace/frontend
2. Run: npm start
3. App will be available at: http://localhost:3000

## Screenshots

# Product List Page
![Screenshot 2026-03-06 at 11.07.44 AM](../Desktop/Screenshot 2026-03-06 at 11.07.44 AM.png)

# Product Detail Page
![Screenshot 2026-03-06 at 11.08.15 AM](../Desktop/Screenshot 2026-03-06 at 11.08.15 AM.png)


# Trigger CI/CD

 
**Live Application**
 
| | URL |
|---|---|
|  Frontend | https://buckeyemarketplace-frontend-05011243.azurewebsites.net |
| Backend API | https://ndw38rn3iu-hfbzdmfhhkdmajev.centralus-01.azurewebsites.net |
| Swagger UI | https://ndw38rn3iu-hfbzdmfhhkdmajev.centralus-01.azurewebsites.net/swagger |
 
---
 
## Features
 
-  Browse all marketplace listings as product cards
-  View full product details (description, price, seller, stock)
-  Add items to cart and manage quantities
- Place orders with a shipping address
-  User registration and login with JWT authentication
- Automatic JWT token refresh
- Admin dashboard to manage products and view all orders
- Responsive design
---
 
## Technology Stack
 
| Layer | Technology | Version |
|-------|------------|---------|
| Frontend | React | 18+ |
| Frontend Routing | React Router | v7+ |
| Backend | ASP.NET Core | .NET 10 |
| ORM | Entity Framework Core | 9+ |
| Database | Azure SQL Server | — |
| Authentication | ASP.NET Core Identity + JWT Bearer | — |
| API Docs | Swagger / Swashbuckle | — |
| Hosting | Azure App Service | — |
| CI/CD | GitHub Actions | — |
 
---
 
## Project Structure
 
```
amis4630-spring26-rinehart/
└── BuckeyeMarketplace/
    ├── backend/
    │   ├── BuckeyeMarketplaceAPI/          # ASP.NET Core Web API
    │   │   ├── Controllers/                # API endpoints
    │   │   ├── Models/                     # Data models and DTOs
    │   │   ├── Data/                       # EF Core DbContext and seeder
    │   │   ├── Migrations/                 # Database migrations
    │   │   └── Program.cs                  # App startup and middleware
    │   └── BuckeyeMarketplaceAPI.Tests/    # Unit and integration tests
    └── frontend/
        └── src/
            ├── components/                 # Reusable UI components
            ├── context/                    # React context (cart, auth, notifications)
            ├── pages/                      # Page-level components
            ├── services/                   # API client and service functions
            └── styles/                     # CSS styles
```
 
---
 
## Local Development Setup
 
### Prerequisites
 
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- SQL Server (local instance) **or** use the Azure SQL connection string directly
---
 
### Backend Setup
 
1. Navigate to the backend project:
   ```bash
   cd BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI
   ```
 
2. For a local SQL Server instance, create or update `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=BuckeyeMarketplace;Trusted_Connection=True;"
     }
   }
   ```
 
3. Run the application — database migrations and seed data are applied automatically on startup:
   ```bash
   dotnet run
   ```
 
4. The API will be available at `http://localhost:5107`
   - Swagger UI: `http://localhost:5107/swagger`
---
 
### Frontend Setup
 
1. In a separate terminal, navigate to the frontend:
   ```bash
   cd BuckeyeMarketplace/frontend
   ```
 
2. Create a `.env` file in the `frontend/` directory:
   ```
   REACT_APP_API_URL=http://localhost:5107
   ```
 
3. Install dependencies and start the dev server:
   ```bash
   npm install
   npm start
   ```
 
4. The app will be available at `http://localhost:3000`
---
 
## Environment Variables
 
### Frontend
 
Set these before running `npm run build` (or in your `.env` file for local dev):
 
| Variable | Description | Example |
|----------|-------------|---------|
| `REACT_APP_API_URL` | Backend API base URL | `https://your-api.azurewebsites.net` |
 
### Backend
 
Configure these in Azure App Service → Configuration → Application Settings:
 
| Variable | Description |
|----------|-------------|
| `ConnectionStrings__DefaultConnection` | Azure SQL Server connection string |
| `Jwt__Key` | Secret key for signing JWT tokens (minimum 32 characters) |
| `Jwt__Issuer` | JWT issuer (e.g., `BuckeyeMarketplaceAPI`) |
| `Jwt__Audience` | JWT audience (e.g., `BuckeyeMarketplaceUsers`) |
| `FrontendUrl` | Frontend origin URL for CORS whitelist (optional) |
 
---
 
## API Documentation
 
Full interactive documentation is available via the live [Swagger UI](https://ndw38rn3iu-hfbzdmfhhkdmajev.centralus-01.azurewebsites.net/swagger).
 
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| `GET` | `/api/products` | Get all products | Public |
| `GET` | `/api/products/{id}` | Get product by ID | Public |
| `POST` | `/api/products` | Create a product | Admin |
| `PUT` | `/api/products/{id}` | Update a product | Admin |
| `DELETE` | `/api/products/{id}` | Delete a product | Admin |
| `POST` | `/api/auth/register` | Register a new user | Public |
| `POST` | `/api/auth/login` | Login and receive JWT | Public |
| `POST` | `/api/auth/refresh` | Refresh JWT token | Public |
| `GET` | `/api/cart` | Get current user's cart | 🔒 User |
| `POST` | `/api/cart/items` | Add item to cart | 🔒 User |
| `DELETE` | `/api/cart/items/{id}` | Remove item from cart | 🔒 User |
| `POST` | `/api/orders` | Place an order | 🔒 User |
| `GET` | `/api/orders` | Get current user's orders | 🔒 User |
| `GET` | `/api/admin/orders` | Get all orders | 🛡️ Admin |
 
---
 
## Deployment
 
### Manual Deployment to Azure
 
**Backend:**
 
```bash
cd BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI
dotnet publish BuckeyeMarketplaceAPI.csproj -c Release -o ./publish-release
zip -r api-deploy.zip publish-release/
az webapp deploy \
  --resource-group rg-buckeye-marketplace \
  --name ndw38rn3iu \
  --src-path api-deploy.zip \
  --type zip
```
 
**Frontend:**
 
```bash
cd BuckeyeMarketplace/frontend
# Ensure REACT_APP_API_URL is set in .env before building
npm run build
zip -r frontend-build.zip build/
az webapp deploy \
  --resource-group rg-buckeye-marketplace \
  --name buckeyemarketplace-frontend-05011243 \
  --src-path frontend-build.zip \
  --type zip
```
 
---
 
### CI/CD with GitHub Actions
 
Pushing to the `main` branch automatically triggers builds and deployments for both the frontend and backend.
 
**Required GitHub Repository Secrets:**
 
| Secret | Description |
|--------|-------------|
| `AZURE_BACKEND_PUBLISH_PROFILE` | Publish profile for the backend Azure App Service |
| `AZURE_FRONTEND_PUBLISH_PROFILE` | Publish profile for the frontend Azure App Service |
 
To download a publish profile: Azure Portal → App Service → **Get publish profile**.
 
---
 
## Running Tests
 
```bash
# Backend unit/integration tests
cd BuckeyeMarketplace/backend/BuckeyeMarketplaceAPI.Tests
dotnet test
 
# Frontend tests
cd BuckeyeMarketplace/frontend
npm test
```
 
---
 
## Seeded Admin Account
 
A default admin account is created automatically when the backend first runs:
 
| Field | Value |
|-------|-------|
| Email | `admin@buckeyemarketplace.com` |
| Password | `Admin123` |
| Role | `Admin` |
 
 
---
 
## AI Usage Summary

### Tools Used
Claude (claude.ai)
Github CoPilot

### Prompts I Used
Enables CORS in program.cs
In productscontroller.cs add the following items as products: Introduction to Information Systems textbook, Ohio State Sweatshirt - Medium, Organic Chemistry Textbook, Desk Lamp, Winter Coat, a tv, a mini refridgerator, an ohio state jacket, a mirror make sure that each product has the following fields: id, title, description, price, category, sellerName, postedDate, imageUrl
create a product list page in the frontend (react) that is a page displaying all available products as cards showing the title, price, category, and seller's name
create another page for each product so that when you click on a product it shows you the full details of that product


### What I Did Myself
- Created the React component structure
- Debugged connection issues between frontend and backend
- added image urls
- used own judgement going back in fourth with github co pilot on different styling things including size of card and image
