using Xunit;
using Microsoft.EntityFrameworkCore;
using BookLendingApp.Data;
using BookLendingApp.Repositories;
using BookStore.Models;

namespace BookLendingApp.Tests.Repositories;

public class BookRepositoryIsbnTests
{
    [Fact]
    public async Task CreateAsync_WithDuplicateISBN_ThrowsException()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BookContext>()
            .UseInMemoryDatabase(databaseName: "DuplicateISBNTest")
            .Options;

        using var context = new BookContext(options);
        var repository = new BookRepository(context);

        var book1 = new Book
        {
            Title = "Book 1",
            Author = "Author 1",
            ISBN = "123456789",
            Publisher = "Publisher 1",
            Pages = 100,
            UnitsAvailable = 5
        };

        var book2 = new Book
        {
            Title = "Book 2",
            Author = "Author 2",
            ISBN = "123456789", // Same ISBN
            Publisher = "Publisher 2",
            Pages = 200,
            UnitsAvailable = 3
        };

        // Act
        await repository.CreateAsync(book1);

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => repository.CreateAsync(book2));
        Assert.Contains("A book with ISBN '123456789' already exists", exception.Message);
    }

    [Fact]
    public async Task IsbnExistsAsync_WithExistingISBN_ReturnsTrue()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BookContext>()
            .UseInMemoryDatabase(databaseName: "ISBNExistsTest")
            .Options;

        using var context = new BookContext(options);
        var repository = new BookRepository(context);

        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "987654321",
            Publisher = "Test Publisher",
            Pages = 150,
            UnitsAvailable = 2
        };

        await repository.CreateAsync(book);

        // Act
        var exists = await repository.IsbnExistsAsync("987654321");

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task IsbnExistsAsync_WithNonExistingISBN_ReturnsFalse()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BookContext>()
            .UseInMemoryDatabase(databaseName: "ISBNNotExistsTest")
            .Options;

        using var context = new BookContext(options);
        var repository = new BookRepository(context);

        // Act
        var exists = await repository.IsbnExistsAsync("999999999");

        // Assert
        Assert.False(exists);
    }
}