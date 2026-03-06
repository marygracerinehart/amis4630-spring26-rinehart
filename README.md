# amis4630-spring26-rinehart
AMIS 4630 Buckeye Marketplace Project

# Buckeye Marketplace
A marketplace web application for Ohio State students, staff, fans, and parents to buy and sell items.

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


# Product Detail Page


## AI Usage Summary

### Tools Used
Claude (claude.ai)
Github CoPilot

### Prompts I Used
Enables CORS in program.cs
In productscontroller.cs add the following items as products: Introduction to Information Systems textbook, Ohio State Sweatshirt - Medium, Organic Chemistry Textbook, Desk Lamp, Winter Coat, a tv, a mini refridgerator, an ohio state jacket, a mirror make sure that each product has the following fields: id, title, description, price, category, sellerName, postedDate, imageUrl
create a product list page in the frontend (react) that is a page displaying all available products as cards showing the title, price, category, and seller's name
create another page for each product so that when you click on a product it shows you the full details of that product
on the main product page update it so that each card shows the title, price, category, and seller's name


### What I Did Myself
- Created the React component structure
- Wired up routing between list and detail pages
- Debugged port and connection issues between frontend and backend