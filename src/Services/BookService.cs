using BookStore.Models;
using BookLendingApp.Interfaces;

namespace BookLendingApp.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _bookRepository.GetAllAsync();
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _bookRepository.GetByIdAsync(id);
    }

    public async Task<Book> CreateBookAsync(Book book)
    {
        return await _bookRepository.CreateAsync(book);
    }

    public async Task<Book> UpdateBookAsync(Book book)
    {
        return await _bookRepository.UpdateAsync(book);
    }

    public async Task DeleteBookAsync(int id)
    {
        await _bookRepository.DeleteAsync(id);
    }

    public async Task<Book?> CheckOutBookAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book != null && book.UnitsAvailable > 0)
        {
            book.UnitsAvailable--;
            book.CheckedOutDate = DateTime.UtcNow;
            return await _bookRepository.UpdateAsync(book);
        }
        return null;
    }

    public async Task<Book?> ReturnBookAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book != null && book.CheckedOutDate.HasValue)
        {
            book.UnitsAvailable++;
            book.CheckedOutDate = null;
            return await _bookRepository.UpdateAsync(book);
        }
        return null;
    }
}