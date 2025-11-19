# SpinTrack API

A production-ready Web API built with **.NET 10**, following **Clean Architecture** principles and best practices for enterprise applications.

## 🏗️ Architecture

This project implements **Clean Architecture** with the following layers:

```
SpinTrack/
├── SpinTrack.Api/              # Presentation Layer (Controllers, Middleware, Configuration)
├── SpinTrack.Application/      # Application Layer (Services, DTOs, Validators, Interfaces)
├── SpinTrack.Infrastructure/   # Infrastructure Layer (DbContext, Repositories, External Services)
└── SpinTrack.Core/            # Domain Layer (Entities, Enums, Business Logic)
```

### Dependency Flow
```
SpinTrack.Api → SpinTrack.Application → SpinTrack.Core
                         ↑
            SpinTrack.Infrastructure
```

## ✨ Features

### Architecture & Design Patterns
- ✅ **Clean Architecture** - Clear separation of concerns
- ✅ **Repository Pattern** - Data access abstraction
- ✅ **Unit of Work Pattern** - Transaction management
- ✅ **Result Pattern** - Explicit error handling
- ✅ **Manual Mapping** - No AutoMapper dependency

### Security
- ✅ **JWT Authentication** - Access & Refresh tokens
- ✅ **BCrypt Password Hashing** - Secure password storage
- ✅ **Claims-based Authorization** - User claims in JWT tokens
- ✅ **Automatic Audit Fields** - CreatedBy/ModifiedBy from token

### API Features
- ✅ **API Versioning** - URL segment versioning (v1, v2, etc.)
- ✅ **Swagger/OpenAPI** - Interactive API documentation
- ✅ **Global Exception Handling** - Centralized error handling
- ✅ **Serilog Logging** - Structured logging to console and file
- ✅ **FluentValidation** - Comprehensive input validation
- ✅ **CORS Support** - Cross-origin resource sharing

### Data Features
- ✅ **Advanced Querying** - Pagination, filtering, sorting, searching
- ✅ **Multi-column Sorting** - Sort by multiple fields
- ✅ **Dynamic Filtering** - Filter by any column with **17 operators**
- ✅ **Global Search** - Search across all user fields
- ✅ **CSV Export** - Export filtered/searched data to CSV

### Database
- ✅ **Entity Framework Core 10** - ORM with SQL Server
- ✅ **Fluent API Configuration** - Entity configurations
- ✅ **Code-First Migrations** - Database version control

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022+ or Visual Studio Code
- Postman or VS Code REST Client (optional)

### Installation

1. **Clone the repository**
```bash
git clone <repository-url>
cd SpinTrack
```

2. **Update Connection String**

Edit `SpinTrack.Api/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=SpinTrackDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

3. **Create Database**
```bash
cd SpinTrack.Api
dotnet ef migrations add InitialCreate --project ../SpinTrack.Infrastructure
dotnet ef database update
```

4. **Run the Application**
```bash
dotnet run
```

5. **Access Swagger UI**

Open browser: `https://localhost:7001`

## 📋 API Endpoints

### Authentication (`/api/v1/auth`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/register` | Register new user | No |
| POST | `/login` | User login | No |
| POST | `/refresh-token` | Refresh access token | No |
| POST | `/revoke-token` | Logout (revoke token) | Yes |
| POST | `/change-password` | Change password | Yes |

### User Management (`/api/v1/users`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/query` | Query users with filters | Yes |
| GET | `/{id}` | Get user by ID | Yes |
| POST | `/` | Create new user | Yes |
| PUT | `/{id}` | Update user | Yes |
| DELETE | `/{id}` | Delete user | Yes |
| PATCH | `/{id}/status` | Change user status | Yes |
| POST | `/export` | Export users to CSV | Yes |

## 🔍 Query Features

### Pagination
```json
{
  "pageNumber": 1,
  "pageSize": 10
}
```

### Search
```json
{
  "searchTerm": "john"
}
```
Searches across: Username, Email, FirstName, MiddleName, LastName, PhoneNumber, NationalId, Nationality, JobTitle

### Filtering

Supported operators: **17 total** (4 new operators added!)

**String Operators (8):**
- `Equals`, `NotEquals`, `Contains`, `NotContains` ⭐, `StartsWith`, `EndsWith`, `IsEmpty` ⭐, `IsNotEmpty` ⭐

