# PesoPinoy: Microfinance & Loan Management System

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

# Installation and Setup

## Clone the Repository
```bash
git clone https://github.com/ZeroPhantom0/Mitochondria_SDG1_PESOPINOY.git
```

## Open the Solution
Navigate to the **CODE** folder and open **PesoPinoy.sln** in **Visual Studio 2026 or later**.

---

## Configure Database Connection

Open `appsettings.json` in the **PesoPinoy.UI** project.

Update the **DefaultConnection** string to match your system's database path:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=C:\\Users\\YourUsername\\source\\repos\\PesoPinoy\\INPUT_DATA\\pesopinoy.db"
  },
  "AppSettings": {
    "ApplicationName": "PesoPinoy Microfinance System",
    "Version": "1.0.0",
    "Company": "PesoPinoy Community Finance"
  }
}
```

---

## Configure Logo Path (Optional)

Place your logo file (`logo.png`) in one of the following locations:

```
PesoPinoy.UI\bin\Debug\logo.png
PesoPinoy.UI\logo.png
PesoPinoy.UI\Images\logo.png
PesoPinoy.UI\Resources\logo.png
PesoPinoy.UI\Assets\logo.png
```

Or update the logo path in **frmLogin.cs** (Recommended):

```csharp
string[] possiblePaths = new string[]
{
    Path.Combine(Application.StartupPath, "logo.png"),
    Path.Combine(Application.StartupPath, "Images", "logo.png"),
    Path.Combine(Application.StartupPath, "Resources", "logo.png"),
    Path.Combine(Application.StartupPath, "Assets", "logo.png"),
    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png"),
    @"C:\Users\YourUsername\source\repos\PesoPinoy\CODE\PesoPinoy.UI\logo.png"
};
```

---

## Run
Run the application from **Visual Studio**.

---

# Using Sample Data

PesoPinoy includes an `initial_seed.json` file with sample data for testing and demonstration purposes.

To load sample data:

1. Login to the system using the default administrator credentials:

```
Username: admin
Password: admin123
```

2. Navigate to the **Backup & Restore** module from the dashboard sidebar.

3. Click the **Browse** button and locate the `initial_seed.json` file in the **INPUT_DATA** folder.

4. Click **Restore** and confirm the operation.

The system will populate with **sample borrowers, loans, payments, savings accounts, and insurance policies** for testing.

---

# System Requirements

- Windows 10 or later  
- .NET 8.0 SDK or later  
- SQLite (included via NuGet packages)  
- Visual Studio 2026 (recommended for development)

---

# Contributors

| Name | Primary Contribution / Assigned Module |
|-----|-----------------------------------------|
| Custorio, April Nicole | Presentation Layer Development, Business Logic Layer, SDAD Documentation, SDAD Diagrams |
| Dela Cruz, Junelle F. | Data Access Layer, Presentation Layer Development, SDAD Diagrams |
| Maningo, Gabriel C. | Business Logic Layer , Data Access Layer Development, SDAD Diagrams |
| Sabiniano, Francis Jr. B. | Data Access Layer Development, SDAD Diagrams |
| Santos, Maureen C. | Presentation Layer Development, Business Logic Layer Development, SDAD Diagrams |

---

**Institutional Affiliation:** National Teachers College, School of Arts, Sciences and Technology  
**Course:** ITELEC 2 | IT Elective 2  
**Instructor:** Prof. Justin Louise R. Neypes  
**Date:** March 2026

---

# Key Features

- **Secure Authentication:** SHA-256 hashed password validation and session management.  
- **Borrower Management:** Complete CRUD operations with borrower profile viewing.  
- **Loan Management:** Automated interest computation and amortization schedule generation.  
- **Payment Tracking:** Real-time balance updates and automated penalty computation.  
- **Savings Management:** Deposit and withdrawal tracking with transaction history.  
- **Insurance Management:** Policy registration, premium payments, and claims processing.  
- **Risk Scoring:** Automated borrower risk classification based on financial profile.  
- **Dashboard Analytics:** Visual charts for collections, loan status, and risk distribution.  
- **Backup & Restore:** JSON-based data export and import with async processing.  
- **Audit Logging:** Comprehensive exception logging and system activity tracking.

---

# Technical Stack

- **Framework:** .NET 8.0 (Windows Forms)  
- **Architecture:** N-Tier (Presentation, Business Logic, Data Access)  
- **ORM:** Entity Framework Core  
- **Database:** SQLite  
- **Charts:** ScottPlot  
- **Authentication:** SHA-256 hashing with salt  
- **Backup:** JSON serialization/deserialization  

---

For more information, refer to the complete documentation in the **DOCUMENTATION** folder.
