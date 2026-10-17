using UniversityCoursesAPI.Models;

namespace UniversityCoursesAPI.Services;

public interface IUserService
{
    User? ValidateUser(string username, string password);
    IEnumerable<User> GetAll();
}

public class UserService : IUserService
{
    private readonly List<User> _users = new()
    {
        new User
        {
            Id = 1,
            UserName = "admin",
            Password = "Admin123",
            Email = "admin@university.edu.kz",
            Role = "Admin"
        },
        new User
        {
            Id = 2,
            UserName = "teacher",
            Password = "Teacher123",
            Email = "teacher@university.edu.kz",
            Role = "Teacher"
        },
        new User
        {
            Id = 3,
            UserName = "student",
            Password = "Student123",
            Email = "student@university.edu.kz",
            Role = "Student"
        }
    };

    public User? ValidateUser(string username, string password)
    {
        return _users.FirstOrDefault(u =>
            string.Equals(u.UserName, username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);
    }

    public IEnumerable<User> GetAll() => _users;
}
