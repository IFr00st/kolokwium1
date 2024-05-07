namespace kolokwium.Models.DTO;

public class BooksDTOWithoutID
{
    public string title { get; set; }
    public List<AuthorDTO> Authors { get; set; } = new List<AuthorDTO>();
}

public class AuthorDTO
{
    public string first_na { get; set; }
    public string last_na { get; set; }
}