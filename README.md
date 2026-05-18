# Weather API - Technical Debt Assignment

## Business Requirement

A new endpoint was requested:

GET /weather/summary/report

Deadline: 2 days.

---

# Part 1 — Identified Technical Debt

## 1. Business Logic Inside Controller

### Problem

The controller contains business logic for generating weather reports and manipulating data.

### Why It Is Technical Debt

Controllers should focus on HTTP concerns while business logic should be moved to dedicated services.

### Impact

- Reduced maintainability
- Harder testing
- Higher coupling

### Decision

A lightweight `WeatherReportService` was introduced.

---

## 2. Weak Validation

### Problem

The API accepted invalid values.

Example:

- Missing summary
- Unrealistic temperatures

### Why It Is Technical Debt

Invalid data can break reports and reduce reliability.

### Impact

- Bugs
- Incorrect statistics
- Bad API usage

### Decision

Validation attributes were added.

Example:

```csharp
[Required]
[StringLength(50)]
public string Summary { get; set; }

[Range(-100,100)]
public int TemperatureC { get; set; }
```

---

## 3. Inconsistent API Responses

### Problem

Different endpoints returned different response structures.

Example:

```csharp
return Ok("Deleted.");
```

vs

```csharp
return Ok(new { result = existing });
```

### Why It Is Technical Debt

Inconsistent APIs are harder to consume and maintain.

### Impact

- Poor API usability
- Frontend integration complexity

### Decision

Response formatting was standardized.

---

## Technical Debt Intentionally Left Unfixed

### Static In-Memory Storage

Reason:
Replacing storage with a database would exceed the deadline and introduce unnecessary risk.

### Manual Id Generation

Reason:
Acceptable temporary limitation for a small in-memory demo API.

### Repository Pattern

Reason:
Too large for a 2-day deadline.

---

# Part 2 — Decision Making

## Decision

A combined approach was chosen:

- Deliver the feature
- Fix only the highest-priority technical debt

### Prioritized

- `/weather/summary/report`
- Validation
- Consistent API responses
- Small service extraction

### Deprioritized

- Database integration
- Repository pattern
- Full architecture redesign

### Accepted Risks

- Data loss after restart
- Limited scalability
- Concurrency limitations

---

# Part 3 — Prioritization Backlog

| Item | Type | Priority | Reason |
|------|------|----------|--------|
| Implement weather summary report | Feature | High | Business requirement |
| Add validation | Technical Debt | High | Prevent invalid data |
| Standardize responses | Technical Debt | High | Better API consistency |
| Extract report service | Technical Debt | Medium | Maintainability |
| Replace in-memory storage | Technical Debt | Low | Too large for deadline |
| Repository layer | Technical Debt | Low | Large refactor |
| Global exception handling | Technical Debt | Low | Not critical for delivery |

---

# Part 4 — Stakeholder Communication

We completed the requested weather summary report feature within the deadline.

To balance delivery speed and long-term maintainability, we also addressed a few important quality improvements, such as input validation and response consistency.

Some larger improvements were intentionally postponed because they would significantly increase development time and risk missing the deadline.

The current solution supports the requested functionality and is suitable for the current scope, while additional scalability improvements can be planned later.

---

# Part 5 — If Deadline Becomes 1 Day

The decision changes.

Priority becomes delivering the feature with only minimal fixes.

### Keep

- `/weather/summary/report`
- Basic validation

### Postpone

- Service extraction
- Response standardization
- Refactoring

Trade-off:
Higher technical debt is accepted to meet delivery expectations.

---

# Part 6 — Reflection

## What is technical debt?

Technical debt is the cost of choosing a faster implementation today that may require additional work later.

## Is technical debt always bad?

No. It can be a conscious trade-off to deliver business value faster.

## When is it acceptable?

- Tight deadlines
- MVP development
- Uncertain requirements

## How to reduce technical debt?

- Code reviews
- Refactoring
- Automated tests
- Coding standards
- Time allocation for maintenance
