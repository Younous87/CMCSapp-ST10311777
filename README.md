# Contract Monthly Claim System (CMCS)

## Overview
For the creation of the Contract Monthly Claim System (CMCS), three important points needed to be addressed:
- The application's overall design.
- The structure of the database.
- The layout of the GUI.

## Design Choices
For the application's overall design, I decided to use the Model-View-Controller (MVC) architecture. MVC is divided into three main components:

- **Model:** Manages interactions with the database.
- **View:** Defines the UI and displays necessary information.
- **Controller:** Processes user inputs and manages interactions between the model and view.

MVC is a good choice for CMCS because it separates concerns by dividing the application into Model, View, and Controller components. This enhances maintainability, allows for modular development, and improves scalability. It also makes it easier to manage user interfaces and business logic independently, providing a clear structure for adding features like claim submission and approval workflows.

## Database Structure 
For the database structure, a relational database was selected for the CMCS. It provides structured data organization, ensuring relationships between entities like lecturers, managers, coordinators, and claims are efficiently managed. 

### Key Benefits:
- Supports data integrity, consistency, and scalability, making it easy to handle multiple users, track claims, and store associated data (like documents) securely.
- Use of foreign keys and constraints ensures accurate record linkage and avoids data redundancy, which is essential for the claim submission and approval process.

## User Interface Design
The UI for the CMCS website follows a clean, minimalistic design with a consistent color scheme of red, black, and pink. The design focuses on usability and a simple layout across all pages.

### Key Pages:
- **Home Page:** Users are welcomed with role selection buttons ("Lecturer" or "Manager/Coordinator"), using bold black buttons on a soft pink background.
- **Create Claim Page:** A form allows users to enter work details and upload supporting documents, along with a summary table of existing claims.
- **Approve Claim Page:** Users can review claim details and either approve or reject claims using a straightforward form with radio buttons.

Across all pages, the red header with navigation links ensures easy access to different sections, while the color contrast ensures clarity and ease of interaction. This UI design maintains a professional and user-friendly appearance for a smooth contract claim process.

## Assumptions and Constraints

### Assumptions:
- **User Registration:** Lecturers, coordinators, and managers are pre-registered with valid credentials.
- **Hourly Rate:** Lecturers will manually enter their hourly rate or the system will define it during claim submission.
- **Role Permissions:** Only authorized users (coordinators and managers) have access to verification and approval functions.

### Constraints:
- **Performance Constraints:** The system must efficiently handle a growing number of users and claims without slowdowns. Scalability is key as more lecturers submit claims.
- **Security Constraints:** Sensitive data (e.g., personal and financial information) must be protected. Only specific users should have access to specific functionality, such as managers and coordinators approving claims.
