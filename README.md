C# Web API Template with CQRS, MediatR, and Layered Architecture
This project is a C# Web API template built using the CQRS (Command Query Responsibility Segregation) pattern, MediatR for handling requests, and a layered architecture for separation of concerns. It includes user management, authentication, and email functionality using SMTP. The template is designed to help you quickly start building scalable and maintainable web applications.

Table of Contents
Project Overview

Architecture

Features

Technologies Used

Getting Started

Prerequisites

Installation

Configuration

Project Structure

Running the Application

Testing

Contributing

License

Project Overview
This template provides a foundation for building C# Web APIs using the CQRS pattern and MediatR for handling commands and queries. It includes:

User management and authentication (already implemented).

Email functionality using SMTP.

Separate databases for identity management and business logic.

A unit test project for testing the application.

The template is structured into three main layers:

Core: Contains the Application and Domain layers.

Infrastructure: Handles Persistence, Identity, and other infrastructure concerns.

API: The entry point of the application.

Architecture
The project follows a layered architecture with CQRS and MediatR:

Core Layer:

Application Layer: Contains commands, queries, handlers, and DTOs.

Domain Layer: Contains entities, enums, and domain-specific logic.

Infrastructure Layer:

Persistence: Handles database interactions for business logic.

Identity: Manages user authentication and authorization (separate database).

Infrastructure: Includes shared services like email sending.

API Layer:

The entry point of the application, exposing endpoints for the frontend.

Unit Test Project:

Contains unit tests for the application.

Features
User Management:

User registration, login, and authentication using JWT tokens.

Role-based authorization.

Email Functionality:

Send emails using SMTP (e.g., for account confirmation or password reset).

CQRS Pattern:

Separates commands (write operations) and queries (read operations) for better scalability.

MediatR:

Handles commands and queries, promoting clean and maintainable code.

Separate Databases:

Identity Database: Stores user-related data (e.g., credentials, roles).

Business Database: Stores application-specific data.

Unit Testing:

Includes a unit test project to ensure code quality.

Technologies Used
C# and .NET Core

MediatR for CQRS implementation

Entity Framework Core for database interactions

JWT for authentication

SMTP for email functionality

xUnit or NUnit for unit testing

Swagger for API documentation

Getting Started
Prerequisites
.NET SDK (version 6.0 or higher)

Visual Studio or Visual Studio Code

SQL Server (or another database provider supported by EF Core)

SMTP credentials (e.g., Gmail, SendGrid)

Installation
Clone the repository

Restore the NuGet packages

Database Configuration:

Update the connection strings in appsettings.json for both the Identity and Business databases.

Run migrations for both databases

SMTP Configuration:

Update the SMTP settings in appsettings.json with your email provider's credentials.

JWT Configuration:

Update the JWT settings in appsettings.json with your secret key and issuer details.

Project Structure
Copy
/CSharpWebApiTemplate
│
├── /Core
│   ├── /Application      # Commands, Queries, Handlers, DTOs
│   └── /Domain           # Entities, Enums, Domain Logic
│
├── /Infrastructure
│   ├── /Identity         # User Authentication and Authorization
│   ├── /Persistence      # Database Contexts and Repositories
│   └── /Infrastructure   # Shared Services (e.g., Email)
│
├── /API                  # Web API Entry Point
│
├── /UnitTests            # Unit Test Project
│
└── README.md             # Project Documentation

Use the provided endpoints to register, log in, and test the application.

Contributing
Contributions are welcome! Please follow these steps:

Fork the repository.

Create a new branch for your feature or bugfix.

Submit a pull request with a detailed description of your changes.

License
This project is licensed under the MIT License. See the LICENSE file for details.
