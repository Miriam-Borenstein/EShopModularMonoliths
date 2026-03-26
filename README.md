# EShop – Modular Monolith & Microservices

This repository features an EShop system implementation showcasing modern architectural patterns and advanced .NET 8 techniques, demonstrating a migration path from a **Modular Monolith** to **Microservices** using the **Strangler Fig Pattern**.

## 🏗️ Architectural Patterns
* **Modular Monoliths (Modulith)** Architecture
* **Vertical Slice Architecture (VSA)**
* **Domain-Driven Design (DDD)**
* **Command Query Responsibility Segregation (CQRS)**
* **Outbox Pattern** for Reliable Messaging

---

## 🛠️ Modules Overview

### 1. Catalog Module
* **Framework:** ASP.NET Core Minimal APIs, .NET 8, and C# 12 features.
* **Architecture:** Vertical Slice Architecture implementation with Feature folders and single .cs file (includes different classes in one file).
* **CQRS:** Implementation using **MediatR** library.
* **Validation:** Pipeline Behaviors with MediatR and **FluentValidation**.
* **Data:** Entity Framework Core Code-First approach and Migrations on **PostgreSQL**.
* **Routing:** Used **Carter** for Minimal API endpoint definition.
* **Cross-Cutting Concerns:** Logging, Global Exception Handling, and Health Checks.

### 2. Basket Module
* **Caching:** Using **Redis** as a Distributed Cache over PostgreSQL database.
* **Patterns:** Implements **Proxy, Decorator, and Cache-aside** patterns.
* **Messaging:** Publish `BasketCheckoutEvent` to **RabbitMQ** via **MassTransit** library.
* **Reliability:** Implement **Outbox Pattern** for Reliable Messaging w/ BasketCheckout Use Case.

### 3. Identity Module
* **Protocols:** OAuth2 + OpenID Connect Flows with **Keycloak**.
* **Infrastructure:** Setup Keycloak into Docker-compose file as a Backing Service.
* **Security:** JwtBearer token for OpenID Connect with Keycloak Identity.

### 4. Ordering Module
* **Implementation:** Implementing **DDD, CQRS, and Clean Architecture** using Best Practices.
* **Reliability:** Implement **Outbox Pattern** for Reliable Messaging w/ BasketCheckout Use Case.

---

## 📡 Module Communications
* **Sync Communications:** Between Catalog and Basket Modules with In-process Method Calls (Public APIs).
* **Async Communications:** Between Modules w/ **RabbitMQ & MassTransit** for `UpdatePrice` between Catalog-Basket Modules.

---

## 🚀 Microservices Migration
* Migration of EShop Modules to Microservices w/ **Stranger Fig Pattern**.

---

## 🐳 Infrastructure
* Orchestrated via **Docker-compose**, including PostgreSQL, Redis, Seq, RabbitMQ, and Keycloak.
