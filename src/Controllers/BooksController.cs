using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookLendingApp.Interfaces;
using BookLendingApp.Models;

namespace BookLendingApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<Book>>>> GetBooks()
    {
        try
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(ApiResponse<IEnumerable<Book>>.Success(books, "Books retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<Book>>.Failure(ex.Message, "Failed to retrieve books", 500));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Book>>> GetBook(int id)
    {
        try
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound(ApiResponse<Book>.Failure("Book not found", "Book not found", 404));
            return Ok(ApiResponse<Book>.Success(book, "Book retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Book>.Failure(ex.Message, "Failed to retrieve book", 500));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Book>>> CreateBook(Book book)
    {
        try
        {
            var createdBook = await _bookService.CreateBookAsync(book);
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, 
                ApiResponse<Book>.Success(createdBook, "Book created successfully", 201));
        }
        catch (Exception ex)
        {
            var errorMessage = ex.Message.Contains("duplicate key value violates unique constraint \"IX_Books_ISBN\"") 
                ? "ISBN already exists" : ex.Message;
            return BadRequest(ApiResponse<Book>.Failure(errorMessage, "Failed to create book"));
        }
    }

    [HttpPost("{id}/checkout")]
    public async Task<ActionResult<ApiResponse<Book>>> CheckOutBook(int id)
    {
        try
        {
            var book = await _bookService.CheckOutBookAsync(id);
            if (book == null)
                return BadRequest(ApiResponse<Book>.Failure("Book not available for checkout", "Checkout failed"));
            return Ok(ApiResponse<Book>.Success(book, "Book checked out successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<Book>.Failure(ex.Message, "Failed to checkout book"));
        }
    }

    [HttpPost("{id}/return")]
    public async Task<ActionResult<ApiResponse<Book>>> ReturnBook(int id)
    {
        try
        {
            var book = await _bookService.ReturnBookAsync(id);
            if (book == null)
                return BadRequest(ApiResponse<Book>.Failure("Book was not checked out", "Return failed"));
            return Ok(ApiResponse<Book>.Success(book, "Book returned successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<Book>.Failure(ex.Message, "Failed to return book"));
        }
    }
}