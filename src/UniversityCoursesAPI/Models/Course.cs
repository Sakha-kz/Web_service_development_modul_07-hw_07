namespace UniversityCoursesAPI.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Teacher { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
}