**Numeric/Date Operators (5):**
- `GreaterThan`, `GreaterThanOrEqual`, `LessThan`, `LessThanOrEqual`, `Between` ⭐

**Collection Operators (2):**
- `In`, `NotIn`

**Null Operators (2):**
- `IsNull`, `IsNotNull`

⭐ = New operators

#### Example: Contains
```json
{
  "filters": [
    {
      "columnName": "Email",
      "operator": "Contains",
      "value": "@gmail.com"
    }
  ]
}
```

#### Example: NotContains (NEW)
```json
{
  "filters": [
    {
      "columnName": "Email",
      "operator": "NotContains",
      "value": "test"
    }
  ]
}
```

#### Example: IsEmpty (NEW)
```json
{
  "filters": [
    {
      "columnName": "MiddleName",
      "operator": "IsEmpty"
    }
  ]
}
```

#### Example: Between (NEW)
```json
{
  "filters": [
    {
      "columnName": "DateOfBirth",
      "operator": "Between",
      "value": "1990-01-01",
      "valueTo": "1999-12-31"
    }
  ]
}
```

### Multi-column Sorting
```json
{
  "sortColumns": [
    {
      "columnName": "FirstName",
      "direction": "Ascending"
    },
    {
      "columnName": "CreatedAt",
      "direction": "Descending"
    }
  ]
}
```

### Export
```json
{
  "exportFormat": "Csv",
  "exportAll": true
}
```

## 🔐 Authentication Flow

1. **Register/Login** → Receive `accessToken` and `refreshToken`
2. **Make API calls** with `Authorization: Bearer {accessToken}`
3. **When token expires** → Use `/refresh-token` with `refreshToken`
4. **Logout** → Use `/revoke-token` to invalidate refresh token

### JWT Claims

The access token includes:
- `UserId` (NameIdentifier)
- `Username` (Name)
- `Email`
- `FirstName` (GivenName)
- `MiddleName`
- `LastName` (Surname)
- `PhoneNumber` (MobilePhone)

## 📦 Database Schema

### User Table (auth.User)
```sql
UserId (GUID, PK)
Username (NVARCHAR(50), Unique)
Email (NVARCHAR(256), Unique)
PasswordHash (NVARCHAR(MAX))
FirstName, MiddleName, LastName
PhoneNumber, NationalId
Gender (VARCHAR(10))
DateOfBirth (DATE)
Nationality, JobTitle
ProfilePicturePath
Status (NVARCHAR(20))
CreatedBy, CreatedAt, ModifiedBy, ModifiedAt
```

### RefreshToken Table (auth.RefreshToken)
```sql
RefreshTokenId (GUID, PK)
UserId (GUID, FK)
Token (NVARCHAR(500), Unique)
ExpiresAt, CreatedAt, RevokedAt
```

## 🧪 Testing

### Using Swagger UI
1. Navigate to `https://localhost:7001`
2. Click **Authorize** button
3. Enter: `Bearer {your-token}`
4. Test endpoints interactively

### Using HTTP File
Open `SpinTrack.Api/SpinTrack.Api.http` in VS Code or Visual Studio 2022+

### Example: Complete Flow
```http
### 1. Register
POST https://localhost:7001/api/v1/auth/register
Content-Type: application/json

{
  "username": "testuser",
  "email": "test@example.com",
  "password": "Test123!@#",
  "confirmPassword": "Test123!@#",
  "firstName": "Test",
  "lastName": "User",
  "gender": "Male",
  "dateOfBirth": "1990-01-01",
  "nationality": "USA"
}

### 2. Login
POST https://localhost:7001/api/v1/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "Test123!@#"
}

### 3. Query Users (use token from login)
POST https://localhost:7001/api/v1/users/query
Authorization: Bearer {your-access-token}
Content-Type: application/json

{
  "pageNumber": 1,
  "pageSize": 10
}
```

## ⚙️ Configuration

### JWT Settings (appsettings.json)
```json
"JwtSettings": {
  "Secret": "Your-Secret-Key-At-Least-32-Characters",
  "Issuer": "SpinTrack",
  "Audience": "SpinTrackUsers",
  "AccessTokenExpirationMinutes": 30,
  "RefreshTokenExpirationDays": 7
}
```

⚠️ **Security Note**: For production, use:
- User Secrets (Development)
- Environment Variables (Production)
- Azure Key Vault / AWS Secrets Manager

