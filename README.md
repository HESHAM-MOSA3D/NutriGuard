# NutriGuard

## Overview

NutriGuard is an AI-powered personal nutrition assistant designed specifically for Egyptian users. The system helps users build healthier eating habits through artificial intelligence, nutrition science, and authentic Egyptian food data.

The project follows Clean Architecture principles using ASP.NET Core, Entity Framework Core, PostgreSQL, and ASP.NET Identity.

---

# Technology Stack

- ASP.NET Core 8 Web API
- Entity Framework Core 8
- PostgreSQL
- ASP.NET Identity
- JWT Authentication
- SendGrid
- CsvHelper
- Swagger
- Clean Architecture
- Repository Pattern
- Service Layer
- Code First

---

# Solution Structure

```text
NutriGuard.API
NutriGuard.Application
NutriGuard.Domain
NutriGuard.Infrastructure
```

---

# Completed Sprint 1

## Project Setup

- Clean Architecture
- Dependency Injection
- PostgreSQL Configuration
- Entity Framework Core Code First
- Initial Migration

## Authentication

- ASP.NET Identity
- Custom ApplicationUser
- User Registration
- User Login
- Forgot Password
- Verify OTP
- Change Password
- JWT Authentication
- Protected Endpoints

---

# Completed Sprint 2

## Health Profile

### Features

- Create Health Profile
- Get Health Profile
- Update Health Profile
- Delete Health Profile
- Food Preferences
- Profile Completion Status

### Validation

- Height Validation
- Weight Validation
- Date of Birth Validation
- Age Validation
- Enum Validation
- One Health Profile per User

---

# Completed Sprint 3

## Nutrition Calculator

Automatically calculates personalized nutrition targets based on the user's health profile.

### Features

- BMR Calculation
- TDEE Calculation
- Daily Calories Target
- Macronutrient Targets
- Goal-Based Adjustments
- Diet-Based Adjustments
- Automatic Target Recalculation

### API

```text
GET /api/nutrition-target
```

Returns

- BMR
- TDEE
- Daily Calories
- Protein Target
- Carbohydrates Target
- Fat Target

---

# Completed Sprint 4

## Food Database

### Features

- Food Entity
- Food Categories
- Food Nutritional Values
- Food Aliases
- Egyptian Food Dataset Import (CSV)

### APIs

```text
GET /api/foods
GET /api/foods/{id}
GET /api/foods/search
GET /api/foods/categories
```

---

## Recipe Database

### Features

- Recipe Entity
- RecipeIngredient Entity
- Egyptian Recipes Dataset Import (CSV)
- Recipe Search
- Recipe Details

### APIs

```text
GET /api/recipes
GET /api/recipes/{id}
```

Each recipe contains

- Ingredients
- Quantities
- Units
- Preparation Instructions
- Description
- Preparation Time
- Servings

Recipes are fully linked with the Food database.

---

# Completed Sprint 5

## Meal & Nutrition Tracking

Sprint 5 introduces comprehensive daily nutrition tracking and monitoring.

### Features

### Meal Logging

- Log meals by meal type
- Multiple food items per meal
- Automatic calorie calculation
- Automatic macronutrient calculation
- Unit-aware food quantity conversion

### Water Tracking

- Log daily water intake
- Daily water consumption summary

### Weight Tracking

- Log body weight
- Retrieve latest recorded weight

### Daily Summary

Automatically calculates:

- Calories Consumed
- Protein Consumed
- Carbohydrates Consumed
- Fat Consumed
- Water Consumed

Also returns:

- Remaining Calories
- Remaining Protein
- Remaining Carbohydrates
- Remaining Fat
- Remaining Water

### Daily History

Retrieve daily nutrition summaries for any date range.

Optimized to avoid N+1 database queries.

### Input Validation

Meal Validation

- Meal must contain at least one food item
- Quantity must be greater than zero
- Food must exist
- Meal type validation

Water Validation

- Water amount must be greater than zero
- Maximum allowed water intake validation

Weight Validation

- Weight must be greater than zero
- Maximum allowed weight validation

### Authorization

Users can only access, update, or delete their own tracking records.

### Delete Operations

Implemented APIs for deleting:

- Meal Logs
- Water Logs
- Weight Logs

### Food Unit Conversion System

