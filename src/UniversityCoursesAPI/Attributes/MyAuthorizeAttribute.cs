using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UniversityCoursesAPI.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class MyAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles;

    public MyAuthorizeAttribute(params string[] roles)
    {
        _roles = roles ?? Array.Empty<string>();
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // 1. Проверка аутентификации
        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                statusCode = StatusCodes.Status401Unauthorized,
                error = "Unauthorized",
                message = "Пользователь не аутентифицирован. Требуются учетные данные Basic Auth."
            });
            return;
        }

        // 2. Если конкретные роли не требуются
        if (_roles.Length == 0)
        {
            return;
        }

        // 3. Проверка ролей через User.IsInRole()
        var hasRole = _roles.Any(role => user.IsInRole(role));

        if (!hasRole)
        {
            // 4. Возврат HTTP 403 Forbidden при недостатке прав
            context.Result = new ObjectResult(new
            {
                statusCode = StatusCodes.Status403Forbidden,
                error = "Forbidden",
                message = $"Доступ запрещен. Необходима роль из списка: {string.Join(", ", _roles)}.",
                currentRole = user.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
