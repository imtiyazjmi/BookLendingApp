using Microsoft.EntityFrameworkCore;
using Xunit;
using BookLendingApp.Data;
using BookLendingApp.Repositories;
using BookStore.Models;

namespace BookLendingApp.Tests.Repositories;

public class BookRepositoryTests : IDisposable
{
    private readonly BookContext _context;
    private readonly BookRepository _repository;

    public BookRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<BookContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BookContext(options);
        _repository = new BookRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllBooks()
    {
        // Arrange
        var books = new List<Book>
        {
            new Book { Title = "Book 1", Author = "Author 1", ISBN = "123", Publisher = "Pub 1", Pages = 100, UnitsAvailable = 5 },
            new Book { Title = "Book 2", Author = "Author 2", ISBN = "456", Publisher = "Pub 2", Pages = 200, UnitsAvailable = 3 }
        };

        _context.Books.AddRange(books);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBook_WhenBookExists()
    {
        // Arrange
        var book = new Book { Title = "Test Book", Author = "Test Author", ISBN = "789", Publisher = "Test Pub", Pages = 300, UnitsAvailable = 2 };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(book.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(book.Title, result.Title);
        Assert.Equal(book.Author, result.Author);
        Assert.Equal(book.ISBN, result.ISBN);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenBookDoesNotExist()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsBookToDatabase()
    {
        // Arrange
        var book = new Book { Title = "New Book", Author = "New Author", ISBN = "999", Publisher = "New Pub", Pages = 400, UnitsAvailable = 1 };

        // Act
        var result = await _repository.CreateAsync(book);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(book.Title, result.Title);

        var bookInDb = await _context.Books.FindAsync(result.Id);
        Assert.NotNull(bookInDb);
        Assert.Equal(book.Title, bookInDb.Title);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingBook()
    {
        // Arrange
        var book = new Book { Title = "Original Title", Author = "Original Author", ISBN = "111", Publisher = "Original Pub", Pages = 100, UnitsAvailable = 5 };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Modify the book
        book.Title = "Updated Title";
        book.Author = "Updated Author";

        // Act
        var result = await _repository.UpdateAsync(book);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("Updated Author", result.Author);

        var bookInDb = await _context.Books.FindAsync(book.Id);
        Assert.NotNull(bookInDb);
        Assert.Equal("Updated Title", bookInDb.Title);
        Assert.Equal("Updated Author", bookInDb.Author);
    }

    [Fact]
    public async Task DeleteAsync_RemovesBookFromDatabase()
    {
        // Arrange
        var book = new Book { Title = "Book to Delete", Author = "Author", ISBN = "222", Publisher = "Delete Pub", Pages = 150, UnitsAvailable = 3 };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        var bookId = book.Id;

        // Act
        await _repository.DeleteAsync(bookId);

        // Assert
        var bookInDb = await _context.Books.FindAsync(bookId);
        Assert.Null(bookInDb);
    }

    [Fact]
    public async Task CreateAsync_CanAddMultipleBooks()
    {
        // Arrange
        var book1 = new Book { Title = "Book 1", Author = "Author 1", ISBN = "ISBN1", Publisher = "Pub 1", Pages = 100, UnitsAvailable = 5 };
        var book2 = new Book { Title = "Book 2", Author = "Author 2", ISBN = "ISBN2", Publisher = "Pub 2", Pages = 200, UnitsAvailable = 3 };

        // Act
        await _repository.CreateAsync(book1);
        await _repository.CreateAsync(book2);

        // Assert
        var books = await _repository.GetAllAsync();
        Assert.Equal(2, books.Count());
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}