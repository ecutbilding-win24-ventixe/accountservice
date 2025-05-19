namespace Presentation.Models;

public class AccountServiceResult
{
    public bool Succeeded { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
}
