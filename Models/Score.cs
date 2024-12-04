namespace Models;

public class Score
{
    public int Id { get; set; } // Уникальный идентификатор оценки
    public int UserId { get; set; } // Идентификатор пользователя, который прошел тест
    public int TestId { get; set; } // Идентификатор теста, который был пройден
    public int PointsEarned { get; set; } // Набранные баллы
    public DateTime TakenAt { get; set; } // Дата и время прохождения теста

    public virtual User User { get; set; } = null!; // Связь с пользователем
    public virtual Test Test { get; set; } = null!; // Связь с тестом
}
