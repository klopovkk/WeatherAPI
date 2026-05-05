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

After testing both Swagger UI and Scalar against the same OpenAPI document, I observed the following:

### Which UI is Easier to Navigate?

**Scalar** is easier to navigate for me personally.

Reasons:

- Cleaner and more modern interface
- Better visual hierarchy between endpoints
- More readable schema and response rendering
- Feels less cluttered when browsing larger APIs

---

### Which One Would I Use in Production?

It depends on the audience:

- **Swagger UI** — better for internal teams because it is the industry standard and familiar to most developers
- **Scalar** — better for external/public-facing API portals where developer experience and polished presentation matter more

---

### What Differences Did I Notice?

| Aspect | Swagger UI | Scalar |
|--------|------------|--------|
| Visual Design | Traditional / utilitarian | Modern / polished |
| Navigation | Basic expandable sections | Cleaner structured navigation |
| Readability | Good | Better for large schemas |
| Familiarity | Very common in industry | Less common but growing |
| First Impression | Functional | More professional / modern |

---

### Which Helps More When Consuming the API as a Client?

**Scalar** helps more for manual API exploration/documentation reading because:

- Better readability of request/response schemas
- Cleaner endpoint grouping/navigation
- Easier visual scanning of larger specifications

However:

- **Swagger UI** remains more practical in many teams due to familiarity and ecosystem support

---

## Contract Validation Challenge

### Experiment Performed

To demonstrate why OpenAPI is considered a contract, I intentionally created a mismatch between the actual API behavior and the documented contract.

### Change Made

I modified the `GET /weather/{id}` endpoint implementation to return a different response shape than declared in the OpenAPI specification.

Declared contract:

    [ProducesResponseType(typeof(WeatherModel), StatusCodes.Status200OK)]

Actual implementation temporarily returned:

    return Ok(new
    {
        Identifier = weather.Id,
        weather.Date,
        weather.TemperatureC
    });

This removed the `Summary` field and renamed `Id` to `Identifier`, while leaving the documented contract unchanged.

---

### Observed Result

Swagger/Scalar documentation still described the response as:

    {
      "id": 1,
      "date": "2026-05-05T00:00:00",
      "temperatureC": 25,
      "summary": "Hot"
    }

but the actual API returned:

    {
      "identifier": 1,
      "date": "2026-05-05T00:00:00",
      "temperatureC": 25
    }

---

### What Broke

A client generated from the OpenAPI contract expected:

- `id`
- `summary`

but received:

- `identifier`
- no `summary`

This caused runtime deserialization / mapping issues and broke client assumptions.

---

### Why This Demonstrates a Contract

OpenAPI is considered a **contract** because it defines the agreed structure of communication between producer and consumer.

When implementation diverges from that definition:

- Documentation becomes inaccurate
- Generated clients become invalid
- Consumers fail at runtime despite successful compilation

---

### Key Takeaway

A broken OpenAPI contract means:

> The API implementation no longer matches what it promises to consumers.

This demonstrates why maintaining synchronization between implementation and contract is critical in contract-first API development.

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
