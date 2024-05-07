using kolokwium.Models.DTO;
using kolokwium.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace kolokwium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class BooksController : ControllerBase

    {
        private readonly IBooksRepository _booksRepository;

        public BooksController(IBooksRepository booksRepository)
        {
            _booksRepository = booksRepository;
        }
        
        [HttpGet("{id}/authors")]
        public async Task<IActionResult> GetBookAndAuthors(int id)
        {
            var book = await _booksRepository.getBookAuthors(id);
            return Ok(book);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddBooks(BooksDTOWithoutID booksDto )
        {

            await _booksRepository.addBookWithAuthors(booksDto);

            return Created(Request.Path.Value ?? "api/books", booksDto);
        }
    }
}