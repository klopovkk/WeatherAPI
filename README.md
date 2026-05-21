






























WeatherForecastController (1).cs
Файл

WeatherModel (1).cs
Файл
# Assignment: Managing Technical Debt

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

Analyze the current codebase and list **technical debt issues**.

For each issue, explain:

- What the problem is
- Why it is considered technical debt
- What the potential impact is (maintainability, bugs, security, performance, etc.)

---

## Part 2: Decision Making

You must decide how to proceed given the deadline:

- Refactor first, then deliver the feature
- Deliver the feature first, then refactor
- Combine both

Explain your decision:

- What did you prioritize and why?
- What did you intentionally leave unfixed?
- What risks are you accepting by making that choice?

---

## Part 3: Prioritization Backlog

Create a backlog table with all items — both technical debt and the new feature:

| Item | Type | Priority | Reason |
|------|------|----------|--------|

---

## Part 4: Stakeholder Communication

Write a short message (5–10 lines) to a non-technical stakeholder explaining your decision.

Your message should:

- Be clear and free of technical jargon
- Justify your priorities
- Acknowledge the trade-offs you are accepting

---

## Part 5: Changing Constraints

Assume the deadline has been reduced to **1 day**.

Answer:

- Does your decision change?
- What do you adjust?
- What do you deprioritize?

---

## Part 6: Reflection

Answer the following questions:

- What is technical debt?
- Is technical debt always bad?
- When is it acceptable to take on technical debt?
- How can a team prevent accumulating more technical debt over time?

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

---

## Important

There is no single correct answer.

What matters is:

- How you think through the problem
- How you justify your decisions
- How you handle trade-offs under pressure

Я бы сделал минимальные, безопасные изменения, которые выглядят профессионально для assignment-а и при этом соответствуют условию “2 дня deadline”.

То есть: не перестраиваем архитектуру, но закрываем самые болезненные техдолги.

Что я бы реально исправил
1. Validation (HIGH priority)
Сейчас API может принять мусор.

Исправляем:

using System.ComponentModel.DataAnnotations;

namespace WeatherAPI
{
    public class WeatherModel
    {
        public int Id { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Range(-100, 100)]
        public int TemperatureC { get; set; }

        [Required]
        [StringLength(50)]
        public string Summary { get; set; } = string.Empty;
    }
}
Почему:

маленькое изменение

высокий impact

выглядит как зрелое инженерное решение

2. Consistent API Responses (HIGH priority)
Сейчас:

return Ok("Deleted.");
и

return StatusCode(201, "Record created successfully.");
Плохо.

Сделаем единый формат:

return Ok(new
{
    message = "Deleted",
    success = true
});
3. Extract report logic into service (MEDIUM priority)
Самый важный architectural debt.

Не Clean Architecture.

Просто сервис.

Структура:

Services/
 ├── WeatherReportService.cs

DTOs/
 ├── WeatherSummaryReportDto.cs
Итоговая структура
Controllers/
 └── WeatherForecastController.cs

Services/
 └── WeatherReportService.cs

DTOs/
 └── WeatherSummaryReportDto.cs

WeatherModel.cs
DTO
namespace WeatherAPI.DTOs
{
    public class WeatherSummaryReportDto
    {
        public int TotalRecords { get; set; }

        public double AverageTemperature { get; set; }

        public WeatherModel? Hottest { get; set; }

        public WeatherModel? Coldest { get; set; }

        public string? MostFrequentCondition { get; set; }
    }
}
Service
using WeatherAPI.DTOs;

