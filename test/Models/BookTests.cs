using Xunit;
using BookStore.Models;

namespace BookLendingApp.Tests.Models;

public class BookTests
{
    [Fact]
    public void Book_CanBeCreated_WithAllProperties()
    {
        // Arrange & Act
        var book = new Book
        {
            Id = 1,
            Title = "Test Book",
            Author = "Test Author",
            Publisher = "Test Publisher",
            ISBN = "978-0123456789",
            Pages = 300,
            UnitsAvailable = 5,
            CheckedOutDate = DateTime.UtcNow
        };

        // Assert
        Assert.Equal(1, book.Id);
        Assert.Equal("Test Book", book.Title);
        Assert.Equal("Test Author", book.Author);
        Assert.Equal("Test Publisher", book.Publisher);
        Assert.Equal("978-0123456789", book.ISBN);
        Assert.Equal(300, book.Pages);
        Assert.Equal(5, book.UnitsAvailable);
        Assert.NotNull(book.CheckedOutDate);
    }

    [Fact]
    public void Book_CanBeModified_AfterCreation()
    {
        // Arrange
        var book = new Book
        {
            Title = "Original Title",
            Author = "Test Author",
            ISBN = "Test ISBN",
            Publisher = "Test Publisher",
            UnitsAvailable = 5,
            CheckedOutDate = null
        };

        // Act
        book.Title = "Updated Title";
        book.UnitsAvailable = 4;
        book.CheckedOutDate = DateTime.UtcNow;

        // Assert
        Assert.Equal("Updated Title", book.Title);
        Assert.Equal(4, book.UnitsAvailable);
        Assert.NotNull(book.CheckedOutDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void Book_UnitsAvailable_AcceptsValidValues(int units)
    {
        // Act
        var book = new Book { Title = "T", Author = "A", ISBN = "I", Publisher = "P", UnitsAvailable = units };

        // Assert
        Assert.Equal(units, book.UnitsAvailable);
    }

    [Fact]
    public void Book_CheckedOutDate_AcceptsNullAndDateTimeValues()
    {
        // Act
        var book1 = new Book { Title = "T", Author = "A", ISBN = "I", Publisher = "P", CheckedOutDate = null };
        var book2 = new Book { Title = "T", Author = "A", ISBN = "I", Publisher = "P", CheckedOutDate = DateTime.UtcNow };

        // Assert
        Assert.Null(book1.CheckedOutDate);
        Assert.NotNull(book2.CheckedOutDate);
    }

    [Fact]
    public void Book_CreatedDate_HasDefaultValue()
    {
        // Act
        var book = new Book { Title = "T", Author = "A", ISBN = "I", Publisher = "P" };

        // Assert
        Assert.True(book.CreatedDate <= DateTime.UtcNow);
        Assert.True(book.CreatedDate > DateTime.UtcNow.AddMinutes(-1));
    }
}