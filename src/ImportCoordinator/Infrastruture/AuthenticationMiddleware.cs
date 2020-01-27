using System;
using System.Linq;
using System.Net;
using System.Security.Principal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ImportCoordinator.Core.Services;
using ImportCoordinator.Core;

namespace ImportCoordinator.Infrastruture
{
    public static class AuthenticationMiddleware
    {
        public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                var session = context.RequestServices.GetService<Session>();

                var request = context.Request;

                var serviceId = GetTokenFromHeaders(request);

                var loggedIn = session.LogIn(serviceId);

                if (loggedIn)
                {
                    context.User = new GenericPrincipal(new GenericIdentity(session.Identity), new string[0]);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                    return;
                }

                await next();
            });
        }

        public static string GetTokenFromHeaders(HttpRequest request)
        {
            var values = request.Headers[HeaderNames.Authorization];

            return values.Count == 0 ? null : (
                    from value in values
                    select value.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries)
                    into parts
                    where parts.Length == 2 && parts[0] == Settings.Instance.Security.AccessTokenType
                    select parts[1]
                )
                .FirstOrDefault();
        }

    }
}
