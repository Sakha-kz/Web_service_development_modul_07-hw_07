using Microsoft.OpenApi.Models;
using UniversityCoursesAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Добавление контроллеров
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. Регистрация сервисов приложения в DI контейнере
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<ICourseService, CourseService>();

// 3. Настройка Swagger UI со схемой Basic Authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UniversityCoursesAPI - Module 07 Homework",
        Version = "v1",
        Description = "ASP.NET Core Web API: Управление курсами с ролевым доступом (Admin, Teacher, Student) и Basic Authentication."
    });

    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Введите учетные данные: admin:Admin123, teacher:Teacher123, student:Student123"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UniversityCoursesAPI v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseRouting();

app.MapControllers();

app.Run();