namespace WeatherAPI.Services
{
    public class WeatherReportService
    {
        public WeatherSummaryReportDto GenerateReport(List<WeatherModel> records)
        {
            return new WeatherSummaryReportDto
            {
                TotalRecords = records.Count,
                AverageTemperature = records.Any()
                    ? records.Average(x => x.TemperatureC)
                    : 0,

                Hottest = records
                    .OrderByDescending(x => x.TemperatureC)
                    .FirstOrDefault(),

                Coldest = records
                    .OrderBy(x => x.TemperatureC)
                    .FirstOrDefault(),

                MostFrequentCondition = records
                    .GroupBy(x => x.Summary)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault()
            };
        }
    }
}
Updated Controller
using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers
{
    [ApiController]
    [Route("weather")]
    [Produces("application/json")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly List<WeatherModel> WeatherRecords = new();

        private readonly WeatherReportService _weatherReportService;

        public WeatherForecastController(
            WeatherReportService weatherReportService)
        {
            _weatherReportService = weatherReportService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                data = WeatherRecords,
                total = WeatherRecords.Count
            });
        }

        [HttpGet("{weatherId:int}")]
        public IActionResult GetById(int weatherId)
        {
            var model = WeatherRecords
                .FirstOrDefault(e => e.Id == weatherId);

            return model == null
                ? NotFound(new
                {
                    success = false,
                    message = "Record not found."
                })
                : Ok(model);
        }

        [HttpPost]
        public IActionResult Create(
            [FromBody] WeatherModel weatherData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            weatherData.Id = WeatherRecords.Count == 0
                ? 1
                : WeatherRecords.Max(e => e.Id) + 1;

            WeatherRecords.Add(weatherData);

            return StatusCode(201, new
            {
                success = true,
                message = "Record created successfully."
            });
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(
            int id,
            [FromBody] WeatherModel obj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = WeatherRecords
                .FirstOrDefault(x => x.Id == id);

            if (existing == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Record not found."
                });
            }

            existing.Date = obj.Date;
            existing.TemperatureC = obj.TemperatureC;
            existing.Summary = obj.Summary;

            return Ok(new
            {
                success = true,
                result = existing
            });
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var weather = WeatherRecords
                .FirstOrDefault(x => x.Id == id);

            if (weather == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Record not found."
                });
            }

            WeatherRecords.Remove(weather);

            return Ok(new
            {
                success = true,
                message = "Deleted."
            });
        }

        [HttpGet("summary/report")]
        public IActionResult GetReport()
        {
            var report =
                _weatherReportService
                    .GenerateReport(WeatherRecords);

            return Ok(report);
        }
    }
}
Program.cs
Добавь DI:

builder.Services.AddScoped<WeatherReportService>();
README.md (готовый)
Сделай файл примерно такого вида:

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
Так это будет выглядеть как работа мидла/стронг мидла, а не “я просто написал эссе”.


дай мне ответы на все вопросы поставленные в тз

Part 1 — Identify Technical Debt
1. Business Logic Inside Controller
Problem
The controller contains business logic for:

generating reports

calculating averages

grouping weather summaries

generating IDs

Example:

var avgTemp = WeatherRecords.Count > 0
    ? WeatherRecords.Average(x => x.TemperatureC)
    : 0;
Why It Is Technical Debt
Controllers should handle HTTP requests only. Business logic should be moved to services.

Potential Impact
Harder testing

Lower maintainability

Higher coupling

Difficult future scaling

2. Static In-Memory Storage
Problem
private static readonly List<WeatherModel> WeatherRecords = new();
Data is stored in memory instead of persistent storage.

Why It Is Technical Debt
This approach is not production-ready.

Potential Impact
Data loss after restart

No scalability

Thread-safety issues

Concurrency problems

3. Weak Validation
Problem
The model accepts invalid values because validation is minimal.

Example:

empty summary

unrealistic temperatures

Why It Is Technical Debt
Invalid data can enter the system.

Potential Impact
Bugs

Incorrect reports

Unstable API behavior

4. Inconsistent API Responses
Problem
Different endpoints return different response formats.

Example:

return Ok("Deleted.");
vs

return Ok(new { result = existing });
Why It Is Technical Debt
API contracts become inconsistent and harder to maintain.

Potential Impact
Harder frontend integration

Poor API usability

Increased maintenance cost

5. Manual ID Generation
Problem
weatherData.Id = WeatherRecords.Max(e => e.Id) + 1;
Why It Is Technical Debt
Unsafe in concurrent environments.

Potential Impact
Duplicate IDs

Race conditions

Data inconsistency

6. Missing Service Layer
Problem
The controller directly manages business operations and data manipulation.

Why It Is Technical Debt
Violates separation of concerns.

Potential Impact
Tight coupling

Harder unit testing

Difficult future refactoring

7. Missing Global Error Handling
Problem
There is no centralized exception handling or logging.

Why It Is Technical Debt
Unhandled exceptions can expose internal implementation details.

Potential Impact
Difficult debugging

Security risks

Poor observability

Part 2 — Decision Making
Decision
I decided to combine:

feature delivery

