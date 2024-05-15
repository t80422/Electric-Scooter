using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Electric_Scooter.Helpers
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString DateInputFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var dateValue = metadata.Model as DateTime?;

            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            if (dateValue.HasValue)
            {
                attributes["value"] = dateValue.Value.ToString("yyyy-MM-dd");
            }

            // Ensure the input type is 'date'
            attributes["type"] = "date";

            return htmlHelper.TextBoxFor(expression, "{0:yyyy-MM-dd}", attributes);
        }
    }
}