# E-Book Management System [(EBMS API Workspace 🚀)](https://www.postman.com/bnadel/workspace/e-book-management-system)

## Overview 👋
Welcome to the E-Book Management System! This advanced API leverages a 3-tier architecture to provide a seamless experience for managing e-books, authors, categories, orders, reviews, and wishlists. 
Our system is packed with powerful features, including integrated payment gateway for secure transactions, advanced searching and filtering to find exactly what you need, 
efficient pagination for smooth navigation, rate limiting for optimal performance, caching for fast data retrieval, versioning for future-proofing, 
and advanced authentication with JWT and refresh tokens for secure user management.

## Features 😊
- [x] **3-Tier Architecture:** to provide a seamless experience for managing system functions.
- [x] **Advanced Authentication:** Secure user management with JWT and refresh tokens.
- [x] **Integrated Payment Gateway:** Secure and efficient transactions with PayPal.
- [x] **Sending Emails:** When payment is made, an e-mail with order details is sent to system admins.
- [x] **Advanced Searching and Filtering:** Easily find books based on various criteria.
- [x] **Efficient Pagination:** Smooth navigation through large datasets.
- [x] **Rate Limiting:** Ensures optimal performance under high traffic.
- [x] **In-Memory Caching:** Quick data retrieval for an enhanced user experience.
- [x] **Versioning:** Future-proof API endpoints to maintain compatibility.
- [x] **Manage E-Books:** Create, retrieve, update, and delete e-book records.
- [x] **Author Management:** Handle author details and their associated works.
- [x] **Category Management:** Organize books into genres or themes.
- [x] **Order Processing:** Create and manage book orders with payment integration.
- [x] **Review Management:** Submit and retrieve book reviews.
- [x] **Wishlist Management:** Add, retrieve, and manage user wishlists.

## Technologies 🤖
- ASP .NET Core Web API
- Microsoft SQL Server
- Postman and Swagger for testing the API

## What I Use and Apply 🛠️
- Using 3-Layers (Infrastructure - Data - API)
- Using Identity for user management
- Authentication with JWT and RefreshTokens
- Using Fluent API to configure system entities
- Using In-Memory Caching
- Using API Integration for payment with Paypal
- Sending Emails with outlook host
- Trying to apply SOLID Principles
- Applying Unit of Work and Repository Pattern
- Applying Generic Repository (Base Repository)
- Applying Dependency Injection

## Getting Started with EBMS API 🎬
### Prerequisites ✍
- [x] .NET 6.0 or later.
- [x] Microsoft SQL Server.
- [x] Postman or Swagger (for API testing).
- [x] PayPal developer account for payment integration.
### Installation ✅
1. Clone The Repository
   ```bash
   git clone https://github.com/Mohamed-Adel23/E-Book-Management-System
   ```
2. Navigate to the project directory
   ```bash
   cd E-Book-Management-System
   ```
3. Install the required dependencies
   ```bash
   dotnet restore
   ```
4. Set up the database connection string in `appsettings.json`.
5. Run the migrations to set up the database
   ```bash
   dotnet ef database update
   ```
6. Build the application
   ```bash
   dotnet build
   ```
7. Run the application
   ```bash
   dotnet run
   ```
## API Documentation 🌐
- [x] [Authentication Endpoints 🚀](https://documenter.getpostman.com/view/28631317/2sA3e5dTjA)
- [x] [Authors Endpoints 🚀](https://documenter.getpostman.com/view/28631317/2sA3e5dTjE)
- [x] [Categories Endpoints 🚀](https://documenter.getpostman.com/view/28631317/2sA3e5dnyt)
- [x] [Books Endpoints 🚀](https://documenter.getpostman.com/view/28631317/2sA3e5dnys)
- [x] [Orders Endpoints 🚀](https://documenter.getpostman.com/view/28631317/2sA3e5dnyu)
- [x] [Reviews Endpoints 🚀](https://documenter.getpostman.com/view/28631317/2sA3e5dnyv)
- [x] [Wishlists Endpoints 🚀](https://documenter.getpostman.com/view/28631317/2sA3e5do4C)
   
## Example of API Usage 🛸
### Book GET Request 
  ```http
  GET  /api/v1/Books/2
  ```
### Get Book By Id Response
  ```javascript
  {
      "title": "Theory of Relativity",
      "description": "It is a book that talk about Ineshtine's Relativity",
      "physicalPrice": 45.50,
      "discount": 0.50,
      "availableQuantity": 116,
      "rate": 4.2,
      "bookFilePath": "53290767-c8f3-4-EBMS.pdf",
      "bookCoverImage": "c9bd1dde-481a-4-EBMS.png",
      "published_at": "2000-11-12",
      "created_at": "2024-07-07T20:06:40.37",
      "updated_at": null,
      "author": {
          "fullName": "Mostafa Mahmoud",
          "bio": "He is a great writer.",
          "profilePic": null,
          "created_at": "2024-07-07T19:36:32.713",
          "updated_at": null,
          "message": null,
          "id": 1
      },
      "categories": [
          {
              "title": "Science",
              "description": "Books about Science and Scientists.",
              "created_at": "2024-07-07T19:42:30.8",
              "updated_at": null,
              "message": null,
              "id": 1
          },
          {
              "title": "Culuture",
              "description": "Books about Culutures.",
              "created_at": "2024-07-07T19:44:14.78",
              "updated_at": null,
              "message": null,
              "id": 2
          },
      ],
      "message": null,
      "id": 2
  }
  ```

## Contributing 🤝
1. Fork the Repository
2. Create a new branch
   ```bash
   git checkout -b feature-name
   ```
3. Make your changes
4. Commit your changes
  ```bash
  git commit -m 'Add some feature
  ```
5. Push to the branch
   ```bash
   git push origin feature-name
   ```
6. Open a pull request

## Contributers ⚡
- [x] [Mohamed Adel Elsayed](https://github.com/Mohamed-Adel23)

## Resources 📚
