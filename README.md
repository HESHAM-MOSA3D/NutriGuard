# Completed Sprint 5

## Meal Logging & Daily Tracking

### Features

- Log Meals
- Log Water Intake
- Log Weight
- Daily Nutrition Summary
- Daily Nutrition History
- Calories Tracking
- Macronutrients Tracking
- Remaining Daily Targets
- Current Weight Tracking
- Food Unit Conversion System
- Food-Specific Unit Conversion
- Authorization Validation
- Input Validation
- Delete Tracking Records

---

### Meal Logging

Users can create meals containing multiple food items.

Each meal stores:

- Meal Type
- Date
- Food Items
- Quantity
- Unit

Each meal item automatically calculates:

- Calories
- Protein
- Carbohydrates
- Fat

---

### Water Tracking

Users can log their daily water intake.

Validation includes:

- Positive values only
- Maximum allowed intake validation

---

### Weight Tracking

Users can record their body weight over time.

Validation includes:

- Positive values only
- Maximum allowed weight validation

---

### Daily Summary

Returns a complete summary for a selected day.

Includes:

- Calories Target
- Calories Consumed
- Calories Remaining

- Protein Target
- Protein Consumed
- Protein Remaining

- Carbohydrates Target
- Carbohydrates Consumed
- Carbohydrates Remaining

- Fat Target
- Fat Consumed
- Fat Remaining

- Water Target
- Water Consumed
- Water Remaining

- Current Weight

- Logged Meals

---

### Daily History

Returns tracking history between two dates.

Optimized to avoid N+1 queries by:

- Loading meals once
- Loading water logs once
- Loading weight once
- Loading nutrition targets once

---

### Food Unit Conversion

The system supports food-specific unit conversions.

Examples:

- Egg → Piece = 55 g
- Apple → Piece = 180 g

Conversions are stored in a dedicated table and loaded automatically during nutrition calculations.

Fallback conversions are available for:

- Gram
- Milliliter
- Cup
- Tablespoon
- Teaspoon
- Piece

---

### Validation

Meal Logging

- Meal must contain at least one item
- Quantity must be greater than zero
- Food must exist
- MealType must be valid

Water Logging

- Amount must be greater than zero
- Maximum allowed amount validation

Weight Logging

- Weight must be greater than zero
- Maximum allowed weight validation

---

### Authorization

Users can only access their own tracking records.

Ownership is validated before:

- Reading
- Deleting

---

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
- FoodCategories
- Foods
- FoodAliases
- FoodUnitConversions
- Recipes
- RecipeIngredients
- FoodPreferences
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
- Food Unit Conversion
- Recipe Database
- Meal Logging
- Water Tracking
- Weight Tracking
- Daily Summary
- Daily History
- CSV Import
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
- Personalized Recommendation Engine
