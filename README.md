# DJualan.Portal
A scalable web application for managing products, orders, and users — built with ASP.NET Core, ReactJS, and SQL Server.

📖 Overview

Djualan is a modern and modular E-Commerce Admin Portal designed to help business owners manage their online store efficiently.
It provides a responsive dashboard for managing products, monitoring orders, viewing sales reports, and handling users — all with a clean UI and secure backend.

💡 “Djualan” combines Dwiki + Jualan, representing a modern digital platform built by a local developer to empower small businesses and online sellers.

⚙️ Tech Stack
Layer	Technology
Frontend	ReactJS (Vite) + Tailwind CSS
Backend	ASP.NET Core 8 Web API
Database	SQL Server / Azure SQL
Storage	Azure Blob Storage
Auth	JWT Authentication
Deployment	Azure App Service / Vercel
🚀 Features

✅ Admin login with JWT authentication
✅ Product management (CRUD operations)
✅ Order and user management
✅ Upload product images to Azure Blob Storage
✅ Responsive dashboard with analytics
✅ Modern UI using Tailwind CSS
✅ API-first architecture for scalability

🧩 Architecture
📦 Djualan
 ├── backend/           # ASP.NET Core Web API
 │   ├── Controllers/
 │   ├── Models/
 │   ├── Data/
 │   ├── Services/
 │   └── Program.cs
 ├── frontend/          # ReactJS + Tailwind
 │   ├── src/
 │   ├── components/
 │   ├── pages/
 │   └── App.jsx
 └── README.md

System Flow
React (Frontend)
   ↓ REST API
ASP.NET Core API (Backend)
   ↓
SQL Server / Azure Storage

⚡ Getting Started
🔧 Prerequisites

Make sure you have installed:

Node.js
 (v18 or later)

.NET SDK

SQL Server

🖥️ Backend Setup
cd backend
dotnet restore
dotnet ef database update
dotnet run


The API will start at https://localhost:5001

💻 Frontend Setup
cd frontend
npm install
npm run dev


The frontend runs at http://localhost:5173

Make sure .env contains your API endpoint:

VITE_API_URL=https://localhost:5001/api

🔐 Environment Variables

Example .env for backend:

ConnectionStrings__DefaultConnection=Server=localhost;Database=DjualanDB;Trusted_Connection=True;
Jwt__Key=SuperSecretKeyHere
Azure__BlobConnection=YourAzureBlobConnectionString
Azure__ContainerName=product-images

🧪 Testing

Use tools like Postman or Swagger UI to test API endpoints.
You can access Swagger at:

https://localhost:5001/swagger

☁️ Deployment

Backend: Deploy to Azure App Service or Render

Frontend: Deploy to Vercel / Azure Static Web Apps

Configure CORS for production domains

🖼️ Screenshots (Optional)

You can include images like:

/assets/dashboard.png
/assets/products.png
/assets/login.png


Example section:

### 📸 Dashboard Preview
![Dashboard](./assets/dashboard.png)

🧠 Future Enhancements

✅ Customer storefront (Next.js)

✅ Payment gateway integration (Midtrans / Stripe)

✅ Reporting and analytics dashboard

✅ Role-based access (Admin, Staff)

✅ AI-powered sales insights

👨‍💻 Author

Dwiki Ikhwan
.NET / ASP.NET Developer | Azure Certified | AI-Driven Engineer | Open to Full-Stack & Cloud Development
🔗 LinkedIn: https://www.linkedin.com/in/dwikiikhwan/

📧 dwikiikhwan@outlook.com

📜 License

This project is open-source and available under the MIT License.
