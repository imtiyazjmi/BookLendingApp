using Microsoft.EntityFrameworkCore;
using Xunit;
using BookLendingApp.Data;
using BookStore.Models;

namespace BookLendingApp.Tests.Data;

public class BookContextTests : IDisposable
{
    private readonly BookContext _context;

    public BookContextTests()
    {
        var options = new DbContextOptionsBuilder<BookContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BookContext(options);
    }

    [Fact]
    public void BookContext_CanCreateDatabase()
    {
        // Act
        var created = _context.Database.EnsureCreated();

        // Assert
        Assert.True(created);
    }

    [Fact]
    public async Task BookContext_CanAddBook()
    {
        // Arrange
        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "123456789",
            Publisher = "Test Publisher",
            Pages = 200,
            UnitsAvailable = 5
        };

        // Act
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        // Assert
        var savedBook = await _context.Books.FirstOrDefaultAsync();
        Assert.NotNull(savedBook);
        Assert.Equal(book.Title, savedBook.Title);
        Assert.True(savedBook.Id > 0);
    }

    [Fact]
    public async Task BookContext_CanAddMultipleBooks()
    {
        // Arrange
        var book1 = new Book { Title = "Book 1", Author = "Author 1", ISBN = "ISBN1", Publisher = "Pub 1", Pages = 100, UnitsAvailable = 1 };
        var book2 = new Book { Title = "Book 2", Author = "Author 2", ISBN = "ISBN2", Publisher = "Pub 2", Pages = 200, UnitsAvailable = 2 };

        // Act
        _context.Books.AddRange(book1, book2);
        await _context.SaveChangesAsync();

        // Assert
        var books = await _context.Books.ToListAsync();
        Assert.Equal(2, books.Count);
    }

    [Fact]
    public async Task BookContext_CanQueryBooks()
    {
        // Arrange
        var books = new[]
        {
            new Book { Title = "Book A", Author = "Author A", ISBN = "ISBN-A", Publisher = "Pub A", Pages = 100, UnitsAvailable = 1 },
            new Book { Title = "Book B", Author = "Author B", ISBN = "ISBN-B", Publisher = "Pub B", Pages = 200, UnitsAvailable = 2 },
            new Book { Title = "Book C", Author = "Author C", ISBN = "ISBN-C", Publisher = "Pub C", Pages = 300, UnitsAvailable = 3 }
        };

        _context.Books.AddRange(books);
        await _context.SaveChangesAsync();

        // Act
        var allBooks = await _context.Books.ToListAsync();
        var bookByAuthor = await _context.Books.Where(b => b.Author == "Author B").FirstOrDefaultAsync();

        // Assert
        Assert.Equal(3, allBooks.Count);
        Assert.NotNull(bookByAuthor);
        Assert.Equal("Book B", bookByAuthor.Title);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}