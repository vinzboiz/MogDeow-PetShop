## 🐾 MogDeow PetShop — ASP.NET Core pet shop web app

MogDeow PetShop is a web application for selling pets & accessories, built with **ASP.NET Core MVC** and **SQL Server**.  
Users can browse products by category (dog/cat/accessories), manage their cart, place orders, rate products, add favorites, and admins have a dedicated area to manage content.

---

## 📋 Table of contents

- [Quick start](#-quick-start)
- [Features](#-features)
- [Tech stack](#-tech-stack)
- [Detailed setup](#-detailed-setup)
- [Configuration](#-configuration)
- [Project structure](#-project-structure)
- [Main flows](#-main-flows)
- [Build & scripts](#-build--scripts)
- [Common issues](#-common-issues)
- [Contributing](#-contributing)
- [License & contact](#-license--contact)

---

## 🚀 Quick start

Requirements:

- **.NET 8+ SDK** (or the version that matches `MOGDEOW.csproj`)
- **SQL Server** (Express/Developer is fine)
- (Optional) **Node.js 18+** if you want to rebuild front‑end assets

Clone the project and run:

```bash
# 1. Clone
git clone <your-github-repo-url>
cd MOGDEOW

# 2. Go into the main web app folder
cd MOGDEOW/MOGDEOW

# 3. Update the connection string in appsettings.Development.json (see Configuration section)

# 4. Apply database migrations (if you use EF Core migrations)
dotnet ef database update   # or run from Package Manager Console in Visual Studio

# 5. Run the app
dotnet run
# → usually runs at http://localhost:5000 or the port assigned by Kestrel
```

Open your browser and navigate to the address where the app is listening (for example: `http://localhost:5000`).

---

## ✨ Features

| Area | Description |
|------|-------------|
| **Home page** | Main banner, randomly shows products in groups: dog, cat, accessories. Optimized image display with fallback when an image is missing. |
| **Product listing** | Filter/view products by type (dog/cat/accessories), quickly see price, stock and images. |
| **Product detail** | Full information: name, price, description, multiple images, product type, ratings. |
| **Cart** | Add/remove/update quantity of items in the cart, calculate totals, prepare for checkout. |
| **Orders & bills** | Create orders from the cart, store bill and bill details, track purchase history. |
| **Ratings & favorites** | Users can rate products and add them to a favorites list to view later. |
| **Accounts & authentication** | Register/login, persist user info, session‑based auth, manage user profile. |
| **Admin area** | Manage products, product types, users, orders, ratings… through controllers like `UserManage`, `ProductManage`. |
| **Supporting APIs** | Several actions return JSON to be consumed by front‑end or other clients, with Swagger available in Development. |

---

## 🛠 Tech stack

| Layer | Technologies |
|-------|-------------|
| **Backend** | ASP.NET Core MVC, .NET, Dependency Injection |
| **Database** | SQL Server, Entity Framework Core |
| **View & UI** | Razor Views, Layout & Partial Views, HTML/CSS/JS, (Bootstrap or other CSS libraries if configured in `wwwroot`) |
| **Auth & Session** | ASP.NET Core Session, Cookies |
| **Docs & Dev tools** | Swagger (Development only), Newtonsoft.Json, CORS |

---

## 📦 Detailed setup

### 1. Clone repo

```bash
git clone <your-github-repo-url>
cd MOGDEOW
```

Solution structure:

- `MOGDEOW.sln` at `MOGDEOW/`
- Main web project at `MOGDEOW/MOGDEOW/`

### 2. Create SQL Server database

Create a database (for example `MOGDEOW_DB`) using SQL Server Management Studio or a script:

```sql
CREATE DATABASE MOGDEOW_DB;
```

> You can change the database name, but it must match the connection string in `appsettings.json` / `appsettings.Development.json`.

### 3. Update connection string

Open `appsettings.Development.json` in the web project folder (`MOGDEOW/MOGDEOW`) and update the `DefaultConnection` string to match your SQL Server instance (see more in [Configuration](#-configuration)).

### 4. Apply migrations (if any)

In the web project folder:

```bash
cd MOGDEOW/MOGDEOW

# If dotnet-ef is installed
dotnet ef database update
```

Or use **Package Manager Console** in Visual Studio:

```powershell
Update-Database
```

### 5. Run the app

```bash
dotnet run
```

Or open `MOGDEOW.sln` in Visual Studio / Rider and press **Run/Debug**.

---

## 🔐 Configuration

The app uses the **ConnectionString** `DefaultConnection` for `MogdeowContext`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MOGDEOW_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

Depending on environment, you can use:

- `Server=localhost,1433;User Id=sa;Password=<password>;Database=MOGDEOW_DB;TrustServerCertificate=True`
- Or any other connection string as long as SQL Server is reachable.

The app also:

- Enables **Session** with a 30‑minute idle timeout.
- Enables **CORS** policy `"AllowAll"` for easier development.
- Enables **Swagger UI** when `ASPNETCORE_ENVIRONMENT=Development`.

---

## 📁 Project structure

```text
MOGDEOW/
├── MOGDEOW.sln
└── MOGDEOW/
    ├── MOGDEOW.csproj          # ASP.NET Core MVC project
    ├── Program.cs              # DI configuration, middleware, routing, Swagger
    ├── appsettings*.json       # Configuration, connection strings
    ├── Attributes/             # Custom attributes (authorize, validate,...)
    ├── Controllers/            # Home, Product, Cart, Order, User, ...
    ├── Data/                   # DbContext (MogdeowContext)
    ├── Extensions/             # Extension methods, service configuration
    ├── Helpers/                # Utility helpers (email, formatting,...)
    ├── Migrations/             # EF Core migrations
    ├── Models/                 # Entities: Product, User, Bill, Rating, ...
    ├── Services/               # Business logic: ProductService, UserService, ...
    ├── Views/                  # Razor Views (Home, Product, Cart, Shared, ...)
    ├── wwwroot/                # Static files: CSS, JS, images
    ├── node_modules/           # (Optional) front‑end packages
    ├── package.json            # Scripts to build front‑end assets (if used)
    └── package-lock.json
```

---

## 🔄 Main flows

- **Browse products**
  - `HomeController.Index` calls `IProductService` to fetch products by category (`dog`, `cat`, `accessories`), randomizes them and exposes them via `ViewBag`.
  - Views render the products with images (prefer existing images, fall back if missing).

- **Cart & checkout**
  - `CartController` works with `CartDetail` via `ICartDetailService`.
  - On checkout confirmation, `Bill` and `BillDetail` are created in the database.

- **Accounts & users**
  - `AccountController` / `UserController` handle login, registration and profile updates.
  - Session is used to store information about the current user.

- **Ratings & favorites**
  - `RatingController` and `FavoriteController` handle logic related to `Rating` and `Favorite` tables.

---

## 🧱 Build & scripts

Depending on your setup, common commands are:

**Backend (.NET project)**

| Command | Description |
|---------|-------------|
| `dotnet run` | Run the web app in Development mode |
| `dotnet build` | Build the project |
| `dotnet ef database update` | Apply migrations to the current database |

**Front‑end assets (if `package.json` is used)**

```bash
cd MOGDEOW/MOGDEOW
npm install
npm run build    # Depending on package.json, builds CSS/JS into wwwroot
```

---

## 🔧 Common issues

| Symptom | How to fix |
|---------|-----------|
| **Cannot connect to SQL Server** | Check the `DefaultConnection` connection string, ensure SQL Server is running, the DB exists, and the account has access. Try `TrustServerCertificate=True` if using a self‑signed SSL certificate. |
| **Migration errors / `__EFMigrationsHistory`** | Drop the test DB, create a fresh one and run `dotnet ef database update`. Make sure the EF Core tools version matches the project. |
| **Static files (images) not showing** | Verify the `ProductImage` paths and `StaticFileOptions` configuration in `Program.cs`. Images are usually served from `wwwroot/src/Assets` or other `wwwroot` folders. |
| **CORS errors when calling API from another domain** | The app configures `"AllowAll"` CORS; if you change it, ensure the front‑end origin (e.g. `http://localhost:3000`) is allowed. |
| **Swagger UI not visible** | Swagger is enabled only when `app.Environment.IsDevelopment()` is `true`. Ensure `ASPNETCORE_ENVIRONMENT=Development`. |

---

## 🤝 Contributing

1. **Fork** this repository on GitHub and clone it locally.
2. Create a new branch: `git checkout -b feature/your-feature-name`.
3. Install & run the project following [Detailed setup](#-detailed-setup).
4. Commit your changes with clear messages, then `git push` your branch.
5. Open a **Pull Request** into `main` (or the primary branch), describing what you changed and why.

For bug reports or feature requests, please open an **Issue** on GitHub.

---

## 📄 License & contact

- **License:** (Update with the license you want to use, e.g. MIT/ISC).
- **Contact / Bug reports / Feature requests:** please open an **Issue** or **Pull Request** directly on the project’s GitHub repository.

*MogDeow PetShop — built with ASP.NET Core MVC & SQL Server.*

