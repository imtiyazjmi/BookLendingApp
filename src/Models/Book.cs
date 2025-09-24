namespace BookStore.Models;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required string ISBN { get; set; }
    public required string Publisher { get; set; }
    public int Pages { get; set; }
    public int UnitsAvailable { get; set; }
    public DateTime? CheckedOutDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}