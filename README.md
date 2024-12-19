# Employee Management assesstment

- Created by Luiz Augusto Ferrete (l.a.ferrete@gmail.com)

- LinkedIn: https://www.linkedin.com/in/luiz-augusto-ferrete-990713b8/

## Running instructions:
You can run the project by doing a simple restore and run.

After you clone the project, make sure you are into the *cloned folder*, and run the following:

```bash
cd EmployeeMaintenance ## To go to the solutions folder
dotnet restore
cd EmployeeMaintenance ## To go to the API project folder
dotnet run
```



## Project Architecture Overview

This solution implements a **Clean Architecture** approach with a focus on separation of concerns. Each layer is independent and easy to modify, with a structure that supports scalability for future growth and changes.

### Layers

#### 1. **EmployeeMaintenance.API**
   - Entry point of the application.
   - Handles HTTP requests and configures **Dependency Injection**.

#### 2. **EmployeeMaintenance.BLL (Business Logic Layer)**
   - Contains business logic and services.
   - Depends only on interfaces and domain models from the **DL**.

#### 3. **EmployeeMaintenance.DAL (Data Access Layer)**
   - Implements repository interfaces defined in the **DL**.
   - Responsible for data access operations.

#### 4. **EmployeeMaintenance.DL (Domain Layer)**
   - Core layer of the application.
   - Defines domain entities, value objects, and interfaces.

#### 5. **EmployeeMaintenance.Tests**
   - Contains unit tests to validate the application’s functionality.