minimal high-impact refactoring

The new endpoint /weather/summary/report was implemented while also addressing the most critical technical debt.

What I Prioritized
1. Business Requirement
The new reporting endpoint had the highest priority because it was the main business request.

2. High-Impact Technical Debt
I also fixed:

validation

inconsistent responses

business logic extraction into a service

These improvements provide good long-term value while remaining safe within the deadline.

What I Intentionally Left Unfixed
I intentionally did not:

introduce a database

implement repository pattern

redesign the architecture

add authentication

implement advanced logging

These tasks are important but too large for a 2-day deadline.

Risks Accepted
By limiting the refactoring scope, I accept:

in-memory storage limitations

data loss after restart

limited scalability

concurrency limitations

However, the API remains functional and maintainable for the current assignment scope.

Part 3 — Prioritization Backlog
Item	Type	Priority	Reason
Implement /weather/summary/report	Feature	High	Business requirement
Add model validation	Technical Debt	High	Prevent invalid data
Standardize API responses	Technical Debt	High	Better API consistency
Extract report logic into service	Technical Debt	Medium	Improve maintainability
Add global exception handling	Technical Debt	Medium	Reliability improvement
Replace anonymous DTOs	Technical Debt	Medium	Stronger API contracts
Replace in-memory storage with database	Technical Debt	Low	Large refactor
Implement repository pattern	Technical Debt	Low	Architectural improvement
Add authentication	Feature/Security	Low	Not required currently
Part 4 — Stakeholder Communication
We completed the requested weather summary reporting feature within the deadline.

To balance delivery speed and code quality, we also implemented a few focused improvements that reduce future maintenance risks, including better validation and cleaner API structure.

Some larger infrastructure improvements were intentionally postponed because they would significantly increase development time and create a risk of missing the deadline.

The current implementation fully supports the requested functionality and is appropriate for the current project scope, while additional scalability improvements can be planned in future iterations.

Part 5 — Changing Constraints (1 Day Deadline)
Does The Decision Change?
Yes.

With only 1 day available, the priority becomes delivering the feature as quickly as possible.

What Changes?
I would:

implement only /weather/summary/report

add minimal validation

avoid most refactoring

What Gets Deprioritized?
I would postpone:

service extraction

response standardization

architectural improvements

exception handling improvements

Trade-Off
More


обнови мд файл и добавь ответы на все вопросы

# Weather API - Technical Debt Assignment

## Business Requirement

The business requested a new endpoint:

```http
GET /weather/summary/report
```

Deadline: 2 days.

The goal was to deliver the feature while making reasonable engineering decisions regarding existing technical debt.

---

# Part 1 — Identify Technical Debt

## 1. Business Logic Inside Controller

### Problem

The controller contains business logic for:
- generating reports
- calculating averages
- grouping weather summaries
- generating IDs

Example:

```csharp
var avgTemp = WeatherRecords.Count > 0
    ? WeatherRecords.Average(x => x.TemperatureC)
    : 0;
```

### Why It Is Technical Debt

Controllers should only handle HTTP concerns.
Business logic should be moved into services.

### Potential Impact

- Harder testing
- Reduced maintainability
- Higher coupling
- Difficult future scaling

### Decision

A lightweight `WeatherReportService` was introduced.

---

## 2. Static In-Memory Storage

### Problem

```csharp
private static readonly List<WeatherModel> WeatherRecords = new();
```

Data is stored only in memory.

### Why It Is Technical Debt

This solution is not production-ready and does not persist data.

### Potential Impact

- Data loss after restart
- No scalability
- Thread-safety issues
- Concurrency problems

### Decision

This issue was intentionally left unfixed because replacing storage with a database would exceed the project deadline.

---

## 3. Weak Validation

### Problem

The API accepted invalid values.

Examples:
- Missing summary
- Unrealistic temperatures

### Why It Is Technical Debt

Invalid data can enter the system and break reporting logic.

### Potential Impact

- Bugs
- Incorrect statistics
- Unstable API behavior

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

## 4. Inconsistent API Responses

### Problem

Different endpoints returned different response formats.

Example:

```csharp
return Ok("Deleted.");
```

vs

```csharp
return Ok(new { result = existing });
```

### Why It Is Technical Debt

Inconsistent APIs are harder to maintain and consume.

### Potential Impact

