# Weather API – OpenAPI Contract Assignment

## Implementation Overview

This project extends the default ASP.NET Core Web API template into a CRUD-based Weather API with a fully documented OpenAPI contract.

### Implemented Endpoints

- `GET /weather` — Get all weather records  
- `GET /weather/{id}` — Get weather record by ID  
- `POST /weather` — Create a new weather record  
- `PUT /weather/{id}` — Update an existing weather record  
- `DELETE /weather/{id}` — Delete a weather record  

---

## Features Implemented

- Data validation via DataAnnotations:
  - `[Required]`
  - `[Range]`
  - `[StringLength]`
- Explicit response type annotations (`ProducesResponseType`)
- Proper HTTP status codes:
  - 200 OK
  - 201 Created
  - 400 BadRequest
  - 404 NotFound
- Swagger UI integration
- Scalar integration using the same OpenAPI document
- OpenAPI specification exposed via JSON

---

## OpenAPI Questions

### What is OpenAPI Specification?

The OpenAPI Specification is a standardized, machine-readable format for describing REST APIs.

It defines:

- Available endpoints
- HTTP methods
- Request/response schemas
- Parameters
- Authentication methods
- Status codes
- Content types

It serves as a **contract between backend and frontend systems**.

---

### What Formats Are Supported?

OpenAPI supports two main formats:

#### JSON

{
"openapi": "3.0.1",
"info": {
"title": "Weather API"
}
}


#### YAML

openapi: 3.0.1
info:
title: Weather API


#### Comparison

| JSON | YAML |
|------|------|
| Machine-friendly | Human-friendly |
| More verbose | More readable |
| Widely used in tooling | Common for manual editing |

---

### Why Is OpenAPI Important for Frontend/Backend Collaboration?

OpenAPI improves collaboration by providing a shared contract.

Benefits:

- Clear API expectations
- Defined request/response models
- Enables parallel development
- Reduces integration mismatches
- Supports client generation (SDKs)
- Improves API testing and mocking

---

### What Happens If the Contract Is Incorrect or Outdated?

If the OpenAPI contract is incorrect or outdated:

- Frontend integration breaks
- Generated API clients become invalid
- Documentation becomes misleading
- Contract tests fail
- Runtime bugs occur due to mismatched expectations

---

## Swagger vs Scalar Comparison

| Feature | Swagger UI | Scalar |
|--------|------------|--------|
| Maturity | Industry standard | Modern alternative |
| UI Style | Traditional | Modern / minimal |
| Developer Experience | Widely adopted | More polished UX |
| Ecosystem | Very large | Growing |
| Best Use Case | Standard API docs | Modern developer portals |

---

## Contract Validation Analysis

The API contract is validated by ensuring:

- OpenAPI spec reflects all implemented endpoints
- Request/response models match actual code
- Validation rules are represented via DataAnnotations
- Response codes are explicitly defined
- Swagger and Scalar both consume the same OpenAPI document

### Potential Risks

- Missing annotations may lead to incomplete documentation
- API changes without contract updates cause drift
- Incorrect validation rules lead to integration issues

---

## OpenAPI Endpoints

- Swagger UI: `/swagger`
- Scalar UI: `/scalar/v1`
- OpenAPI JSON: `/swagger/v1/swagger.json`

---

## Summary

This project demonstrates:

- RESTful CRUD API design in ASP.NET Core
- OpenAPI contract definition
- API documentation with Swagger and Scalar
- Proper validation and HTTP semantics
- Contract-first API design principles
