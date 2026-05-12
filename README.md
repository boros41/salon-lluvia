**This ASP.NET Core MVC project is no longer maintained as it has been migrated to an [Angular](https://github.com/boros41/ng-salonlluvia) and [ASP.NET Web API](https://github.com/boros41/salonlluvia) project.**

# Salon Lluvia Web Application
Production web application for a local salon business built to support online appointment scheduling, gallery browsing, and delivery of general business information.

## Azure Deployment
Developed and deployed as a full-stack web application on **Microsoft Azure App Service** using:
-  **ASP .NET Core MVC**
-  **EF Core**
-  **Azure SQL Database**
-  **Azure Blob Storage**
-  **Bootstrap**
-  **jQuery**

This deployment supports a public-facing business website for a local salon's operation.

## Appointment Scheduling Integration
Integrated the [Calendly API](https://developer.calendly.com/api-docs/4b402d5ab3edd-calendly-developer) through an **ASP .NET Core MVC** backend gateway and an **AJAX** workflow to:
- Display only valid appointment dates
- Populate a [Flatpickr.js](https://flatpickr.js.org/) date picker with available days
- Synchronize appointments with the business calendar
- Automate scheduling and reduce booking friction

This helps the business provide a smoother booking experience and reach more clients.

## Authentication and Authorization
Implemented **ASP .NET Identity** authentication and **role-based authorization** to secure administrative functionality, including:

- Restricting image uploads to administrators
- Restricting content management to administrators

This ensures sensitive site management features are protected from public access.

## Cloud Image Storage
Integrated **Azure Blob Storage** with **Microsoft Entra ID** authentication to securely store and retrieve uploaded gallery images.

This enables:
- Secure cloud-based image storage
- Scalable media delivery
- Public gallery access for site visitors

## Gallery Page
Uses the [Masonry](https://masonry.desandro.com/) JavaScript library to provide a responsive image viewing experience with the ability to filter images by gender, hairstyle, and hair color.

![enter image description here](https://i.imgur.com/yNuj6a5.png)

## Project Highlights
- Full-stack web application deployed to **Microsoft Azure App Service**
- Real-world business website for a local salon
-  **Calendly API** integration for appointment automation
-  **Azure Blob Storage** with **Microsoft Entra ID** for secure image management
