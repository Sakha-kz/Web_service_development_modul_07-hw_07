using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UniversityCoursesAPI.Services;

namespace UniversityCoursesAPI.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class BasicAuthenticationAttribute : Attribute, IAuthorizationFilter
{
    private const string Realm = "UniversityCoursesAPI";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Пропуск проверки для эндпоинтов, помеченных [AllowAnonymous]
        if (context.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any())
        {
            return;
        }

        // 1. Получение заголовка Authorization
        var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(authHeader))
        {
            ReturnUnauthorized(context, "Заголовок Authorization отсутствует.");
            return;
        }

        // 2. Проверка схемы Basic
        if (!AuthenticationHeaderValue.TryParse(authHeader, out var headerValue) ||
            !string.Equals(headerValue.Scheme, "Basic", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(headerValue.Parameter))
        {
            ReturnUnauthorized(context, "Используйте базовую схему аутентификации: Authorization: Basic <base64_credentials>");
            return;
        }

        string credentials;
        try
        {
            // 3. Декодирование Base64 с обработкой исключения некорректного формата
            var credentialBytes = Convert.FromBase64String(headerValue.Parameter);
            credentials = Encoding.UTF8.GetString(credentialBytes);
        }
        catch (FormatException)
        {
            ReturnUnauthorized(context, "Некорректная строка Base64.");
            return;
        }

        // 4. Извлечение логина и пароля
        var separatorIndex = credentials.IndexOf(':');
        if (separatorIndex == -1)
        {
            ReturnUnauthorized(context, "Неверный формат учетных данных (ожидается: username:password).");
            return;
        }

        var username = credentials.Substring(0, separatorIndex);
        var password = credentials.Substring(separatorIndex + 1);

        // 5. Проверка учетных данных через UserService
        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
        var user = userService.ValidateUser(username, password);

        if (user == null)
        {
            ReturnUnauthorized(context, "Неверный логин или пароль.");
            return;
        }

        // 6. Создание ClaimsIdentity и ClaimsPrincipal
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, "Basic");
        var principal = new ClaimsPrincipal(identity);

        // 7. Сохранение в HttpContext.User
        context.HttpContext.User = principal;
    }

    private static void ReturnUnauthorized(AuthorizationFilterContext context, string message)
    {
        context.HttpContext.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Realm}\"";
        context.Result = new UnauthorizedObjectResult(new
        {
            statusCode = StatusCodes.Status401Unauthorized,
            error = "Unauthorized",
            message = message
        });
    }
}