NutriGuard now supports food-specific unit conversions.

Instead of using generic assumptions (e.g., every piece = 100g), the system uses a dedicated FoodUnitConversion table.

This enables accurate conversions for units such as:

- Piece
- Cup
- Tablespoon
- Teaspoon
- Gram
- Milliliter

Examples:

- 1 Egg = 55 g
- 1 Apple = 180 g
- Future foods can be added without changing application code.

Conversion data is seeded automatically during application startup.

### APIs

```text
POST   /api/tracking/meals
POST   /api/tracking/water
POST   /api/tracking/weight

GET    /api/tracking/summary/{date}
GET    /api/tracking/history

DELETE /api/tracking/meals/{id}
DELETE /api/tracking/water/{id}
DELETE /api/tracking/weight/{id}
```

---

# Current Database Tables

## Identity

- AspNetUsers
- AspNetRoles
- AspNetUserClaims
- AspNetRoleClaims
- AspNetUserLogins
- AspNetUserRoles
- AspNetUserTokens

## Application

- HealthProfiles
- PasswordResetOtps

### Food

- FoodCategories
- Foods
- FoodAliases
- FoodUnitConversions

### Recipes

- Recipes
- RecipeIngredients

### Tracking

- MealLogs
- MealItems
- WaterLogs
- WeightLogs

---

# Project Status

Completed

- Clean Architecture
- ASP.NET Identity
- JWT Authentication
- Health Profile
- Food Preferences
- Nutrition Calculator
- Food Database
- Food Categories
- Food Aliases
- Recipe Database
- CSV Import
- Meal Tracking
- Water Tracking
- Weight Tracking
- Daily Summary
- Daily History
- Food Unit Conversion
- Repository Pattern
- Service Layer
- Swagger
- PostgreSQL

---

# Next Sprint

## Sprint 6 — Nutrition Rules Engine

Planned Features

- Nutrition Rules Engine
- Allergy Rules
- Medical Condition Rules
- Diet Rules
- Goal Rules
- Personalized Nutrition Recommendations

---

# Completed Sprint 6

## Nutrition Rules Engine

An intelligent rules engine evaluating food choices and meals against user health profiles, goals, allergens, diets, and traditional Egyptian food rules.

### Features

- **Allergy Rules**: Automatic allergen detection (Milk, Egg, Gluten, Nuts, Fish, Soy) and user preference matching.
- **Diet Rules**: Enforces Vegan, LowCarb, and Balanced diet compliance.
- **Goal Rules**: Aligns meal choices with user weight loss, weight gain, or maintenance goals.
- **Calories Validation**: Real-time evaluation against remaining daily budget & single-meal thresholds.
- **Macronutrient Validation**: Evaluates protein, carbohydrate, and fat balance.
- **Meal & Recipe Eligibility Filtering**: Pre-screening APIs for safe food and recipe choices.
- **Traditional Egyptian Food Rules**: Culturally tailored advice and portion recommendations for Egyptian dishes (Koshary, Fiteer, Mahshi, Ful, Taameya, Molokhia, etc.).

### APIs

```text
POST /api/nutrition/validate-meal
GET  /api/nutrition/check-food/{foodId}
POST /api/nutrition/eligible-foods
POST /api/nutrition/eligible-recipes
```

---

# Future Roadmap

## Sprint 7

- AI Knowledge Base (RAG)
- PostgreSQL pgvector / ChromaDB
- Embeddings
- Semantic Search

## Sprint 8

- AI Conversation
- Chat API
- Tool Calling

## Sprint 9

- Meal Planning Agent

## Sprint 10

- Insights & Motivation

## Sprint 11

- SignalR Notifications

## Sprint 12

- Voice Assistant

## Sprint 13

- Preference Memory

## Sprint 14

- Optimization & Deployment

---

# Team Workflow

Git Flow

```text
master
develop

feature/authentication
feature/health-profile
feature/nutrition-calculator
feature/meal-tracking
feature/rules-engine
feature/rag
```

Rules

- Never push directly to `master`
- Create a feature branch
- Open a Pull Request
- Merge into `develop`
- Release from `develop` to `master`

---

# Getting Started

Clone

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

Run

```bash
dotnet run
```

---

# Contributors

- Hesham Mosaad
- Amr Zaghlol
