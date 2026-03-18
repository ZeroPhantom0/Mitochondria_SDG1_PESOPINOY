#  PesoPinoy: Microfinance & Loan Management System

## Project Title and SDG Goal
PesoPinoy is a desktop-based Community Microfinance Loan Management System developed to support Sustainable Development Goal 1 (SDG 1): No Poverty. The system addresses the problem of manual record-keeping in small-scale lending cooperatives and microfinance institutions (MFIs) in the Philippines, which often leads to errors in loan computations, missed payment tracking, and limited financial monitoring. By providing a secure and automated digital platform, PesoPinoy helps strengthen community-based lending institutions and promotes financial inclusion for underserved individuals who cannot access traditional bank loans due to strict requirements.

---

## Project Description
PesoPinoy is built on the .NET / C# platform using a strict N-Tier Architecture, separating presentation, business logic, and data access concerns. This architecture ensures maintainability, scalability, and organized system structure, allowing efficient management of the complete lifecycle of micro-loans, borrower accounts, savings, insurance, and risk analysis through a single cohesive platform.

---

## Architecture Overview

**Presentation Layer**  
Windows Forms-based user interface where administrators manage borrowers, loans, payments, savings, insurance, and view reports.

**Business Logic Layer**  
Core system component handling financial computations, loan processing, risk scoring, and business rule enforcement.

**Data Access Layer**  
Manages database operations using Entity Framework Core with SQLite for persistent storage and JSON files for backup.

---

# ⚠️ IMPORTANT: Installation Requirements

## 🔴 Before You Start
If you don't have Visual Studio installed, you will need to install it first. The system will **NOT** run without the proper development environment.

---

## 📦 Required Software Installation

