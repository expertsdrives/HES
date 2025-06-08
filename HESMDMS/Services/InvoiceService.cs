using HESMDMS.Models;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System.IO;
using System.Web;

namespace HESMDMS.Services
{
    public class InvoiceService : IInvoiceService
    {
        public void GenerateAndSaveInvoice(sp_GenerateInvoice_Result invoiceData, HttpServerUtilityBase server)
        {
            // It's better to use a robust templating engine like Razor (e.g., using RazorEngine)
            // to generate HTML. For this example, we'll stick to simple string replacement.

            var templatePath = server.MapPath("~/Invoice/Index.html");
            if (!File.Exists(templatePath))
            {
                // Handle template not found error
                throw new FileNotFoundException("Invoice template not found.", templatePath);
            }

            string htmlContent = File.ReadAllText(templatePath);

            // Replace placeholders with actual data
            htmlContent = htmlContent.Replace("##CUSTOMER_NAME##", invoiceData.FullName);
            htmlContent = htmlContent.Replace("##CUSTOMER_ID##", invoiceData.CustomerID);
            // ... add more replacements for other data points in the invoice.

            // Ensure the output directory exists
            var outputDirectory = server.MapPath("~/Invoice/Generated");
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Generate a unique file name to avoid conflicts
            var pdfPath = Path.Combine(outputDirectory, $"Invoice_{invoiceData.CustomerID}_{invoiceData.MonthOfSales}.pdf");

            // Convert HTML to PDF
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
            PdfDocument document = htmlConverter.Convert(htmlContent, Path.GetDirectoryName(templatePath));

            // Save the document
            document.Save(pdfPath);
            document.Close(true);
        }
    }
}