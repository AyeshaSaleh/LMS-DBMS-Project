# 📚 LMS — Library Management System

**LMS ** is a robust desktop application developed as part of a Database Management Systems course. It features a modern, dark-themed UI and a structured MySQL backend to handle the complex workflows of a modern library.

---

## 🛠️ The Tech Stack
* **Frontend:** C# with **WPF (Windows Presentation Foundation)** for a sleek, responsive dark-themed desktop UI.
* **Backend:** C# Logic for business rules and data validation.
* **Database:** **MySQL** for relational data storage and integrity.
* **Project Style:** Collaborative development (Team: Ayesha Saleh & Wania Adnan).

---

## 🚀 Key Features & Functionality
* **User Roles:** Dedicated interfaces for Librarians (Admin) and Members.
* **Book Management:** Full CRUD operations for adding, updating, and removing books from the catalog.
* **Transaction Logic:** Automated systems for **Issuing** and **Returning** books with real-time availability updates.
* **Fine Tracking:** Built-in logic to calculate and track overdue fines based on return dates.
* **Search Engine:** Quick filtering by Title, Author, or ISBN.

---

## 🧠 Database Architecture
The core of this project is a relational database designed to minimize redundancy and ensure data consistency. Key entities include:
* **Books:** Title, Author, ISBN, Quantity, Genre.
* **Users:** Member details and authentication roles.
* **Transactions:** Linking Users to Books with Issue/Return timestamps.
* **Fines:** Table tracking financial penalties for overdue items.

---

## ⚙️ Setup & Installation
1.  **Database:** Import the provided `.sql` file into your MySQL Workbench to set up the schema.
2.  **Configuration:** Update the Connection String in the `App.config` or `Database.cs` file with your MySQL credentials.
3.  **Build:** Open the solution in **Visual Studio** and run the project.

---
*Developed with a focus on clean UI design and relational database integrity.*
