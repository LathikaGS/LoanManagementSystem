# Loan Management System

A secure full-stack Loan Management System developed as a capstone project using ASP.NET Core Web API and Angular. The application manages the complete loan lifecycle from application to approval, EMI tracking, and loan closure with role-based access control.

---

## Project Overview

The system digitizes loan processing for banks and financial institutions by providing online loan applications, approval workflows, EMI management, and reporting dashboards. It ensures data security using JWT authentication and follows a layered architecture.

---

## Technology Stack

### Backend

* ASP.NET Core Web API (.NET 8/9)
* Entity Framework Core
* LINQ
* SQL Server
* JWT Authentication
* Swagger / OpenAPI

### Frontend

* Angular (Latest)
* TypeScript
* Angular Material / Bootstrap
* Reactive and Template-Driven Forms

---

## User Roles

* Admin: Manage users, loan types, interest rates, and system configuration
* Loan Officer: Review loan applications, approve or reject loans, generate EMI schedules
* Customer: Apply for loans, track loan status, view EMI details

---

## System Architecture

The application follows a layered architecture:

* Angular frontend with role-based layouts (Admin, Loan Officer, Customer)
* ASP.NET Core Web API handling business logic through controllers
* JWT-secured communication between frontend and backend
* Entity Framework Core for data access
* SQL Server as the persistent data store

Data Flow:
Frontend → JWT Secured API → Controllers → Entity Framework Core → SQL Server

---

## Core Features

* User registration and login
* Role-based authorization
* Online loan application management
* Loan approval and rejection workflow
* Automatic EMI calculation and schedule generation
* EMI repayment and outstanding balance tracking
* Reports and dashboards

---

## Loan Status Flow

Applied → Under Review → Approved → Rejected → Closed

---

## LINQ Usage

* Retrieve loans by status
* Calculate total outstanding amount
* Generate monthly EMI and overdue reports

---

## Backend Controllers and Services

### Controllers (Backend)

* AdminController
* AuthController
* EMIController
* LoanApplicationController
* LoanOfficerController
* LoanTypeController
* ReportsController

### Services

* EmiCalculator

---

## Frontend Modules and Components

### Admin Module

* AdminLayoutComponent
* DashboardComponent
* PendingUsersComponent
* AllUsersComponent
* LoanTypesComponent

### Customer Module

* CustomerLayoutComponent
* CustomerDashboardComponent
* ApplyLoanComponent
* CustomerEmiComponent

### Loan Officer Module

* LoanOfficerLayoutComponent
* ReportsDashboardComponent
* ReportsSummaryComponent
* OverdueEmisComponent
* LoanOfficerComponent

---

## Validation Rules

* Loan amount must be within limits defined for each loan type
* Tenure must not exceed the maximum tenure for the loan type
* EMI is auto-calculated using standard formula
* UI access is restricted based on user roles

---

## Testing

* Unit testing using xUnit
* API testing using Postman
* Authentication and authorization testing

---

## How to Run the Project

### Backend

```
dotnet restore
dotnet ef database update
dotnet run
```

### Frontend

```
npm install
ng serve
```

---

## Deliverables

* GitHub repository (Backend and Frontend)
* Database schema
* UI screenshots
* Final demo presentation

---

