namespace kolokwium.Models.DTO;

public class BooksDTO
{
    public int PK { get; set; }
    public string title { get; set; }
    public List<Author> Authors { get; set; } = new List<Author>();
}

public class Author
{
    public string first_na { get; set; }
    public string last_na { get; set; }
}