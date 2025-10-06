using BookStore.Models;

namespace BookLendingApp.Interfaces;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int id);
    Task<Book> CreateAsync(Book book);
    Task<Book> UpdateAsync(Book book);
    Task DeleteAsync(int id);
    Task<bool> IsbnExistsAsync(string isbn);
}