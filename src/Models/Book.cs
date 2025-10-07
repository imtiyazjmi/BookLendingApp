namespace BookStore.Models;

/// <summary>
/// Book entity representing a book in the library
/// </summary>
public class Book
{
    /// <summary>
    /// Unique identifier for the book
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Title of the book
    /// </summary>
    public required string Title { get; set; }
    
    /// <summary>
    /// Author of the book
    /// </summary>
    public required string Author { get; set; }
    
    /// <summary>
    /// ISBN of the book (must be unique)
    /// </summary>
    public required string ISBN { get; set; }
    
    /// <summary>
    /// Publisher of the book
    /// </summary>
    public required string Publisher { get; set; }
    
    /// <summary>
    /// Number of pages in the book
    /// </summary>
    public int Pages { get; set; }
    
    /// <summary>
    /// Number of units available for checkout
    /// </summary>
    public int UnitsAvailable { get; set; }
    
    /// <summary>
    /// Date when the book was checked out (null if not checked out)
    /// </summary>
    public DateTime? CheckedOutDate { get; set; }
    
    /// <summary>
    /// Date when the book was added to the system
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}