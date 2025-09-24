using BookStore.Models;

namespace BookLendingApp.Interfaces;

public interface IBookService
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(int id);
    Task<Book> CreateBookAsync(Book book);
    Task<Book> UpdateBookAsync(Book book);
    Task DeleteBookAsync(int id);
    Task<Book?> CheckOutBookAsync(int id);
    Task<Book?> ReturnBookAsync(int id);
}