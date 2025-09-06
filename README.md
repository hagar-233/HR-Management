# HR-Management
HR-Management
📌 HR Management System
📖 Overview
The HR Management System is a console-based C# application that allows managing employees' information, calculating payroll, and handling basic HR operations.
It supports different roles (Admin, Manager, Developer, Intern) with access controlled via simple password authentication.
⚙️ Features
🔐 Role-based login system 
Admin, Manager, Developer, Intern (each with a predefined password).
👨‍💼 Add Employee 
Register new employees with ID, Name, Role, and Salary.
💰 Update Employee Salary 
Modify employee salary by ID.
🔎 Display Employee (by ID) 
Search and show employee details.
📊 Calculate Payroll 
Compute payroll based on working hours and salary.
📝 Log System 
Keeps a record of operations in a log file.
📂 File Structure
EMPdata.txt → Stores employees’ details.
SystemLog.txt → Stores log messages with timestamps.
Program.cs → Main program logic.
🔑 Default Passwords
Admin: 123
Manager: 456
Developer/Intern: 789
🚀 How to Run
Clone or download the repository.
Open the project in Visual Studio or any C# IDE.
Build and run the program.
Choose your role and enter the correct password.
⚠️ Known Issues
⚡️ Duplicate error messages: Sometimes, "Employee not found!" appears multiple times.
🛠 Error handling: Needs improvement (e.g., invalid input handling).
📑 Code duplication: Some parts can be refactored for cleaner structure.
🔄 No database: Data is stored in text files only (EMPdata.txt).
🌟 Future Improvements
Replace text files with a database (SQL/SQLite).
Add a better UI (maybe Windows Forms or WPF).
Enhance input validation to avoid crashes.
Improve logging system with more detailed actions.
👩‍💻 Author
Developed with effort and practice by Hagar Ahmed.
