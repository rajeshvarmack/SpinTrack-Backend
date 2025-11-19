# SpinTrack API - Setup Guide

This guide will help you set up and run the SpinTrack API from scratch.

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

1. **.NET 10 SDK**
   - Download from: https://dotnet.microsoft.com/download/dotnet/10.0
   - Verify installation: `dotnet --version`

2. **SQL Server**
   - SQL Server 2019+ (Express, Developer, or Full edition)
   - Or SQL Server LocalDB (included with Visual Studio)
   - Verify: SQL Server Management Studio (SSMS) or Azure Data Studio

3. **IDE (Choose one)**
   - Visual Studio 2022 (version 17.8+)
   - Visual Studio Code with C# extension
   - JetBrains Rider 2023.3+

4. **Optional Tools**
   - Postman (for API testing)
   - Git (for version control)

---

## 🚀 Step-by-Step Setup

### Step 1: Restore NuGet Packages

Open terminal in the `SpinTrack` folder and run:

```bash
dotnet restore
```

This will restore all dependencies for all projects.

### Step 2: Update Connection String

1. Open `SpinTrack/SpinTrack.Api/appsettings.json`
2. Update the connection string based on your SQL Server setup:

**For SQL Server Express:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=SpinTrackDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

**For SQL Server LocalDB:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SpinTrackDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

**For SQL Server with Authentication:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=SpinTrackDb;User Id=your_username;Password=your_password;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

### Step 3: Update JWT Secret (Important for Production)

For **Development**, you can use the default secret in `appsettings.json`.

For **Production**, use User Secrets or Environment Variables:

**Using User Secrets (Recommended for Development):**

```bash
cd SpinTrack.Api
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:Secret" "YourProductionSecretKeyThatIsAtLeast32CharactersLong!@#$%^&*()"
```

**Using Environment Variables (Recommended for Production):**

Set environment variable:
- Windows: `setx JwtSettings__Secret "YourSecretKey"`
- Linux/Mac: `export JwtSettings__Secret="YourSecretKey"`

### Step 4: Create Database Migration

Navigate to the API project folder:

```bash
cd SpinTrack.Api
```

Create the initial migration:

```bash
dotnet ef migrations add InitialCreate --project ../SpinTrack.Infrastructure --startup-project .
```

This will create migration files in `SpinTrack.Infrastructure/Migrations/` folder.

### Step 5: Apply Database Migration

Apply the migration to create the database:

```bash
dotnet ef database update --project ../SpinTrack.Infrastructure --startup-project .
```

**Verify Database Creation:**
- Open SQL Server Management Studio (SSMS) or Azure Data Studio
- Connect to your SQL Server instance
- You should see `SpinTrackDb` database with two tables:
  - `auth.User`
  - `auth.RefreshToken`

### Step 6: Build the Solution

```bash
cd ..
dotnet build
```

Ensure there are no build errors.

### Step 7: Run the Application

```bash
cd SpinTrack.Api
dotnet run
```

