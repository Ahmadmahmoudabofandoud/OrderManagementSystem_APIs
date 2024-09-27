# Order Management System - Web API

## About

 Order Management System is a Web API project designed to streamline the order management process. Built using the Onion Architecture Pattern, this system ensures a clear separation of concerns, enhancing scalability and maintainability. It incorporates various design patterns and best practices, including Dependency Injection, Unit Testing, and Entity Framework Core for efficient data persistence.

## Overview

This project consists of multiple layers, each with a specific responsibility, which allows for a clean architecture and easy maintenance:

### Architecture Overview

1. **Core Layer (Domain Layer)**
   - A class library that contains:
     - **Entities**: Core business entities.
     - **Interfaces**: Defines contracts for repositories, services, and specifications without implementation details.

2. **Application Layer**
   - Another class library that handles application logic and business rules, implementing interfaces from the Core layer.

3. **Infrastructure Layer**
   - A class library responsible for:
     - Managing the `DbContext` and migrations using Entity Framework Core.
     - Implementing Unit of Work, Specification, and Generic Repository patterns for database operations.

4. **Web API Layer**
   - The outermost layer containing controllers that expose the API endpoints for client consumption.

5. **Unit Testing**
   - Unit testing is implemented to ensure the functionality of individual components. 
   - Tests focus on services, repositories, and business logic to verify correctness and reliability, utilizing Moq and xUnit for mocking dependencies and promoting Test-Driven Development (TDD) practices.

### Design Patterns Used

- **Unit of Work Design Pattern (UoW)**: 
  - Manages transactions and ensures consistency across multiple operations.

- **Specification Design Pattern**: 
  - Encapsulates query logic in reusable specifications, allowing for flexible and maintainable queries.

- **Generic Repository Design Pattern**: 
  - Abstracts data access logic, providing a clean and consistent way to interact with the database.

## Key Features

- **Onion Architecture**: Structured separation of concerns for improved scalability and maintainability.
- **Entity Framework Core**: Code First with LINQ for seamless data persistence.
- **Unit Testing**: Comprehensive tests for services and repositories using Moq and xUnit for reliable and maintainable code.
- **Design Patterns**: Incorporates Unit of Work, Specification, and Generic Repository patterns for better structure and clarity.
- **Dependency Injection**: Ensures flexibility and decoupling of components, enhancing testability.

## Getting Started

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
