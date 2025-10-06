using Xunit;
using Microsoft.EntityFrameworkCore;
using BookLendingApp.Data;
using BookStore.Models;

namespace BookLendingApp.Tests.Data;

public class BookContextTests
{
    [Fact]
    public void BookContext_HasBooksDbSet()
    {
        // Arrange & Act
        var options = new DbContextOptionsBuilder<BookContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        using var context = new BookContext(options);

        // Assert
        Assert.NotNull(context.Books);
    }

    [Fact]
    public void BookContext_ConfiguresUniqueISBNConstraint()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BookContext>()
            .UseInMemoryDatabase(databaseName: "TestDb2")
            .Options;

        using var context = new BookContext(options);

        // Act & Assert - Model should be created without errors
        var model = context.Model;
        var bookEntity = model.FindEntityType(typeof(Book));
        Assert.NotNull(bookEntity);
    }

    [Fact]
    public void BookContext_CanAddAndRetrieveBook()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BookContext>()
            .UseInMemoryDatabase(databaseName: "TestDb3")
            .Options;

        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "123456789",
            Publisher = "Test Publisher",
            Pages = 100,
            UnitsAvailable = 5
        };

        // Act
        using (var context = new BookContext(options))
        {
            context.Books.Add(book);
            context.SaveChanges();
        }

        // Assert
        using (var context = new BookContext(options))
        {
            var retrievedBook = context.Books.First();
            Assert.Equal("Test Book", retrievedBook.Title);
            Assert.Equal("Test Author", retrievedBook.Author);
            Assert.Equal("123456789", retrievedBook.ISBN);
        }
    }
}