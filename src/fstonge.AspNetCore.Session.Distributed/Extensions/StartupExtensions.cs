using System;
using fstonge.AspNetCore.Session.Distributed.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.DependencyInjection;

namespace fstonge.AspNetCore.Session.Distributed.Extensions
{
    public static class SesssionAsyncExtensions
    {
        /// <summary>
        /// Have sessions be asyncronous. This adaptation is needed to force the session provider to use async calls instead of syncronous ones for session. 
        /// Someone surprisingly for something that seems common, Microsoft didn't make this aspect super nice.
        /// </summary>
        /// <param name="app">App builder instance.</param>
        /// <returns>App builder instance for chaining.</returns>
        /// <remarks>
        /// From Microsoft Documentation (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-6.0):
        /// The default session provider in ASP.NET Core will only load the session record from the underlying IDistributedCache store asynchronously if the
        /// ISession.LoadAsync method is explicitly called before calling the TryGetValue, Set or Remove methods. 
        /// Failure to call LoadAsync first will result in the underlying session record being loaded synchronously,
        /// which could potentially impact the ability of an application to scale.
        /// 
        /// See also:
        /// https://github.com/dotnet/aspnetcore/blob/d2a0cbc093e1e7bb3e38b55cd6043e4e2a0a2e9a/src/Middleware/Session/src/DistributedSession.cs#L268
        /// https://github.com/dotnet/AspNetCore.Docs/issues/1840#issuecomment-454182594
        /// https://bartwullems.blogspot.com/2019/12/aspnet-core-load-session-state.html
        /// </remarks>
        public static IApplicationBuilder UseAsyncDistributedSession(this IApplicationBuilder app)
        {
            app.UseSession();
            app.Use(async (context, next) =>
            {
                try
                {
                    await context.Session.LoadAsync();
                }
                catch
                {
                    // Prevent crash from dynamic controller when accessing the httpContext (see line 85)
                    // https://github.com/dotnet/aspnetcore/blob/main/src/Mvc/Mvc.RazorPages/src/Infrastructure/PageActionEndpointDataSource.cs
                }

                await next();
            });
            return app;
        }

        public static IServiceCollection AddAsyncDistributedSession(this IServiceCollection services, Action<SessionOptions> configure = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure != null)
            {
                services.Configure(configure);
            }

            services.AddTransient<ISessionStore, EnforcedAsyncDistributedSessionStore>();

            return services;
        }
    }
}