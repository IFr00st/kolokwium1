using kolokwium.Models.DTO;
using Microsoft.Data.SqlClient;

namespace kolokwium.Repositories;

public class BooksRepository : IBooksRepository
{
    private readonly IConfiguration _configuration;

    public BooksRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<BooksDTO> getBookAuthors(int id)
    {
        BooksDTO booksDto = null;
        
        var query = @"SELECT 
							books.PK,
							title,
							first_name,
							last_name
						FROM books
						JOIN books_authors ON books_authors.FK_book = books.PK
						JOIN authors ON authors.PK = books_authors.FK_author
						WHERE books.PK = @PK";
	    
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@PK", id);
	    
        await connection.OpenAsync();
        
        var reader = await command.ExecuteReaderAsync();

        var pkOrdinal = reader.GetOrdinal("PK");
        var titleOrdinal = reader.GetOrdinal("title");
        var firstNameOrdinal = reader.GetOrdinal("first_name");
        var lastNameOrdinal = reader.GetOrdinal("last_name");

        while (await reader.ReadAsync())
        {
	        if (booksDto is not null)
	        {
		        booksDto.Authors.Add(new Author()
		        {
			        first_na = reader.GetString(firstNameOrdinal),
			        last_na = reader.GetString(lastNameOrdinal)
		        });
	        }
	        else
	        {
		        booksDto = new BooksDTO()
		        {
			        PK = reader.GetInt32(pkOrdinal),
			        title = reader.GetString(titleOrdinal),
			        Authors = new List<Author>()
			        {
				        new Author()
				        {
					        first_na = reader.GetString(firstNameOrdinal),
					        last_na = reader.GetString(lastNameOrdinal)
				        }
			        }
		        };
	        }
        }
        
        
        
        
        return booksDto;
    }

    public async Task addBookWithAuthors(BooksDTOWithoutID booksDtoWithoutId)
    {
	    var insert = @"INSERT INTO books VALUES(@Title);
						SELECT @@IDENTITY AS ID;";
	    
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();
	    
	    command.Connection = connection;
	    command.CommandText = insert;
	    
	    command.Parameters.AddWithValue("@Title", booksDtoWithoutId.title);
	    

	    await connection.OpenAsync();
	    

	    var transaction = await connection.BeginTransactionAsync();
	    command.Transaction = transaction as SqlTransaction;
	    
	    try
	    {
		    var id = await command.ExecuteScalarAsync();
    
		    foreach (var author in booksDtoWithoutId.Authors)
		    {
			    var authorId = await getAuthorid(author);
			    command.Parameters.Clear();
			    command.CommandText = "INSERT INTO books_authors VALUES(@FK_book, @FK_author)";
			    command.Parameters.AddWithValue("@FK_book", id);
			    command.Parameters.AddWithValue("@FK_author", authorId);

			    await command.ExecuteNonQueryAsync();
		    }

		    await transaction.CommitAsync();
	    }
	    catch (Exception)
	    {
		    await transaction.RollbackAsync();
		    throw;
	    }
	    
	    
	    
	    
    }

    public async Task<int> getAuthorid(AuthorDTO author)
    {
	    var query = @"SELECT PK FROM authors WHERE authors.first_name = @first_name AND authors.last_name = @last_name";
	    
	    await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
	    await using SqlCommand command = new SqlCommand();

	    command.Connection = connection;
	    command.CommandText = query;
	    command.Parameters.AddWithValue("@first_name", author.first_na);
	    command.Parameters.AddWithValue("@last_name", author.last_na);
	    
	    await connection.OpenAsync();

	    var reader = await command.ExecuteReaderAsync();

	    await reader.ReadAsync();

	    var pkOrdinal = reader.GetOrdinal("PK");
	    var id = reader.GetInt32(pkOrdinal);
	    return id;
    }
}