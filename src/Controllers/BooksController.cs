using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookLendingApp.Interfaces;
using BookLendingApp.Models;

namespace BookLendingApp.Controllers;

/// <summary>
/// Controller for managing books and book operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>
    /// Get all books
    /// </summary>
    /// <returns>List of all books</returns>
    /// <response code="200">Books retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<Book>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<Book>>), 500)]
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

    /// <summary>
    /// Get a book by ID
    /// </summary>
    /// <param name="id">Book ID</param>
    /// <returns>Book details</returns>
    /// <response code="200">Book retrieved successfully</response>
    /// <response code="404">Book not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<Book>), 200)]
    [ProducesResponseType(typeof(ApiResponse<Book>), 404)]
    [ProducesResponseType(typeof(ApiResponse<Book>), 500)]
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

    /// <summary>
    /// Create a new book
    /// </summary>
    /// <param name="book">Book details</param>
    /// <returns>Created book</returns>
    /// <response code="201">Book created successfully</response>
    /// <response code="400">Bad request - validation error or duplicate ISBN</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Book>), 201)]
    [ProducesResponseType(typeof(ApiResponse<Book>), 400)]
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

    /// <summary>
    /// Check out a book
    /// </summary>
    /// <param name="id">Book ID</param>
    /// <returns>Checked out book</returns>
    /// <response code="200">Book checked out successfully</response>
    /// <response code="400">Book not available for checkout</response>
    [HttpPost("{id}/checkout")]
    [ProducesResponseType(typeof(ApiResponse<Book>), 200)]
    [ProducesResponseType(typeof(ApiResponse<Book>), 400)]
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

    /// <summary>
    /// Return a book
    /// </summary>
    /// <param name="id">Book ID</param>
    /// <returns>Returned book</returns>
    /// <response code="200">Book returned successfully</response>
    /// <response code="400">Book was not checked out</response>
    [HttpPost("{id}/return")]
    [ProducesResponseType(typeof(ApiResponse<Book>), 200)]
    [ProducesResponseType(typeof(ApiResponse<Book>), 400)]
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