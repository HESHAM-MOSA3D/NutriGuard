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
- Refresh Token Authentication
- SendGrid
- CsvHelper
- Swagger
- Clean Architecture
- Repository Pattern
- Service Layer
- Code First
- PostgreSQL Trigram Search
- Arabic Text Normalization

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

### Features

- ASP.NET Identity
- Custom ApplicationUser
- User Registration
- User Login
- Forgot Password
- Verify OTP
- Change Password
- JWT Authentication
- Refresh Token Authentication
- Refresh Token Rotation
- Logout
- Current User Endpoint
- Protected Endpoints

### APIs

```text
POST /api/Auth/register
POST /api/Auth/login
POST /api/Auth/forgot-password
POST /api/Auth/verify-otp
POST /api/Auth/change-password
POST /api/Auth/refresh
POST /api/Auth/logout
GET  /api/Auth/me
```

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

### Search Features

- Arabic-aware search
- Alias search
- PostgreSQL Trigram Search
- Optimized search performance

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

### Search Features

- Arabic-aware recipe search
- Alias search
- PostgreSQL optimized queries

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

Automatically calculates

- Calories Consumed
- Protein Consumed
- Carbohydrates Consumed
- Fat Consumed
- Water Consumed

Also returns

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

Weight Validation

- Weight must be greater than zero

### Authorization

Users can only access, update, or delete their own tracking records.

### Delete Operations

Implemented APIs for deleting

- Meal Logs
- Water Logs
- Weight Logs

### Food Unit Conversion

Supports

- Piece
- Cup
- Tablespoon
- Teaspoon
- Gram
- Milliliter

Examples

- 1 Egg = 55 g
- 1 Apple = 180 g

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

# Completed Sprint 6

# Nutrition Rules Engine

Sprint 6 introduces an intelligent Nutrition Rules Engine that validates foods, recipes, and complete meals according to each user's health profile.

### Allergy Rules

- Explicit allergy preferences
- Automatic allergen detection
- Milk
- Egg
- Fish
- Gluten
- Soy
- Nuts
- Meat

### Diet Rules

Supports

- Balanced
- LowCarb
- Vegan

Automatically rejects foods that violate the selected diet.

### Goal Rules

Supports

- Lose Weight
- Maintain Weight
- Gain Weight

Provides personalized recommendations according to the user's goal.

### Calories Validation

- Meal Calories
- Remaining Calories
- Daily Target Calories
- Remaining Daily Budget
- High-Calorie Meal Detection

### Macronutrient Validation

Evaluates

- Protein
- Carbohydrates
- Fat

Provides

- Low Protein Warnings
- High Carbohydrate Warnings
- High Fat Warnings
- Diet-aware Recommendations

### Meal Eligibility

Determines whether foods are suitable according to

- Health Profile
- Allergies
- Diet
- Goal
- Food Tags

### Recipe Eligibility

Evaluates complete recipes using

- Ingredients
- Nutrition Rules
- User Preferences

### Traditional Egyptian Food Rules

Special handling for Egyptian dishes including

- Koshary
- Mahshi
- Molokhia
- Ful
- Taameya
- Fiteer

Recommendations adapt according to

- User Goal
- Diet
- Calories
- Macronutrients

### Performance Optimizations

- Arabic-aware search
- PostgreSQL Trigram Indexes
- Optimized filtering
- Bulk loading
- Eliminated N+1 Queries

### APIs

```text
POST /api/Nutrition/validate-meal

GET  /api/Nutrition/check-food/{foodId}

POST /api/Nutrition/eligible-foods

POST /api/Nutrition/eligible-recipes
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
- FoodTags
- FoodTagAssignments

### Recipes

- Recipes
- RecipeIngredients

### Tracking

- MealLogs
- MealItems
- WaterLogs
- WeightLogs

---

# Backend Status

The backend API is feature-complete.

Implemented modules

- Authentication
- Refresh Tokens
- Health Profile
- Food Preferences
- Nutrition Calculator
- Food Database
- Recipe Database
- Meal Tracking
- Water Tracking
- Weight Tracking
- Nutrition Rules Engine
- Traditional Egyptian Food Rules
- Arabic-aware Search
- PostgreSQL Optimization
- Repository Pattern
- Service Layer
- Swagger
- PostgreSQL

---

# Future Roadmap

## Sprint 7

- AI Knowledge Base (RAG)

## Sprint 8

- AI Chat Assistant

## Sprint 9

- AI Meal Planning Agent

## Sprint 10

- AI Insights & Recommendations

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

- Never push directly to master
- Create a feature branch
- Open a Pull Request
- Merge into develop
- Release from develop to master

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