You should see output like:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5001
```

### Step 8: Access Swagger UI

Open your browser and navigate to:
```
https://localhost:7001
```

You should see the Swagger UI with all API endpoints documented.

---

## ✅ Verify Installation

### Test 1: Register a User

Using Swagger UI:
1. Expand `POST /api/v1/auth/register`
2. Click "Try it out"
3. Use this sample payload:

```json
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
```

4. Click "Execute"
5. You should receive a 200 OK response with `accessToken` and `refreshToken`

### Test 2: Login

1. Expand `POST /api/v1/auth/login`
2. Click "Try it out"
3. Use this payload:

```json
{
  "username": "testuser",
  "password": "Test123!@#"
}
```

4. Click "Execute"
5. Copy the `accessToken` from the response

### Test 3: Authorize and Query Users

1. Click the **"Authorize"** button at the top of Swagger UI
2. Enter: `Bearer {paste-your-access-token}`
3. Click "Authorize"
4. Expand `POST /api/v1/users/query`
5. Click "Try it out"
6. Use this payload:

```json
{
  "pageNumber": 1,
  "pageSize": 10
}
```

7. Click "Execute"
8. You should see the paginated list of users

---

## 🔧 Troubleshooting

### Issue 1: Cannot Connect to Database

**Error:** `A network-related or instance-specific error occurred`

**Solutions:**
1. Verify SQL Server is running:
   - Open "Services" (Win + R → `services.msc`)
   - Find "SQL Server (MSSQLSERVER)" or "SQL Server (SQLEXPRESS)"
   - Ensure it's running

2. Check connection string format
3. Enable TCP/IP in SQL Server Configuration Manager
4. Check firewall settings

### Issue 2: Migration Fails

**Error:** `Unable to create an object of type 'SpinTrackDbContext'`

**Solution:**
Ensure you're in the correct directory and using the right startup project:

```bash
cd SpinTrack.Api
dotnet ef migrations add InitialCreate --project ../SpinTrack.Infrastructure --startup-project .
```

### Issue 3: Port Already in Use

**Error:** `Address already in use`

**Solutions:**
1. Change ports in `launchSettings.json`:
   ```json
   "applicationUrl": "https://localhost:7002;http://localhost:5002"
   ```

2. Or kill the process using the port:
   - Windows: `netstat -ano | findstr :7001`
   - Then: `taskkill /PID <process-id> /F`

### Issue 4: JWT Token Invalid

**Error:** `401 Unauthorized`

**Solutions:**
1. Ensure token hasn't expired (30 minutes by default)
2. Use refresh token to get new access token
3. Check that token is properly formatted: `Bearer {token}`

### Issue 5: CORS Error

**Error:** `CORS policy: No 'Access-Control-Allow-Origin' header`

**Solution:**
Update `appsettings.json` to include your frontend URL:

```json
"Cors": {
  "AllowedOrigins": [
    "http://localhost:4200",
    "http://localhost:3000",
    "https://your-frontend-url.com"
  ]
}
```

---

## 📦 Package Installation Issues

If you encounter package restore issues:

1. **Clear NuGet cache:**
   ```bash
   dotnet nuget locals all --clear
   ```

2. **Restore packages explicitly:**
   ```bash
   dotnet restore --force
   ```

3. **Update packages:**
   ```bash
   dotnet add package PackageName --version LatestVersion
   ```

---

## 🗄️ Database Management

### View Database Tables

```sql
USE SpinTrackDb;

-- List all tables
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'auth';

-- View users
SELECT * FROM auth.[User];

-- View refresh tokens
SELECT * FROM auth.RefreshToken;
```

### Reset Database

```bash
# Drop database
dotnet ef database drop --project ../SpinTrack.Infrastructure --startup-project .

# Recreate
dotnet ef database update --project ../SpinTrack.Infrastructure --startup-project .
```

### Seed Sample Data (Optional)

You can manually insert test data or create a seed script:

```sql
USE SpinTrackDb;

INSERT INTO auth.[User] (UserId, Username, Email, PasswordHash, FirstName, LastName, Gender, DateOfBirth, Nationality, Status, CreatedBy, CreatedAt)
VALUES 
(NEWID(), 'admin', 'admin@spintrack.com', '$2a$11$...', 'Admin', 'User', 'Male', '1985-01-01', 'USA', 'Active', '00000000-0000-0000-0000-000000000000', SYSDATETIMEOFFSET());
```

---

## 🧪 Testing with Different Tools

### Using HTTP File (VS Code / Visual Studio 2022+)

1. Open `SpinTrack.Api/SpinTrack.Api.http`
2. Update the host address if needed
3. Click "Send Request" above each request

### Using Postman

1. Import the following as a collection:
   - Base URL: `https://localhost:7001`
   - Create requests for each endpoint
   - Use environment variables for tokens

### Using cURL

```bash
# Register
curl -X POST https://localhost:7001/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test123!@#",
    "confirmPassword": "Test123!@#",
    "firstName": "Test",
    "lastName": "User",
    "gender": "Male",
    "dateOfBirth": "1990-01-01",
    "nationality": "USA"
  }'

# Login
curl -X POST https://localhost:7001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "Test123!@#"
  }'
```

---

## 🎯 Next Steps

After successful setup:

1. **Explore API Documentation**
   - Read through Swagger UI
   - Test all endpoints

2. **Customize Configuration**
   - Update JWT expiration times
   - Configure CORS for your frontend
   - Set up logging preferences

3. **Implement Additional Features**
   - Add more entities (Projects, Tasks, etc.)
   - Implement role-based authorization
   - Add email notifications

4. **Set Up CI/CD**
   - GitHub Actions
   - Azure DevOps
   - Jenkins

5. **Deploy to Production**
   - Azure App Service
   - AWS Elastic Beanstalk
   - Docker containers

---

## 📚 Additional Resources

- [Official Documentation](README.md)
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

## 🆘 Getting Help

If you encounter issues:

1. Check the logs in `logs/spintrack-{date}.txt`
2. Review error messages in console
3. Verify all prerequisites are installed
4. Check SQL Server connectivity
5. Review configuration settings

---

**Setup Complete! You're ready to start developing with SpinTrack API! 🎉**
