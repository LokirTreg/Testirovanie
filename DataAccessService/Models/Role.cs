namespace DataAccessService.Models;

public class Role
{
    public int Id { get; set; } // Уникальный идентификатор роли
    public string Name { get; set; } // Название роли (например, "Администратор", "Студент")
    public string Description { get; set; } // Описание роли

    // Связь с пользователями, имеющими эту роль
    public virtual ICollection<User> Users { get; set; }

    public Role()
    {
        Users = new List<User>(); // Инициализация коллекции пользователей
    }
}
