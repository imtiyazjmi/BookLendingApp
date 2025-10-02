# Book Lending App

ASP.NET Core Web API for managing book lending operations, built as an AWS Lambda serverless application.

## Features

- **Book Management**: CRUD operations for books
- **Book Lending**: Check-out and return functionality
- **ISBN Validation**: Unique ISBN constraint
- **Standard API Responses**: Consistent JSON format with code, message, data, and error fields
- **Database Integration**: PostgreSQL with Entity Framework Core
- **AWS Integration**: SSM Parameter Store for secure configuration
- **Serverless Ready**: AWS Lambda deployment support

## Project Structure

```
BookLendingApp/
├── src/                     # Source code
│   ├── Controllers/         # API controllers
│   ├── Services/           # Business logic
│   ├── Repositories/       # Data access layer
│   ├── Interfaces/         # Contracts
│   ├── Models/             # Data models
│   ├── Data/               # Database context and configuration
│   ├── Migrations/         # EF Core migrations
│   └── Program.cs          # Application entry point
├── deployment/             # AWS deployment files
├── test/                   # Unit tests
└── README.md
```

## API Endpoints

### Books
- `GET /api/books` - Get all books
- `GET /api/books/{id}` - Get book by ID
- `POST /api/books` - Create new book
- `PUT /api/books/{id}` - Update book
- `DELETE /api/books/{id}` - Delete book

### Book Operations
- `POST /api/books/{id}/checkout` - Check out a book
- `POST /api/books/{id}/return` - Return a book

## Configuration

### Environment Variables
```
USE_SSM=false
POSTGRESQL_HOST=localhost
POSTGRESQL_DATABASE=booklendingdb
POSTGRESQL_USERNAME=postgres
POSTGRESQL_PASSWORD=admin
AWS_LAMBDA_REGION=us-east-1
```

### Database Setup
1. Install PostgreSQL
2. Update connection settings in `appsettings.json`
3. Run migrations:
   ```bash
   cd src
   dotnet ef database update
   ```

## Running the Application

### Local Development
```bash
cd src
dotnet run
```

### AWS Lambda Deployment
```bash
cd src
dotnet lambda deploy-serverless --template ../deployment/serverless.template --s3-bucket your-deployment-bucket
```

## API Response Format

All endpoints return standardized JSON responses:

### Success Response
```json
{
  "code": 200,
  "message": "Books retrieved successfully",
  "data": [...],
  "error": null
}
```

### Error Response
```json
{
  "code": 400,
  "message": "Failed to create book",
  "data": null,
  "error": "ISBN already exists"
}
```

## Technologies

- **Framework**: ASP.NET Core 8.0
- **Database**: PostgreSQL with Entity Framework Core
- **Cloud**: AWS Lambda, API Gateway, SSM Parameter Store
- **Architecture**: Clean Architecture with Repository Pattern

## Prerequisites

- .NET 8.0 SDK
- PostgreSQL
- AWS CLI (for deployment)
- Visual Studio or VS Code