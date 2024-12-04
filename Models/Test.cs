namespace Models;

public class Test
{
    public int Id { get; set; } // Уникальный идентификатор теста
    public string? Comment { get; set; } // Название теста
    public DateTime CreatedAt { get; set; } // Дата создания теста
    public virtual ICollection<Answer> Answers { get; set; } = null!; // Вопросы, входящие в тест
}