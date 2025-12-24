# FPT Tracking System

FPT Tracking System is a fullstack web application built as a graduation project to support internal tracking and management workflows, including user authentication, business logic services, and communication mechanisms.

## 🚀 Features

- **Secure Authentication & Authorization**  
  Implements user login with **JWT (JSON Web Tokens)** and cookie-based session handling for secure access control.

- **Layered Backend Architecture**  
  Backend services are designed following a clear separation of concerns using **Controller – Service – Repository** patterns to ensure maintainability and scalability.

- **Message Queue Integration**  
  Asynchronous processing of tasks is enabled through a **message queue** system, improving responsiveness and decoupling service components.

- **AI Integration (Gemini)**  
  Integrates with **AI Gemini** to support intelligent features within the system.

- **Email Notifications**  
  Supports sending email notifications for system events and user interactions.

- **RESTful API Support**  
  Provides RESTful endpoints to communicate between frontend and backend services.

## 🧠 Technology Stack

The project is implemented using industry-standard technologies to build a scalable web solution, with frontend, backend, and integration components.

## 📁 Architecture Overview

The application follows a layered architecture:

- **Controller Layer:** Handles incoming HTTP requests and routes them to backend services.
- **Service Layer:** Contains business logic and coordinates operations between controllers and repositories.
- **Repository Layer:** Interacts with the database and data models.

## 🛠 Setup & Deployment

The repository includes scripts and configuration for environment setup and deployment automation (Docker support and deployment scripts).

Refer to the included deployment guides for setting up the application locally or in a development environment.

## 📌 How to Run

1. Clone the repository  
2. Configure environment variables  
3. Run database migration and seed  
4. Build and start backend services  
5. Run frontend application

## 🧩 Contribution

This repository was developed as a single-author graduation project. Contributions are welcome for improvements or feature extensions.

---

## 📜 License

This project is open-source for educational and portfolio purposes.

