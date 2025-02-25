# CleanArchitecture With DotNet

A sample microservice demonstrating a clean, modern architecture built with ASP.NET Core, Entity Framework Core, and GraphQL.

## Key Design Decisions

- **Clean Architecture & Layered Separation:**
  - The project is organized into clear layers:
    - **Domain:** Contains entities, value objects, and business rules.
    - **Application:** Houses use cases, CQRS command/handler implementations, and MediatR integrations.
    - **Infrastructure:** Implements persistence, external integrations, and data access via Entity Framework Core.

- **CQRS & Mediator Pattern:**
  - Read and write operations are separated using the CQRS pattern, and MediatR is used to decouple request dispatching from handling. This improves maintainability and testability of business logic.

- **Domain-Driven Design (DDD) & Value Objects:**
  - The domain layer leverages DDD principles by encapsulating business invariants within entities and value objects (such as Name and Email). This ensures consistency and clarity of domain logic.

- **Functional Programming Principles:**
  - Using the [CSharpFunctionalExtensions](https://github.com/vkhorikov/CSharpFunctionalExtensions) library, operations return explicit `Result<T>` types to handle success/failure scenarios in a predictable, side-effect-free manner.

- **GraphQL Integration:**
  - HotChocolate is integrated to provide a robust GraphQL API layer, supporting efficient queries and mutations with features like filtering, sorting, and projections.
  - More details can be found in the [HotChocolate Documentation](https://chillicream.com/docs/hotchocolate).

These design choices not only ensure the application is robust and scalable but also foster a codebase that's easy to maintain and extend. We invite you to explore the code to see how these principles are applied throughout the solution.

For more details, see
   - [CQRS and DDD patterns in a microservice](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/apply-simplified-microservice-cqrs-ddd-patterns).
   - [Design a DDD-oriented microservice](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice).
   - [Implement value objects](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/implement-value-objects).



## Technologies

- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [MediatR](https://github.com/jbogard/MediatR)
- [CQRS](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [HotChocolate](https://chillicream.com/docs/hotchocolate)
- [PostgreSQL] (https://www.postgresql.org/)

## Database Migration

Run the following commands to manage database migrations:

```bash
cd src
```

```bash
dotnet ef migrations add [MigrationName]  --verbose --project .\Infrastructure\   --startup-project .\Api\
```

```bash
dotnet ef database update --verbose --project .\Infrastructure\   --startup-project .\Api\
```



