using UniversityCoursesAPI.Models;

namespace UniversityCoursesAPI.Services;

public interface ICourseService
{
    IEnumerable<Course> GetAll();
    Course? GetById(int id);
    Course Add(Course course);
    bool Update(int id, Course course);
    bool Delete(int id);
}

public class CourseService : ICourseService
{
    private readonly List<Course> _courses = new()
    {
        new Course
        {
            Id = 1,
            Title = "Разработка веб-сервисов (CSE5032)",
            Description = "Изучение RESTful Web API, Dependency Injection, аутентификации Basic/JWT и фильтров ASP.NET Core",
            Teacher = "Герцен Е.А.",
            Duration = "15 недель (5 ECTS)"
        },
        new Course
        {
            Id = 2,
            Title = "Проектирование баз данных и NoSQL",
            Description = "Реляционные модели, Entity Framework Core, MongoDB, агрегационные запросы и шардирование",
            Teacher = "Абдолдина Ф.Н.",
            Duration = "15 недель (5 ECTS)"
        },
        new Course
        {
            Id = 3,
            Title = "Архитектура распределенных систем",
            Description = "Микросервисы, очереди сообщений RabbitMQ, gRPC, API Gateway и паттерны отказоустойчивости",
            Teacher = "Смагулов А.К.",
            Duration = "15 недель (6 ECTS)"
        },
        new Course
        {
            Id = 4,
            Title = "Информационная безопасность веб-приложений",
            Description = "OAuth2, OpenID Connect, криптография, защита от OWASP Top-10 и безопасность API",
            Teacher = "Касымов Д.Т.",
            Duration = "15 недель (5 ECTS)"
        },
        new Course
        {
            Id = 5,
            Title = "Машинное обучение и анализ данных",
            Description = "Алгоритмы классификации, нейронные сети, интеграция ML моделей в корпоративные веб-сервисы",
            Teacher = "Нурланов Б.С.",
            Duration = "15 недель (6 ECTS)"
        }
    };

    public IEnumerable<Course> GetAll() => _courses;

    public Course? GetById(int id)
    {
        return _courses.FirstOrDefault(c => c.Id == id);
    }

    public Course Add(Course course)
    {
        var newId = _courses.Any() ? _courses.Max(c => c.Id) + 1 : 1;
        course.Id = newId;
        _courses.Add(course);
        return course;
    }

    public bool Update(int id, Course course)
    {
        var existing = GetById(id);
        if (existing == null) return false;

        existing.Title = course.Title;
        existing.Description = course.Description;
        existing.Teacher = course.Teacher;
        existing.Duration = course.Duration;
        return true;
    }

    public bool Delete(int id)
    {
        var existing = GetById(id);
        if (existing == null) return false;

        return _courses.Remove(existing);
    }
}
