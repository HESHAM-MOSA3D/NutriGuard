# NutriGuard

## Overview

NutriGuard is an AI-powered personal nutrition assistant designed specifically for Egyptian users. The system helps users build healthier eating habits through a combination of artificial intelligence, nutrition rules, and Egyptian food data.

The project follows Clean Architecture principles using ASP.NET Core and Entity Framework Core with Code First.

---

## Technology Stack

- ASP.NET Core 8 Web API
- Entity Framework Core 8
- PostgreSQL Server
- ASP.NET Identity
- JWT Authentication
- Clean Architecture
- Code First
- Swagger

---

## Solution Structure

```
NutriGuard.API
NutriGuard.Application
NutriGuard.Domain
NutriGuard.Infrastructure
```

---

## Completed Sprint 1

### Project Setup

- Created Clean Architecture solution
- Configured four projects
- Dependency Injection
- PostgreSQL Server configuration

### Authentication

- ASP.NET Identity
- Custom ApplicationUser
- User Registration
- User Login
- JWT Token Generation
- Protected Endpoints

### Domain

Created the following entities:

- ApplicationUser
- HealthProfile

Created the following enums:

- Gender
- ActivityLevel
- DietType
- Goal

### Database

- Entity Framework Core Code First
- Initial Migration
- PostgreSQL Server Database
- One-to-One relationship between ApplicationUser and HealthProfile

### Swagger

- Swagger Documentation
- JWT Authorization Support

---

## Current Database Tables

### Identity Tables

- AspNetUsers
- AspNetRoles
- AspNetUserClaims
- AspNetRoleClaims
- AspNetUserLogins
- AspNetUserRoles
- AspNetUserTokens

### Application Tables

- HealthProfiles

---

## Authentication Flow

1. Register a new user.
2. Login using email and password.
3. Receive JWT token.
4. Use the token to access protected endpoints.

---

## Project Status

Sprint 1 has been completed successfully.

Implemented features:

- Authentication
- JWT
- Identity
- Initial Database
- Clean Architecture Setup

---

## Sprint 2

The next sprint will implement the Health Profile module.

Planned features:

- Create Health Profile
- Update Health Profile
- Get Health Profile
- Complete User Profile

---

## Future Roadmap

- Profile Calculator (BMR & TDEE)
- Calories Calculation
- Macronutrients Calculation
- Egyptian Food Database
- Food Search
- Meal Logging
- Nutrition Rules Engine
- Meal Recommendation
- RAG Integration
- AI Assistant
- Voice Assistant

---

## Team Workflow

The project uses GitHub with feature branches.

Main branch:

```
main
```

Each developer should create a separate feature branch before starting any task.

Examples:

```
feature/auth
feature/health-profile
feature/profile-calculator
feature/meal-logging
feature/rules-engine
feature/rag
```

Developers should never push directly to the main branch.

Each completed feature should be merged through a Pull Request.

---

## Getting Started

Clone the repository

```bash
git clone https://github.com/HESHAM-MOSA3D/NutriGuard.git
```

Restore packages

```bash
dotnet restore
```

Apply database migration

```bash
dotnet ef database update
```

Run the project

```bash
dotnet run
```

---

## Contributors

- Hesham Mosaad
- Amr Zaghlol
