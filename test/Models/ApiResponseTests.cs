using Xunit;
using BookLendingApp.Models;

namespace BookLendingApp.Tests.Models;

public class ApiResponseTests
{
    [Fact]
    public void Success_CreatesSuccessResponse_WithDefaultCode()
    {
        // Arrange
        var data = "test data";
        var message = "Success message";

        // Act
        var response = ApiResponse<string>.Success(data, message);

        // Assert
        Assert.Equal(200, response.Code);
        Assert.Equal(message, response.Message);
        Assert.Equal(data, response.Data);
        Assert.Null(response.Error);
    }

    [Fact]
    public void Success_CreatesSuccessResponse_WithCustomCode()
    {
        // Arrange
        var data = "test data";
        var message = "Created successfully";
        var code = 201;

        // Act
        var response = ApiResponse<string>.Success(data, message, code);

        // Assert
        Assert.Equal(code, response.Code);
        Assert.Equal(message, response.Message);
        Assert.Equal(data, response.Data);
        Assert.Null(response.Error);
    }

    [Fact]
    public void Failure_CreatesFailureResponse_WithDefaultCode()
    {
        // Arrange
        var error = "Error occurred";
        var message = "Operation failed";

        // Act
        var response = ApiResponse<string>.Failure(error, message);

        // Assert
        Assert.Equal(400, response.Code);
        Assert.Equal(message, response.Message);
        Assert.Null(response.Data);
        Assert.Equal(error, response.Error);
    }

    [Fact]
    public void Failure_CreatesFailureResponse_WithCustomCode()
    {
        // Arrange
        var error = "Not found";
        var message = "Resource not found";
        var code = 404;

        // Act
        var response = ApiResponse<string>.Failure(error, message, code);

        // Assert
        Assert.Equal(code, response.Code);
        Assert.Equal(message, response.Message);
        Assert.Null(response.Data);
        Assert.Equal(error, response.Error);
    }

    [Fact]
    public void ApiResponse_CanBeCreated_WithAllProperties()
    {
        // Arrange & Act
        var response = new ApiResponse<int>
        {
            Code = 500,
            Message = "Internal server error",
            Data = 42,
            Error = "Database connection failed"
        };

        // Assert
        Assert.Equal(500, response.Code);
        Assert.Equal("Internal server error", response.Message);
        Assert.Equal(42, response.Data);
        Assert.Equal("Database connection failed", response.Error);
    }

    [Fact]
    public void ApiResponse_SupportsNullData()
    {
        // Act
        var response = ApiResponse<string?>.Success(null, "No data available");

        // Assert
        Assert.Equal(200, response.Code);
        Assert.Equal("No data available", response.Message);
        Assert.Null(response.Data);
        Assert.Null(response.Error);
    }

    [Fact]
    public void ApiResponse_SupportsComplexTypes()
    {
        // Arrange
        var complexData = new { Id = 1, Name = "Test", Items = new[] { "A", "B", "C" } };

        // Act
        var response = ApiResponse<object>.Success(complexData, "Complex data retrieved");

        // Assert
        Assert.Equal(200, response.Code);
        Assert.Equal("Complex data retrieved", response.Message);
        Assert.Equal(complexData, response.Data);
        Assert.Null(response.Error);
    }
}