using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MvcApp.TagHelpers
{
    [HtmlTargetElement("a", Attributes = ActionAttributeName)]
    [HtmlTargetElement("a", Attributes = ControllerAttributeName)]
    [HtmlTargetElement("a", Attributes = AreaAttributeName)]
    [HtmlTargetElement("a", Attributes = PageAttributeName)]
    [HtmlTargetElement("a", Attributes = PageHandlerAttributeName)]
    [HtmlTargetElement("a", Attributes = FragmentAttributeName)]
    [HtmlTargetElement("a", Attributes = HostAttributeName)]
    [HtmlTargetElement("a", Attributes = ProtocolAttributeName)]
    [HtmlTargetElement("a", Attributes = RouteAttributeName)]
    [HtmlTargetElement("a", Attributes = RouteValuesDictionaryName)]
    [HtmlTargetElement("a", Attributes = RouteValuesPrefix + "*")]
    public class AnchorTagHelper : Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper
    {
        private const string ActionAttributeName = "asp-action";
        private const string ControllerAttributeName = "asp-controller";
        private const string AreaAttributeName = "asp-area";
        private const string PageAttributeName = "asp-page";
        private const string PageHandlerAttributeName = "asp-page-handler";
        private const string FragmentAttributeName = "asp-fragment";
        private const string HostAttributeName = "asp-host";
        private const string ProtocolAttributeName = "asp-protocol";
        private const string RouteAttributeName = "asp-route";
        private const string RouteValuesDictionaryName = "asp-all-route-data";
        private const string RouteValuesPrefix = "asp-route-";
        private const string Href = "href";

        /// <summary>
        /// Creates a new <see cref="AnchorTagHelper"/>.
        /// </summary>
        /// <param name="generator">The <see cref="IHtmlGenerator"/>.</param>
        public AnchorTagHelper(IHtmlGenerator generator) 
            : base(generator)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!output.Attributes.ContainsName(Href) &&
                !context.AllAttributes.ContainsName(RouteValuesPrefix + "culture") &&
                !(RouteValues?.ContainsKey("culture") == true))
            {
                var routeValues = ViewContext?.RouteData?.Values;

                if (routeValues != null)
                {
                    var langValue = routeValues["culture"]?.ToString();

                    if (string.IsNullOrWhiteSpace(langValue))
                    {
                        
                        var rqf = ViewContext.HttpContext.Request.HttpContext.Features.Get<IRequestCultureFeature>();
                        langValue = rqf.RequestCulture.Culture.TwoLetterISOLanguageName.ToLower();
                    }

                    if (RouteValues == null)
                    {
                        RouteValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    }

                    RouteValues.Add("culture", langValue);
                }
            }

            base.Process(context, output);
        }
    }
}