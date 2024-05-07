using kolokwium.Models.DTO;

namespace kolokwium.Repositories;

public interface IBooksRepository
{
    public Task<BooksDTO> getBookAuthors(int id);
    public Task addBookWithAuthors(BooksDTOWithoutID booksDto);

    public Task<int> getAuthorid(AuthorDTO author);
}