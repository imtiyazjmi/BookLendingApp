# Book Lending App

ASP.NET Core Web API for managing book lending operations, built as an AWS Lambda serverless application with comprehensive testing and CI/CD pipeline.

## 🚀 Features

- **Book Management**: Create, read operations for books
- **Book Lending**: Check-out and return functionality
- **ISBN Validation**: Unique ISBN constraint with duplicate prevention
- **Standard API Responses**: Consistent JSON format with code, message, data, and error fields
- **Database Integration**: PostgreSQL for AWS, In-Memory for local development
- **AWS Integration**: Lambda, API Gateway, RDS with automated deployment
- **Serverless Architecture**: AWS Lambda with dynamic packaging
- **Comprehensive Testing**: 86.2% code coverage with 51 unit tests
- **CI/CD Pipeline**: GitHub Actions with automated deployment

## 📁 Project Structure

```
BookLendingApp/
├── src/                           # Source code
│   ├── Controllers/               # API controllers
│   │   └── BooksController.cs     # Books API endpoints
│   ├── Services/                  # Business logic
│   │   ├── BookService.cs         # Book business logic
│   │   ├── DatabaseConfigurationService.cs  # DB config
│   │   └── ParameterStoreService.cs         # AWS SSM integration
│   ├── Repositories/              # Data access layer
│   │   └── BookRepository.cs      # Book data operations
│   ├── Interfaces/                # Contracts
│   │   ├── IBookService.cs        # Service interface
│   │   └── IBookRepository.cs     # Repository interface
│   ├── Models/                    # Data models
│   │   ├── Book.cs                # Book entity
│   │   └── ApiResponse.cs         # Standard response wrapper
│   ├── Data/                      # Database context
│   │   ├── BookContext.cs         # EF Core context
│   │   └── DatabaseSettings.cs    # Configuration models
│   ├── Migrations/                # EF Core migrations
│   │   ├── 20250924171454_InitialCreate.cs
│   │   ├── 20250924173959_AddUniqueISBN.cs
│   │   └── BookContextModelSnapshot.cs
│   ├── Properties/                # Launch configuration
│   │   └── launchSettings.json    # Development settings
│   ├── Program.cs                 # Application entry point
│   ├── LambdaEntryPoint.cs        # AWS Lambda entry point
│   ├── BookLendingApp.csproj      # Project file
│   └── appsettings.json           # Configuration
├── test/                          # Unit tests (51 tests, 86.2% coverage)
│   ├── Controllers/               # Controller tests
│   ├── Services/                  # Service tests
│   ├── Repositories/              # Repository tests
│   ├── Models/                    # Model tests
│   ├── Data/                      # Data layer tests
│   ├── Integration/               # Integration tests
│   ├── BookLendingApp.Tests.csproj # Test project file
│   └── coverlet.runsettings       # Coverage configuration
├── deployment/                    # AWS deployment files
│   ├── minimal-infrastructure.yaml # CloudFormation template
│   ├── api-gateway.yaml          # API Gateway configuration
│   ├── swagger.json               # OpenAPI specification (JSON)
│   └── aws-lambda-tools-defaults.json # Lambda deployment config
├── .github/workflows/             # CI/CD pipeline
│   └── deploy.yml                 # GitHub Actions workflow
├── BookLendingApp.sln             # Solution file
└── README.md                      # Project documentation
```

## 🔗 API Endpoints

### Books Management
- `GET /api/books` - Get all books
- `GET /api/books/{id}` - Get book by ID
- `POST /api/books` - Create new book

### Book Operations
- `POST /api/books/{id}/checkout` - Check out a book
- `POST /api/books/{id}/return` - Return a book

## 🗄️ Database Configuration

### Local Development (In-Memory)
- Uses Entity Framework In-Memory database
- No PostgreSQL installation required
- Automatic data seeding for testing

### AWS Production (PostgreSQL)
- RDS PostgreSQL instance
- Automatic migrations on Lambda startup
- Environment-based configuration

### Environment Variables
```bash
# Local Development (automatically uses in-memory DB)
ASPNETCORE_ENVIRONMENT=Development

# AWS Production
USE_SSM=false
POSTGRESQL_HOST=your-rds-endpoint
POSTGRESQL_DATABASE=postgres
POSTGRESQL_USERNAME=masteruser
POSTGRESQL_PASSWORD=your-password
AWS_LAMBDA_REGION=us-east-1
```

## 🏃‍♂️ Running the Application

### Local Development
```bash
cd src
dotnet run
# Uses in-memory database automatically
# API available at http://localhost:5000/swagger
```

### Unit Tests with Coverage
```bash
cd test
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:TestResults/CoverageReport -reporttypes:Html
# Open TestResults/CoverageReport/index.html
```

