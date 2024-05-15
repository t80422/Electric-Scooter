using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout.Font;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Electric_Scooter.Helpers
{
    public static class PDFHelper
    {
        /// <summary>
        /// 生成PDF
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static byte[] GeneratePdfFromHtml(ControllerContext controllerContext, string viewName, object model)
        {
            string htmlString = RenderRazorViewToString(controllerContext, viewName, model);

            // 加載字體
            var fontProvider = new FontProvider();
            var fontPath = HttpContext.Current.Server.MapPath("~/fonts/msjh.ttc,0");
            fontProvider.AddFont(fontPath);
            var converterProperties = new ConverterProperties();
            converterProperties.SetFontProvider(fontProvider);

            // 生成PDF
            using (var ms = new MemoryStream())
            {
                var writer = new PdfWriter(ms);
                var pdfDocument = new PdfDocument(writer);
                pdfDocument.SetDefaultPageSize(iText.Kernel.Geom.PageSize.A4.Rotate()); // 將頁面設置為橫向

                HtmlConverter.ConvertToPdf(htmlString, pdfDocument, converterProperties);
                return ms.ToArray();
            }
        }

        // 渲染Razor視圖為HTML字符串的輔助方法
        private static string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model)
        {
            var viewData = new ViewDataDictionary(model);

            using (StringWriter sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(controllerContext, viewName, null);
                var viewContext = new ViewContext(controllerContext, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}