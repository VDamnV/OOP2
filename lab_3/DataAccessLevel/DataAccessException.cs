namespace DataAccessLevel;

public class DataAccessException : Exception
{
    public DataAccessException() : base("Сталася помилка на рівні доступу до даних.") { }
    public DataAccessException(string message) : base(message) { }
    public DataAccessException(string message, Exception inner) : base(message, inner) { }
}