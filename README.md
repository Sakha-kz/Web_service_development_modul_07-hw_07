# Модуль 07. Домашнее задание: UniversityCoursesAPI (Система курсов с Basic Auth и ролями)

Самостоятельная работа студентов (СРС / Домашнее задание) по дисциплине «Разработка веб-сервисов» (CSE5032).  
Кафедра Программной инженерии, Институт Автоматики и информационных технологий.

---

## 🎯 Цель работы
Закрепить знания об аутентификации и авторизации в ASP.NET Core Web API, самостоятельно реализовать механизм Basic Authentication, управлять доступом к ресурсам на основе ролей пользователей (`Admin`, `Teacher`, `Student`) с помощью кастомных фильтров `IAuthorizationFilter` и протестировать защищённые HTTP-запросы.

---

## 🛠 Технологический стек
- **Платформа:** .NET 9.0 (C# 13)
- **Фреймворк:** ASP.NET Core Web API
- **Безопасность:** Кастомные атрибуты `[BasicAuthentication]` и `[MyAuthorize]`, `ClaimsIdentity`, `ClaimsPrincipal`
- **Документация и UI:** Swagger UI (Swashbuckle 6.6.2) со схемой `Basic`

---

## 📂 Структура проекта

```text
Web_service_development_modul_07-hw_07/
├── .gitignore
├── UniversityCoursesAPI.sln
├── README.md
├── DESIGN_AND_ANSWERS.md
└── src/
    └── UniversityCoursesAPI/
        ├── Attributes/
        │   ├── BasicAuthenticationAttribute.cs  # Извлечение заголовка, Base64-декодирование, проверка
        │   └── MyAuthorizeAttribute.cs          # Проверка User.IsInRole()
        ├── Controllers/
        │   └── CoursesController.cs             # CRUD курсов и публичный эндпоинт
        ├── Models/
        │   ├── Course.cs                        # Модель курса (Id, Title, Description, Teacher, Duration)
        │   └── User.cs                          # Модель пользователя (Id, UserName, Password, Email, Role)
        ├── Services/
        │   ├── ICourseService.cs / CourseService.cs # Сервис курсов (5 начальных дисциплин кафедры)
        │   └── IUserService.cs / UserService.cs     # Сервис проверки учетных данных
        ├── Program.cs                           # Регистрация DI сервисов и Swagger Basic Auth
        └── UniversityCoursesAPI.csproj
```

---

## 👥 Пользователи и матрица прав доступа

| Роль | Логин | Пароль | Разрешённые операции |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin` | `Admin123` | Полный доступ: просмотр, создание, изменение и удаление курсов |
| **Teacher** | `teacher` | `Teacher123` | Просмотр, создание и изменение информации о курсах |
| **Student** | `student` | `Student123` | Только просмотр учебных курсов |

---

## 📋 Эндпоинты API

| Метод | Маршрут | Разрешённые роли | Описание |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/courses` | `Admin`, `Teacher`, `Student` | Получение списка всех курсов |
| `GET` | `/api/courses/{id}` | `Admin`, `Teacher`, `Student` | Получение курса по ID |
| `POST` | `/api/courses` | `Admin`, `Teacher` | Создание нового курса |
| `PUT` | `/api/courses/{id}` | `Admin`, `Teacher` | Редактирование курса |
| `DELETE` | `/api/courses/{id}` | `Admin` *(только)* | Удаление курса |
| `GET` | `/api/courses/info` | **Все (анонимно)** | Публичная информация об учебной программе |

---

## 🚀 Быстрый запуск

### 1. Сборка проекта
```powershell
cd E:\Web_service_development_RK1\week_7\Web_service_development_modul_07-hw_07
dotnet build
```

### 2. Запуск приложения
```powershell
dotnet run --project src\UniversityCoursesAPI
```
После запуска откройте `http://localhost:5000/swagger` для интерактивного тестирования всех ролей через Swagger UI.
