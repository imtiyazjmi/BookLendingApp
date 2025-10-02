using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using BookLendingApp.Controllers;
using BookLendingApp.Interfaces;
using BookStore.Models;
using BookLendingApp.Models;

namespace BookLendingApp.Tests.Controllers;

public class BooksControllerTests
{
    private readonly Mock<IBookService> _mockBookService;
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _mockBookService = new Mock<IBookService>();
        _controller = new BooksController(_mockBookService.Object);
    }

    [Fact]
    public async Task GetBooks_ReturnsOkResult_WithBooks()
    {
        // Arrange
        var books = new List<Book>
        {
            new Book { Id = 1, Title = "Test Book", Author = "Test Author", ISBN = "123", Publisher = "Test Pub", Pages = 100, UnitsAvailable = 5 }
        };
        _mockBookService.Setup(s => s.GetAllBooksAsync()).ReturnsAsync(books);

        // Act
        var result = await _controller.GetBooks();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Book>>>(okResult.Value);
        Assert.Equal(200, response.Code);
        Assert.Equal("Books retrieved successfully", response.Message);
        Assert.Single(response.Data ?? Array.Empty<Book>());
    }

    [Fact]
    public async Task GetBooks_ReturnsError_WhenExceptionThrown()
    {
        // Arrange
        _mockBookService.Setup(s => s.GetAllBooksAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetBooks();

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
        var response = Assert.IsType<ApiResponse<IEnumerable<Book>>>(statusResult.Value);
        Assert.Equal(500, response.Code);
        Assert.Equal("Failed to retrieve books", response.Message);
    }

    [Fact]
    public async Task GetBook_ReturnsOkResult_WhenBookExists()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Test Book", Author = "Test Author", ISBN = "123", Publisher = "Test Pub", Pages = 100, UnitsAvailable = 5 };
        _mockBookService.Setup(s => s.GetBookByIdAsync(1)).ReturnsAsync(book);

        // Act
        var result = await _controller.GetBook(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Book>>(okResult.Value);
        Assert.Equal(200, response.Code);
        Assert.Equal("Book retrieved successfully", response.Message);
        Assert.Equal(book, response.Data);
    }

    [Fact]
    public async Task GetBook_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        _mockBookService.Setup(s => s.GetBookByIdAsync(1)).ReturnsAsync((Book?)null);

        // Act
        var result = await _controller.GetBook(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Book>>(notFoundResult.Value);
        Assert.Equal(404, response.Code);
        Assert.Equal("Book not found", response.Message);
    }

    [Fact]
    public async Task CreateBook_ReturnsCreatedResult_WhenBookIsValid()
    {
        // Arrange
        var book = new Book { Title = "New Book", Author = "New Author", ISBN = "456", Publisher = "New Pub", Pages = 200, UnitsAvailable = 3 };
        var createdBook = new Book { Id = 1, Title = "New Book", Author = "New Author", ISBN = "456", Publisher = "New Pub", Pages = 200, UnitsAvailable = 3 };
        _mockBookService.Setup(s => s.CreateBookAsync(book)).ReturnsAsync(createdBook);

        // Act
        var result = await _controller.CreateBook(book);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Book>>(createdResult.Value);
        Assert.Equal(201, response.Code);
        Assert.Equal("Book created successfully", response.Message);
        Assert.Equal(createdBook, response.Data);
    }

    [Fact]
    public async Task CreateBook_ReturnsBadRequest_WhenISBNExists()
    {
        // Arrange
        var book = new Book { Title = "New Book", Author = "New Author", ISBN = "456", Publisher = "New Pub", Pages = 200, UnitsAvailable = 3 };
        _mockBookService.Setup(s => s.CreateBookAsync(book))
            .ThrowsAsync(new Exception("duplicate key value violates unique constraint \"IX_Books_ISBN\""));

        // Act
        var result = await _controller.CreateBook(book);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Book>>(badRequestResult.Value);
        Assert.Equal(400, response.Code);
        Assert.Equal("Failed to create book", response.Message);
        Assert.Equal("ISBN already exists", response.Error);
    }

    [Fact]
    public async Task CheckOutBook_ReturnsOkResult_WhenBookAvailable()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Test Book", Author = "Test Author", ISBN = "123", Publisher = "Test Pub", Pages = 100, UnitsAvailable = 4, CheckedOutDate = DateTime.UtcNow };
        _mockBookService.Setup(s => s.CheckOutBookAsync(1)).ReturnsAsync(book);

        // Act
        var result = await _controller.CheckOutBook(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Book>>(okResult.Value);
        Assert.Equal(200, response.Code);
        Assert.Equal("Book checked out successfully", response.Message);
        Assert.Equal(book, response.Data);
    }

    [Fact]
    public async Task ReturnBook_ReturnsOkResult_WhenBookWasCheckedOut()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Test Book", Author = "Test Author", ISBN = "123", Publisher = "Test Pub", Pages = 100, UnitsAvailable = 5, CheckedOutDate = null };
        _mockBookService.Setup(s => s.ReturnBookAsync(1)).ReturnsAsync(book);

        // Act
        var result = await _controller.ReturnBook(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Book>>(okResult.Value);
        Assert.Equal(200, response.Code);
        Assert.Equal("Book returned successfully", response.Message);
        Assert.Equal(book, response.Data);
    }
}