### Logging (Serilog)
Logs are written to:
- Console (formatted output)
- File: `logs/spintrack-{Date}.txt`

## 🏗️ Project Structure Details

### SpinTrack.Core (Domain Layer)
- `Entities/` - Domain entities (User, RefreshToken)
- `Enums/` - Domain enumerations (UserStatus, Gender)

### SpinTrack.Application (Application Layer)
- `Common/` - Shared models, interfaces, validators
  - `Results/` - Result pattern classes
  - `Models/` - QueryRequest, PagedResult, etc.
  - `Interfaces/` - IRepository, IUnitOfWork, etc.
  - `Validators/` - FluentValidation validators
- `Features/` - Feature-based organization
  - `Auth/` - Authentication DTOs, services, validators
  - `Users/` - User management DTOs, services

### SpinTrack.Infrastructure (Infrastructure Layer)
- `Persistence/` - EF Core configurations
- `Repositories/` - Repository implementations
- `Services/` - Service implementations
- `Authentication/` - JWT token service
- `QueryBuilders/` - Dynamic LINQ expression builders

### SpinTrack.Api (Presentation Layer)
- `Controllers/V1/` - Versioned API controllers
- `Middleware/` - Global exception handling
- `Configuration/` - Swagger, API versioning setup

## 📝 Best Practices Implemented

1. **Clean Architecture** - Independent, testable layers
2. **SOLID Principles** - Applied throughout
3. **Dependency Injection** - Constructor injection
4. **Async/Await** - All I/O operations
5. **Cancellation Tokens** - Support for request cancellation
6. **Result Pattern** - Explicit success/failure handling
7. **FluentValidation** - Comprehensive validation
8. **Manual Mapping** - Explicit, maintainable mappings
9. **Audit Fields** - Automatic CreatedBy/ModifiedBy
10. **Structured Logging** - Serilog with context
11. **API Versioning** - Future-proof API design
12. **Swagger Documentation** - Interactive API docs

## 🔄 Entity Framework Migrations

### Create Migration
```bash
dotnet ef migrations add MigrationName --project SpinTrack.Infrastructure --startup-project SpinTrack.Api
```

### Apply Migration
```bash
dotnet ef database update --project SpinTrack.Infrastructure --startup-project SpinTrack.Api
```

### Remove Last Migration
```bash
dotnet ef migrations remove --project SpinTrack.Infrastructure --startup-project SpinTrack.Api
```

## 📊 Filter Operators Reference

| Operator | Type | Description | Example |
|----------|------|-------------|---------|
| `Equals` | String | Exact match | `Status = "Active"` |
| `NotEquals` | String | Not equal | `Status != "Inactive"` |
| `Contains` | String | String contains | `Email contains "@gmail"` |
| `NotContains` ⭐ | String | Does not contain | `Email not contains "test"` |
| `StartsWith` | String | String starts with | `Username starts with "john"` |
| `EndsWith` | String | String ends with | `Email ends with ".com"` |
| `IsEmpty` ⭐ | String | Null or empty string | `MiddleName is empty` |
| `IsNotEmpty` ⭐ | String | Not null and not empty | `PhoneNumber is not empty` |
| `GreaterThan` | Numeric/Date | Greater than | `Age > 18` |
| `GreaterThanOrEqual` | Numeric/Date | Greater or equal | `Age >= 18` |
| `LessThan` | Numeric/Date | Less than | `Age < 65` |
| `LessThanOrEqual` | Numeric/Date | Less or equal | `Age <= 65` |
| `Between` ⭐ | Numeric/Date | Between range (inclusive) | `DateOfBirth between 1990-1999` |
| `In` | Collection | Value in list | `Status in ["Active", "Pending"]` |
| `NotIn` | Collection | Value not in list | `Status not in ["Suspended"]` |
| `IsNull` | Null | Value is null | `MiddleName is null` |
| `IsNotNull` | Null | Value is not null | `PhoneNumber is not null` |

⭐ = New operator

**For detailed examples and usage**, see [FILTER_OPERATORS_GUIDE.md](FILTER_OPERATORS_GUIDE.md)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## 📄 License

This project is licensed under the MIT License.

## 🙋 Support

For questions or issues, please open an issue in the repository.

---

**Built with ❤️ using .NET 10 and Clean Architecture**
