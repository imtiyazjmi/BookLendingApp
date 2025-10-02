using Moq;
using Xunit;
using BookLendingApp.Services;
using BookLendingApp.Interfaces;
using BookStore.Models;

namespace BookLendingApp.Tests.Services;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _mockRepository;
    private readonly BookService _service;

    public BookServiceTests()
    {
        _mockRepository = new Mock<IBookRepository>();
        _service = new BookService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllBooksAsync_ReturnsAllBooks()
    {
        // Arrange
        var books = new List<Book>
        {
            new Book { Id = 1, Title = "Book 1", Author = "Author 1", ISBN = "123", Publisher = "Pub 1", Pages = 100, UnitsAvailable = 5 },
            new Book { Id = 2, Title = "Book 2", Author = "Author 2", ISBN = "456", Publisher = "Pub 2", Pages = 200, UnitsAvailable = 3 }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(books);

        // Act
        var result = await _service.GetAllBooksAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal(books, result);
    }

    [Fact]
    public async Task GetBookByIdAsync_ReturnsBook_WhenBookExists()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Test Book", Author = "Test Author", ISBN = "123", Publisher = "Test Pub", Pages = 100, UnitsAvailable = 5 };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);

        // Act
        var result = await _service.GetBookByIdAsync(1);

        // Assert
        Assert.Equal(book, result);
    }

    [Fact]
    public async Task GetBookByIdAsync_ReturnsNull_WhenBookDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Book?)null);

        // Act
        var result = await _service.GetBookByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateBookAsync_ReturnsCreatedBook()
    {
        // Arrange
        var book = new Book { Title = "New Book", Author = "New Author", ISBN = "789", Publisher = "New Pub", Pages = 300, UnitsAvailable = 2 };
        var createdBook = new Book { Id = 1, Title = "New Book", Author = "New Author", ISBN = "789", Publisher = "New Pub", Pages = 300, UnitsAvailable = 2 };
        _mockRepository.Setup(r => r.CreateAsync(book)).ReturnsAsync(createdBook);

        // Act
        var result = await _service.CreateBookAsync(book);

        // Assert
        Assert.Equal(createdBook, result);
        _mockRepository.Verify(r => r.CreateAsync(book), Times.Once);
    }

    [Fact]
    public async Task UpdateBookAsync_ReturnsUpdatedBook()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Updated Book", Author = "Updated Author", ISBN = "999", Publisher = "Updated Pub", Pages = 400, UnitsAvailable = 1 };
        _mockRepository.Setup(r => r.UpdateAsync(book)).ReturnsAsync(book);

        // Act
        var result = await _service.UpdateBookAsync(book);

        // Assert
        Assert.Equal(book, result);
        _mockRepository.Verify(r => r.UpdateAsync(book), Times.Once);
    }

    [Fact]
    public async Task DeleteBookAsync_CallsRepositoryDelete()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteBookAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task CheckOutBookAsync_ReturnsBook_WhenBookAvailable()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Test Book", Author = "Test Author", ISBN = "123", Publisher = "Test Pub", Pages = 100, UnitsAvailable = 5 };
        var checkedOutBook = new Book { Id = 1, Title = "Test Book", Author = "Test Author", ISBN = "123", Publisher = "Test Pub", Pages = 100, UnitsAvailable = 4, CheckedOutDate = DateTime.UtcNow };
        
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Book>())).ReturnsAsync(checkedOutBook);

        // Act
        var result = await _service.CheckOutBookAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.CheckedOutDate);
        Assert.Equal(4, result.UnitsAvailable);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task CheckOutBookAsync_ReturnsNull_WhenBookNotAvailable()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Test Book", Author = "Test Author", ISBN = "123", Publisher = "Test Pub", Pages = 100, UnitsAvailable = 0 };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);

        // Act
        var result = await _service.CheckOutBookAsync(1);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Book>()), Times.Never);
    }

    [Fact]
    public async Task CheckOutBookAsync_ReturnsNull_WhenBookDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Book?)null);

        // Act
        var result = await _service.CheckOutBookAsync(1);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Book>()), Times.Never);
    }

    [Fact]
    public async Task ReturnBookAsync_ReturnsBook_WhenBookWasCheckedOut()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Test Book", Author = "Test Author", ISBN = "123", Publisher = "Test Pub", Pages = 100, UnitsAvailable = 4, CheckedOutDate = DateTime.UtcNow };
        var returnedBook = new Book { Id = 1, Title = "Test Book", Author = "Test Author", ISBN = "123", Publisher = "Test Pub", Pages = 100, UnitsAvailable = 5, CheckedOutDate = null };
        
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Book>())).ReturnsAsync(returnedBook);

        // Act
        var result = await _service.ReturnBookAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.CheckedOutDate);
        Assert.Equal(5, result.UnitsAvailable);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task ReturnBookAsync_ReturnsNull_WhenBookWasNotCheckedOut()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Test Book", Author = "Test Author", ISBN = "123", Publisher = "Test Pub", Pages = 100, UnitsAvailable = 5 };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(book);

        // Act
        var result = await _service.ReturnBookAsync(1);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Book>()), Times.Never);
    }

    [Fact]
    public async Task ReturnBookAsync_ReturnsNull_WhenBookDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Book?)null);

        // Act
        var result = await _service.ReturnBookAsync(1);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Book>()), Times.Never);
    }
}