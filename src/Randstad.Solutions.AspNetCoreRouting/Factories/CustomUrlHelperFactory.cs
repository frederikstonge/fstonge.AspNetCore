using System;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Randstad.Solutions.AspNetCoreRouting.Helpers;

namespace Randstad.Solutions.AspNetCoreRouting.Factories
{
    public class CustomUrlHelperFactory : IUrlHelperFactory
    {
        public IUrlHelper GetUrlHelper(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var httpContext = context.HttpContext;

            if (httpContext == null)
            {
                throw new ArgumentException(nameof(context.HttpContext));
            }

            if (httpContext.Items == null)
            {
                throw new ArgumentException(nameof(context.HttpContext.Items));
            }

            // Perf: Create only one UrlHelper per context
            if (httpContext.Items.TryGetValue(typeof(IUrlHelper), out var value) && value is IUrlHelper)
            {
                return (IUrlHelper)value;
            }

            IUrlHelper urlHelper;
            var endpointFeature = httpContext.Features.Get<IEndpointFeature>();
            if (endpointFeature?.Endpoint != null)
            {
                var services = httpContext.RequestServices;
                var linkGenerator = services.GetRequiredService<LinkGenerator>();

                urlHelper = new CustomEndpointRoutingUrlHelper(context, linkGenerator);
            }
            else
            {
                urlHelper = new UrlHelper(context);
            }

            httpContext.Items[typeof(IUrlHelper)] = urlHelper;

            return urlHelper;
        }
    }
}