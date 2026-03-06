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
![Screenshot 2026-03-06 at 11.07.44 AM](../Desktop/Screenshot 2026-03-06 at 11.07.44 AM.png)

# Product Detail Page
![Screenshot 2026-03-06 at 11.08.15 AM](../Desktop/Screenshot 2026-03-06 at 11.08.15 AM.png)

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