- Harder frontend integration
- Poor API usability
- Increased maintenance cost

### Decision

Response formatting was standardized.

---

## 5. Manual ID Generation

### Problem

```csharp
weatherData.Id = WeatherRecords.Max(e => e.Id) + 1;
```

### Why It Is Technical Debt

This implementation is unsafe in concurrent environments.

### Potential Impact

- Duplicate IDs
- Race conditions
- Data inconsistency

### Decision

Left unchanged due to limited assignment scope and in-memory storage limitations.

---

## 6. Missing Service Layer

### Problem

The controller directly manipulates data and business operations.

### Why It Is Technical Debt

Violates separation of concerns.

### Potential Impact

- Tight coupling
- Harder unit testing
- Difficult future refactoring

### Decision

A small report service was introduced to partially separate business logic.

---

## 7. Missing Global Error Handling

### Problem

The application has no centralized exception handling or logging.

### Why It Is Technical Debt

Unhandled exceptions may expose internal implementation details.

### Potential Impact

- Difficult debugging
- Security risks
- Poor observability

### Decision

Postponed due to deadline constraints.

---

# Part 2 — Decision Making

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

---

## What I Intentionally Left Unfixed

I intentionally did not:
- introduce a database
- implement repository pattern
- redesign the architecture
- add authentication
- implement advanced logging

These improvements are valuable but too large for a 2-day deadline.

---

## Risks Accepted

By limiting the refactoring scope, I accept:
- in-memory storage limitations
- data loss after restart
- limited scalability
- concurrency limitations

However, the API remains functional and maintainable for the current assignment scope.

---

# Part 3 — Prioritization Backlog

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

# Part 4 — Stakeholder Communication

We completed the requested weather summary reporting feature within the deadline.

To balance delivery speed and long-term maintainability, we also implemented several focused quality improvements, including validation and cleaner API structure.

Some larger infrastructure improvements were intentionally postponed because they would significantly increase development time and create a risk of missing the deadline.

The current implementation fully supports the requested functionality and is appropriate for the current project scope, while additional scalability improvements can be planned in future iterations.

---

# Part 5 — Changing Constraints (1 Day Deadline)

## Does The Decision Change?

Yes.

With only 1 day available, the priority becomes delivering the feature as quickly as possible.

---

## What Changes?

I would:
- implement only `/weather/summary/report`
- add minimal validation
- avoid most refactoring

---

## What Gets Deprioritized?

I would postpone:
- service extraction
- response standardization
- architectural improvements
- exception handling improvements

---

## Trade-Off

More technical debt would be accepted in order to meet the delivery deadline.

---

# Part 6 — Reflection

## What Is Technical Debt?

Technical debt is the cost of choosing a faster or simpler implementation today that may require additional work in the future.

---

## Is Technical Debt Always Bad?

No.

Technical debt can be a conscious engineering trade-off that helps deliver business value faster.

The problem appears when technical debt becomes unmanaged or ignored for too long.

---

## When Is It Acceptable To Take Technical Debt?

Technical debt is acceptable when:
- deadlines are strict
- requirements are uncertain
- MVP development is required
- rapid experimentation is needed

The important part is understanding and documenting the trade-offs.

---

## How Can Teams Prevent Accumulating More Technical Debt?

Teams can reduce technical debt by:
- performing code reviews
- writing automated tests
- following coding standards
- refactoring continuously
- using proper architecture
- allocating time for maintenance work
- monitoring code quality regularly

---

# Implemented Improvements

The following improvements were implemented:

- Added `/weather/summary/report`
- Added validation attributes
- Standardized API responses
- Introduced `WeatherReportService`
- Added DTO for report responses

---

# Example Validation Improvements

```csharp
[Required]
[StringLength(50)]
public string Summary { get; set; } = string.Empty;

[Range(-100,100)]
public int TemperatureC { get; set; }
```

---

# Example DTO

```csharp
public class WeatherSummaryReportDto
{
    public int TotalRecords { get; set; }

    public double AverageTemperature { get; set; }

    public WeatherModel? Hottest { get; set; }

    public WeatherModel? Coldest { get; set; }

    public string? MostFrequentCondition { get; set; }
}
```

---

# Final Result

The requested feature was delivered on time while also reducing several important technical debt issues.

The solution remains intentionally lightweight and appropriate for the assignment scope and deadline constraints.