### AWS Lambda Deployment
```bash
# Automated via GitHub Actions on push to main
# Manual deployment:
cd src
dotnet lambda package --configuration Release --framework net8.0
```

## 📊 Testing & Quality

### Test Coverage: 86.2%
- **Total Tests**: 51
- **Line Coverage**: 86.2% (151/175 lines)
- **Branch Coverage**: 65.3% (17/26 branches)
- **Method Coverage**: 94.8% (37/39 methods)

### Coverage by Component
- **BookContext**: 100% ✅
- **BookRepository**: 100% ✅
- **ApiResponse**: 100% ✅
- **Book Model**: 100% ✅
- **BooksController**: 79.2% ✅
- **BookService**: 70.4% ✅

### Test Categories
- **Controller Tests**: API endpoint testing with mocked services
- **Service Tests**: Business logic validation
- **Repository Tests**: Data access and ISBN validation
- **Model Tests**: Entity and response model validation
- **Integration Tests**: End-to-end API testing

## 🚀 CI/CD Pipeline

### GitHub Actions Workflow
- **Trigger**: Push to main branch
- **Build**: .NET 8.0 compilation
- **Test**: Unit test execution with coverage
- **Package**: Dynamic Lambda package naming (`lambda-function-YYYYMMDD-HHMMSS.zip`)
- **Deploy**: Automated AWS infrastructure and Lambda deployment
- **Configure**: Environment variables and API Gateway setup

### Deployment Steps
1. Infrastructure deployment (CloudFormation)
2. Lambda package upload to S3
3. Lambda function code and configuration update
4. API Gateway deployment
5. Environment variables configuration

## 📋 API Response Format

All endpoints return standardized JSON responses:

### Success Response
```json
{
  "code": 200,
  "message": "Books retrieved successfully",
  "data": [
    {
      "id": 1,
      "title": "The Great Gatsby",
      "author": "F. Scott Fitzgerald",
      "isbn": "978-0-7432-7356-5",
      "publisher": "Scribner",
      "pages": 180,
      "unitsAvailable": 5,
      "checkedOutDate": null,
      "createdDate": "2024-01-15T09:00:00Z"
    }
  ],
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

## 🛠️ Technologies

### Backend
- **Framework**: ASP.NET Core 8.0
- **Database**: PostgreSQL (AWS) / In-Memory (Local)
- **ORM**: Entity Framework Core 9.0
- **Architecture**: Clean Architecture with Repository Pattern

### AWS Services
- **Compute**: AWS Lambda
- **API**: API Gateway
- **Database**: RDS PostgreSQL
- **Storage**: S3 (deployment packages)
- **Networking**: VPC, Security Groups

### Testing
- **Framework**: xUnit
- **Mocking**: Moq
- **Coverage**: Coverlet + ReportGenerator
- **Database**: Entity Framework In-Memory

### DevOps
- **CI/CD**: GitHub Actions
- **Infrastructure**: CloudFormation
- **Packaging**: AWS Lambda Tools
- **Documentation**: OpenAPI/Swagger

## 📚 API Documentation

### Swagger/OpenAPI
- **JSON**: `deployment/swagger.json`
- **Import to AWS API Gateway**: Ready for direct import
- **Interactive UI**: Available at `/swagger` endpoint during development

### Key Features
- Complete schema definitions
- Request/response examples
- Validation rules
- Error response documentation
- ISBN uniqueness constraints

## 🔧 Prerequisites

### Development
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- Git

### AWS Deployment
- AWS CLI configured
- AWS account with appropriate permissions
- S3 bucket for deployment packages

### Optional
- PostgreSQL (for local PostgreSQL testing)
- Docker (for containerized PostgreSQL)

## 🏗️ Architecture

### Clean Architecture Layers
1. **Controllers**: API endpoints and HTTP handling
2. **Services**: Business logic and validation
3. **Repositories**: Data access abstraction
4. **Models**: Domain entities and DTOs
5. **Data**: Database context and configuration

### Design Patterns
- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Service registration and resolution
- **Factory Pattern**: ApiResponse creation
- **Strategy Pattern**: Database provider selection (In-Memory vs PostgreSQL)

## 🔒 Security & Validation

- **ISBN Uniqueness**: Database constraint with application-level validation
- **Input Validation**: Model validation attributes
- **Error Handling**: Comprehensive exception handling
- **Environment Separation**: Different configurations for local/AWS
- **Secure Configuration**: Environment variables for sensitive data

## 📈 Performance

- **Serverless**: Auto-scaling with AWS Lambda
- **Connection Pooling**: Entity Framework connection management
- **Async Operations**: All database operations are asynchronous
- **Minimal API Surface**: Only essential endpoints exposed
- **Efficient Queries**: Optimized Entity Framework queries