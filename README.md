## Objective

The goal of this assignment is to understand how to identify, prioritize, and manage technical debt in a real-world scenario.

You will practice making engineering decisions under constraints, balancing business needs with code quality.

The codebase you are working with is your existing Weather API. It is functional, but it has accumulated several issues over time.

---

## Business Requirement

The business has requested a new feature:

- **New endpoint:** GET /weather/summary/report
- **Deadline:** 2 days (Teorical, the business or client company will require this)

---

## Part 1: Identify Technical Debt

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

## Part 2: Decision Making

## Decision

I decided to combine:
- feature delivery
- minimal high-impact refactoring

The goal was to deliver the business feature while improving the most critical quality issues without risking the deadline.

---

## What I Prioritized

### Business Requirement

The new endpoint `/weather/summary/report` had the highest priority because it was the primary business request.

### High-Impact Technical Debt

I also addressed:
- validation
- inconsistent API responses
- report business logic extraction

These changes provide long-term value while remaining safe within the deadline.

Explain your decision:

## What Did You Prioritize and Why?

I prioritized the implementation of the new `/weather/summary/report` endpoint because it was the primary business requirement and had a strict deadline.

I also prioritized fixing several high-impact technical debt issues that could be improved safely without major architectural changes:
- model validation
- response consistency
- extracting report logic into a small service

These improvements increase maintainability and API reliability while keeping delivery risk low.

---

## What Did You Intentionally Leave Unfixed?

I intentionally left the following issues unfixed:
- in-memory static storage
- repository pattern implementation
- database integration
- authentication and authorization
- global exception handling
- full architectural redesign

These improvements would require significantly more time and could risk missing the deadline.

---

## What Risks Are You Accepting by Making That Choice?

By limiting the refactoring scope, I accept several risks:
- data loss after application restart
- limited scalability
- possible concurrency issues with manual ID generation
- some maintainability problems remaining in the codebase

However, these risks are acceptable for the current assignment scope because the application remains functional, readable, and capable of delivering the required business feature on time.
---

## Part 3: Prioritization Backlog

Create a backlog table with all items — both technical debt and the new feature:

| Item | Type | Priority | Reason |
|------|------|----------|--------|
| Implement `/weather/summary/report` | Feature | High | Business requirement |
| Add model validation | Technical Debt | High | Prevent invalid data |
| Standardize API responses | Technical Debt | High | Better API consistency |
| Extract report logic into service | Technical Debt | Medium | Improve maintainability |
| Add global exception handling | Technical Debt | Medium | Reliability improvement |
| Replace anonymous DTOs | Technical Debt | Medium | Stronger API contracts |
| Replace in-memory storage with database | Technical Debt | Low | Large refactor |
| Implement repository pattern | Technical Debt | Low | Architectural improvement |
| Add authentication | Feature/Security | Low | Not required currently |
---

## Part 4: Stakeholder Communication

We completed the requested weather summary reporting feature within the deadline.

To balance delivery speed and long-term maintainability, we also implemented several focused quality improvements, including validation and cleaner API structure.

Some larger infrastructure improvements were intentionally postponed because they would significantly increase development time and create a risk of missing the deadline.

The current implementation fully supports the requested functionality and is appropriate for the current project scope, while additional scalability improvements can be planned in future iterations.


## Part 5: Changing Constraints

Assume the deadline has been reduced to **1 day**.

Answer:

- Does your decision change?
- What do you adjust?
- What do you deprioritize?

# Part 5 — Changing Constraints (1 Day Deadline)

## Does the Decision Change?

Yes.

With only 1 day available, the priority becomes delivering the business feature as quickly and safely as possible.

The focus shifts from balancing feature delivery and code quality improvements to primarily ensuring that the required endpoint is completed on time.

---

## What Do You Adjust?

I would reduce the scope of refactoring and apply only minimal safe improvements.

The implementation would focus on:
- delivering `/weather/summary/report`
- keeping the solution simple
- adding only essential validation

I would avoid changes that require larger architectural modifications or introduce additional delivery risk.

---

## What Do You Deprioritize?

I would postpone:
- service layer extraction
- response standardization
- repository pattern implementation
- architectural cleanup
- global exception handling improvements

These tasks improve long-term maintainability but are less critical than delivering the requested functionality within the shortened deadline.

---

## Trade-Off

By reducing refactoring efforts, more technical debt is accepted in exchange for meeting the delivery deadline.

This decision increases future maintenance costs slightly, but it minimizes the risk of failing to deliver the required feature on time.
---

## Part 6: Reflection

Answer the following questions:

- What is technical debt?
- Is technical debt always bad?
- When is it acceptable to take on technical debt?
- How can a team prevent accumulating more technical debt over time?

## What Is Technical Debt?

Technical debt is the cost of choosing a faster or simpler implementation today that may require additional work in the future.

It usually appears when development speed is prioritized over long-term code quality, architecture, or maintainability.

Examples include:
- quick fixes
- duplicated code
- missing tests
- temporary architectural decisions

---

## Is Technical Debt Always Bad?

No.

Technical debt is not always negative. In many situations, it is a conscious engineering trade-off that helps deliver business value faster.

For example:
- meeting a strict deadline
- building an MVP
- validating a business idea quickly

The problem begins when technical debt is ignored for too long and continues accumulating without proper management.

---

## When Is It Acceptable to Take on Technical Debt?

Technical debt is acceptable when:
- deadlines are strict
- requirements are uncertain
- rapid delivery is more important than perfect architecture
- building prototypes or MVPs
- experimenting with new features

However, the trade-offs should always be understood, documented, and planned for future improvement.

---

## How Can a Team Prevent Accumulating More Technical Debt Over Time?

Teams can reduce technical debt accumulation by:
- performing regular code reviews
- writing automated tests
- following coding standards
- refactoring continuously
- maintaining clean architecture
- allocating time for maintenance tasks
- documenting technical decisions
- monitoring code quality with static analysis tools

Preventing technical debt is mainly about maintaining engineering discipline while balancing business priorities.

---

## Deliverables

You must submit:

1. Updated source code (apply whatever changes you decided to make)
2. An updated README.md that includes:
   - Identified technical debt items
   - Your decision and justification
   - Prioritization backlog table
   - Stakeholder message
   - Answers to Part 5 and Part 6

---

## Success Criteria

- Clear and specific identification of the existing technical debt
- Logical and well-justified decisions
- Demonstrated ability to balance business needs with code quality
- Clear communication aimed at a non-technical audience

--
