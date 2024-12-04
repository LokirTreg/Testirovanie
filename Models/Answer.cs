namespace Models;

public class Answer
{
    public int Id { get; set; } // Уникальный идентификатор вопроса
    public int TestId { get; set; } // Идентификатор теста, к которому относится вопрос
    public string? Content { get; set; } // Содержимое вопроса
    public virtual Test? Test { get; set; } // Связь с тестом
}
