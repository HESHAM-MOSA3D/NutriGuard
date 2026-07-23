# NutriGuard

## Overview

NutriGuard is an AI-powered personal nutrition assistant designed specifically for Egyptian users. The system helps users build healthier eating habits through artificial intelligence, nutrition science, and Egyptian food data.

The project follows Clean Architecture principles using ASP.NET Core, Entity Framework Core, PostgreSQL, and ASP.NET Identity.

---

## Technology Stack

- ASP.NET Core 8 Web API
- Entity Framework Core 8
- PostgreSQL
- ASP.NET Identity
- JWT Authentication
- SendGrid Email Service
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

# Completed Sprint 1

## Project Setup

- Created Clean Architecture solution
- Configured four projects
- Dependency Injection
- PostgreSQL configuration
- Entity Framework Core Code First

## Authentication

- ASP.NET Identity
- Custom ApplicationUser
- User Registration
- User Login
- JWT Token Generation
- JWT Authorization
- Protected Endpoints

## Domain

Created the following entities

- ApplicationUser
- HealthProfile

Created the following enums

- Gender
- ActivityLevel
- DietType
- Goal

## Database

- Entity Framework Core Code First
- Initial Migration
- PostgreSQL Database
- One-to-One relationship between ApplicationUser and HealthProfile

## Swagger

- Swagger Documentation
- JWT Authorization Support

---

# Completed Sprint 2

## Password Recovery Module

Implemented a complete password recovery workflow.

### Features

- Forgot Password
- OTP Generation
- OTP Verification
- Password Reset
- Confirm Password Validation
- Email Verification Flow

### Security

- One active OTP per user
- Previous OTPs are automatically invalidated
- OTP expiration
- Failed attempts counter
- Maximum failed attempts protection
- OTP verification before password reset
- Generic responses to prevent email enumeration

### Email Service

- SendGrid Integration
- HTML Email Templates
- Email Service Abstraction
- Transaction support to ensure OTP is not stored unless the email is sent successfully

### Authentication Improvements

- Unified AuthResponseDto
- Authentication message constants
- Improved validation
- Better error handling

---

# Current Database Tables

## Identity Tables

- AspNetUsers
- AspNetRoles
- AspNetUserClaims
- AspNetRoleClaims
- AspNetUserLogins
- AspNetUserRoles
- AspNetUserTokens

## Application Tables

- HealthProfiles
- PasswordResetOtps

---

# Authentication Flow

## Registration

1. Register a new user.
2. Login.
3. Receive JWT Token.
4. Access protected endpoints.

## Password Recovery

1. User requests password reset.
2. System generates a secure OTP.
3. OTP is sent via email.
4. User verifies the OTP.
5. User resets the password.
6. OTP becomes invalid immediately after password reset.

---

# Project Status

Completed

- Clean Architecture
- ASP.NET Identity
- JWT Authentication
- User Registration
- User Login
- Password Recovery
- OTP Verification
- SendGrid Email Integration
- PostgreSQL
- Swagger
- Entity Framework Core Code First

---

# Next Sprint (Sprint 3)

## Health Profile Module

Planned Features

- Create Health Profile
- Update Health Profile
- Get Health Profile
- Complete User Profile
- Validation
- Mapping
- Repository & Service Layer

---

# Future Roadmap

- BMR Calculator
- TDEE Calculator
- Calories Calculator
- Macronutrients Calculator
- Egyptian Food Database
- Food Search
- Meal Logging
- Nutrition Rules Engine
- AI Recommendation Engine
- RAG Integration
- Multi-Agent System
- Voice Assistant
- SignalR Notifications

---

# Team Workflow

The project uses GitHub with feature branches.

Main branch

```
master
```

Feature branch examples

```
feature/auth
feature/password-recovery
feature/health-profile
feature/profile-calculator
feature/meal-logging
feature/rules-engine
feature/rag
```

Developers should never push directly to the main branch.

Each feature must be merged through a Pull Request.

---

# Getting Started

Clone the repository

```bash
git clone https://github.com/HESHAM-MOSA3D/NutriGuard.git
```

Restore packages

```bash
dotnet restore
```

Apply migrations

```bash
dotnet ef database update
```

Run the project

```bash
dotnet run
```

---

# Contributors

- Hesham Mosaad
- Amr Zaghlol
