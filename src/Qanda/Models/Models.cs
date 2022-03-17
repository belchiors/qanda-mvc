namespace Qanda.Models;

public class User
{
    public int? UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string Role { get; set; } = "User";
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
}

public class Question
{
    public int? QuestionId { get; set; }
    public int? UserId { get; set; }
    public string? Author { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public string? Url { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public virtual List<Answer>? Answers { get; set; }
}

public class Answer
{
    public int? AnswerId { get; set; }
    public int? UserId { get; set; }
    public string? Author { get; set; }
    public string? Content { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public virtual int? QuestionId { get; set; }
    public virtual Question? Question { get; set; }
}