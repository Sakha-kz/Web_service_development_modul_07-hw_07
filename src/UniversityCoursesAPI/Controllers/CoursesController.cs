using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityCoursesAPI.Attributes;
using UniversityCoursesAPI.Models;
using UniversityCoursesAPI.Services;

namespace UniversityCoursesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[BasicAuthentication]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    /// <summary>
    /// Получение всех учебных курсов (Доступ: Admin, Teacher, Student)
    /// </summary>
    [HttpGet]
    [MyAuthorize("Admin", "Teacher", "Student")]
    public IActionResult GetAll()
    {
        var courses = _courseService.GetAll();
        return Ok(new
        {
            message = "Список учебных курсов успешно получен.",
            user = User.Identity?.Name,
            role = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value,
            total = courses.Count(),
            data = courses
        });
    }

    /// <summary>
    /// Получение курса по ID (Доступ: Admin, Teacher, Student)
    /// </summary>
    [HttpGet("{id}")]
    [MyAuthorize("Admin", "Teacher", "Student")]
    public IActionResult GetById(int id)
    {
        var course = _courseService.GetById(id);
        if (course == null)
        {
            return NotFound(new { message = $"Учебный курс с ID {id} не найден." });
        }

        return Ok(new
        {
            message = $"Курс #{id} найден.",
            requestedBy = User.Identity?.Name,
            data = course
        });
    }

    /// <summary>
    /// Создание нового курса (Доступ: Admin, Teacher)
    /// </summary>
    [HttpPost]
    [MyAuthorize("Admin", "Teacher")]
    public IActionResult Create([FromBody] Course course)
    {
        if (course == null || string.IsNullOrWhiteSpace(course.Title))
        {
            return BadRequest(new { message = "Название курса обязательно." });
        }

        var created = _courseService.Add(course);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new
        {
            message = "Учебный курс успешно создан.",
            createdBy = User.Identity?.Name,
            course = created
        });
    }

    /// <summary>
    /// Редактирование курса (Доступ: Admin, Teacher)
    /// </summary>
    [HttpPut("{id}")]
    [MyAuthorize("Admin", "Teacher")]
    public IActionResult Update(int id, [FromBody] Course course)
    {
        if (course == null)
        {
            return BadRequest(new { message = "Данные курса не переданы." });
        }

        var updated = _courseService.Update(id, course);
        if (!updated)
        {
            return NotFound(new { message = $"Курс с ID {id} не найден для обновления." });
        }

        return Ok(new
        {
            message = $"Курс #{id} успешно обновлен.",
            updatedBy = User.Identity?.Name,
            course = _courseService.GetById(id)
        });
    }

    /// <summary>
    /// Удаление курса (Доступ: ТОЛЬКО Admin)
    /// </summary>
    [HttpDelete("{id}")]
    [MyAuthorize("Admin")]
    public IActionResult Delete(int id)
    {
        var deleted = _courseService.Delete(id);
        if (!deleted)
        {
            return NotFound(new { message = $"Курс с ID {id} не найден для удаления." });
        }

        return Ok(new
        {
            message = $"Курс #{id} успешно удален администратором системы.",
            deletedBy = User.Identity?.Name
        });
    }

    /// <summary>
    /// Публичная общая информация об образовательной программе (Доступ: Все, включая анонимных)
    /// </summary>
    [HttpGet("info")]
    [AllowAnonymous]
    public IActionResult GetPublicUniversityInfo()
    {
        return Ok(new
        {
            faculty = "Институт Автоматики и информационных технологий",
            department = "Кафедра Программной инженерии",
            educationalProgram = "6B06101 - Программная инженерия",
            academicYear = "2026-2027",
            status = "Публичный открытый справочник курсов",
            note = "Метод доступен всем клиентам без необходимости аутентификации."
        });
    }
}
