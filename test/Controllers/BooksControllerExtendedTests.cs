using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using BookLendingApp.Controllers;
using BookLendingApp.Interfaces;
using BookStore.Models;
using BookLendingApp.Models;

namespace BookLendingApp.Tests.Controllers;

public class BooksControllerExtendedTests
{
    private readonly Mock<IBookService> _mockBookService;
    private readonly BooksController _controller;

    public BooksControllerExtendedTests()
    {
        _mockBookService = new Mock<IBookService>();
        _controller = new BooksController(_mockBookService.Object);
    }

    [Fact]
    public async Task GetBooks_ReturnsEmptyList_WhenNoBooksExist()
    {
        // Arrange
        _mockBookService.Setup(s => s.GetAllBooksAsync()).ReturnsAsync(new List<Book>());

        // Act
        var result = await _controller.GetBooks();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<IEnumerable<Book>>>(okResult.Value);
        Assert.Equal(200, response.Code);
        Assert.Empty(response.Data!);
    }

    [Fact]
    public async Task CreateBook_WithValidBook_ReturnsCreated()
    {
        // Arrange
        var book = new Book { Title = "New Book", Author = "Author", ISBN = "123", Publisher = "Pub" };
        _mockBookService.Setup(s => s.CreateBookAsync(book)).ReturnsAsync(book);

        // Act
        var result = await _controller.CreateBook(book);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<ApiResponse<Book>>(createdResult.Value);
        Assert.Equal(201, response.Code);
        Assert.Equal("Book created successfully", response.Message);
        Assert.Equal(book, response.Data);
    }

    [Fact]
    public async Task GetBook_WithValidId_ReturnsBook()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Test Book", Author = "Author", ISBN = "123", Publisher = "Pub" };
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
    public async Task CheckOutBook_WithValidId_ReturnsCheckedOutBook()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Test Book", Author = "Author", ISBN = "123", Publisher = "Pub", CheckedOutDate = DateTime.UtcNow };
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
    public async Task ReturnBook_WithValidId_ReturnsReturnedBook()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Test Book", Author = "Author", ISBN = "123", Publisher = "Pub", CheckedOutDate = null };
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