### 1. Visual Studio 2022 or Later (2026 Recommended)
- Download from: [Visual Studio](https://visualstudio.microsoft.com/downloads/)  
- Choose **Visual Studio Community 2022** (free for students and individuals)  
- During installation, select these workloads:
  - ✅ **.NET Desktop Development**
  - ✅ **ASP.NET and web development** (optional but recommended)

### 2. .NET SDK
- Download from: [Microsoft .NET](https://dotnet.microsoft.com/en-us/download)  
- Install **.NET 8.0 SDK or later**  
- To check installation, run:
```bash
dotnet --version
```

### 3. SQLite
- Included automatically via NuGet packages in Visual Studio

### 4. Git (Optional)
- For cloning the repository: [Git Downloads](https://git-scm.com/downloads)

---

## 📦 Required NuGet Packages

**PesoPinoy.DAL**
- Microsoft.EntityFrameworkCore.Sqlite (v8.0.0+)  
- Microsoft.EntityFrameworkCore.Tools (v8.0.0+)  
- Microsoft.Extensions.Configuration.Json (v8.0.0+)

**PesoPinoy.BLL**
- Newtonsoft.Json (v13.0.3+)

**PesoPinoy.UI**
- ScottPlot.WinForms (v5.0.38+)  
- Microsoft.EntityFrameworkCore.Design (v8.0.0+)  
- Microsoft.Extensions.Configuration.Json (v8.0.0+)  
- Microsoft.Extensions.DependencyInjection (v8.0.0+)

> **Manual installation:**  
Right-click project → Manage NuGet Packages → Browse → Install required packages

---

# Installation and Setup

## Step 1: Clone or Download Repository

**Option A: Using Git**
```bash
git clone https://github.com/ZeroPhantom0/Mitochondria_SDG1_PESOPINOY.git
```

**Option B: Download ZIP**
1. Go to [GitHub Repo](https://github.com/ZeroPhantom0/Mitochondria_SDG1_PESOPINOY)  
2. Click **Code → Download ZIP**  
3. Extract to desired folder (e.g., `C:\Users\YourUsername\source\repos\`)

---

## Step 2: Open the Solution
1. Navigate to **CODE** folder  
2. Open **PesoPinoy.sln**  
3. Trust the project if prompted  
4. Restore NuGet packages if prompted

> **Troubleshooting:** Right-click solution → Restore NuGet Packages

---

## Step 3: Configure Database Connection
Open **appsettings.json** in **PesoPinoy.UI** and update the path:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=C:\\Users\\YOUR_USERNAME\\source\\repos\\Mitochondria_SDG1_PESOPINOY\\INPUT_DATA\\pesopinoy.db"
  },
  "AppSettings": {
    "ApplicationName": "PesoPinoy Microfinance System",
    "Version": "1.0.0",
    "Company": "PesoPinoy Community Finance"
  }
}
```
> Replace `YOUR_USERNAME` with your Windows username. Use `\\` or `/`.

---

## Step 4: Create the Database

**Automatic:** Run the application (F5) – database is created automatically.  

**Manual (if needed):**
1. Open **Package Manager Console**  
2. Set Default Project → `PesoPinoy.DAL`  
3. Run:
```powershell
Add-Migration InitialCreate
Update-Database
```

---

## Step 5: Configure Logo (Optional)
Place `logo.png` in one of these:
- `PesoPinoy.UI\bin\Debug\net8.0-windows\logo.png`  
- `PesoPinoy.UI\logo.png`  
- `PesoPinoy.UI\Images\logo.png`  
- `PesoPinoy.UI\Resources\logo.png`  
- `PesoPinoy.UI\Assets\logo.png`  

> If missing, the login screen hides the logo automatically.

---

## Step 6: Run the Application
1. Set **PesoPinoy.UI** as startup project  
2. Press **F5** to debug or **Ctrl+F5** to run  
3. Login:
```
Username: admin
Password: admin123
```

---

# ⚠️ Common Installation Problems

| Problem | Solution |
|---------|----------|
| Visual Studio not installed | Download & install Visual Studio 2022 Community |
| No .NET SDK found | Install .NET 8.0 SDK |
| NuGet packages missing | Right-click solution → Restore NuGet Packages |
| Database not found | Run app once to create database automatically |
| appsettings.json missing | Ensure it is in PesoPinoy.UI folder |
| Build errors | Verify NuGet packages installed correctly |
| SQLite error | Confirm correct path in appsettings.json and INPUT_DATA folder exists |

---

# Using Sample Data
1. Login as admin  
```
Username: admin
Password: admin123
```
2. Go to **Backup & Restore**  
3. Click **Browse**, select `initial_seed.json` in **INPUT_DATA**  
4. Click **Restore**

---

# System Requirements

**Minimum:**
- Windows 10/11 (64-bit)  
- 1.8 GHz+ CPU, 4 GB RAM, 500 MB free, 1366x768 display

**Software:**
- Visual Studio 2022+  
- .NET 8.0+  
- SQLite (via NuGet)  
- Git (optional)

---

# Users Without Visual Studio
- Obtain published `.exe` from developer  
- Or publish via Visual Studio: Right-click PesoPinoy.UI → Publish → Folder

> Requires .NET 8.0 Runtime only.

---

# Contributors

| Name | Primary Contribution / Assigned Module |
|------|----------------------------------------|
| Custorio, April Nicole | Presentation Layer Development, Business Logic Layer, SDAD Documentation, SDAD Diagrams |
| Dela Cruz, Junelle F. | Data Access Layer, Presentation Layer Development, SDAD Diagrams |
| Maningo, Gabriel C. | Business Logic Layer, Data Access Layer Development, SDAD Diagrams |
| Sabiniano, Francis Jr. B. | Data Access Layer Development, SDAD Diagrams |
| Santos, Maureen C. | Presentation Layer Development, Business Logic Layer Development, SDAD Diagrams |

**Institutional Affiliation:** National Teachers College, School of Arts, Sciences and Technology  
**Course:** ITELEC 2 | IT Elective 2  
**Instructor:** Prof. Justin Louise R. Neypes  
**Date:** March 2026

---

# Key Features
- **Secure Authentication:** SHA-256 hashed passwords  
- **Borrower Management:** CRUD with profile viewing  
- **Loan Management:** Automated interest & amortization  
- **Payment Tracking:** Real-time balance & penalties  
- **Savings Management:** Deposits, withdrawals, history  
- **Insurance Management:** Policies, premiums, claims  
- **Risk Scoring:** Automated borrower classification  
- **Dashboard Analytics:** Charts for collections & risk  
- **Backup & Restore:** JSON-based data handling  
- **Audit Logging:** Exception & activity tracking

---

# Technical Stack
- .NET 8.0 (Windows Forms)  
- N-Tier Architecture  
- Entity Framework Core  
- SQLite  
- ScottPlot charts  
- SHA-256 authentication  
- JSON backup/restore

---

# Need Help?
1. Check **Common Problems** section  
2. Verify Visual Studio & .NET SDK installed  
3. Ensure NuGet packages restored  
4. Confirm database path in `appsettings.json`  
5. Ensure `INPUT_DATA` folder exists  

Refer to **DOCUMENTATION** folder for full details.

---

**Thank you for using PesoPinoy!**

