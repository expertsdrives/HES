using HESMDMS.Models;
using System.Web;

namespace HESMDMS.Services
{
    public interface IInvoiceService
    {
        void GenerateAndSaveInvoice(sp_GenerateInvoice_Result invoiceData, HttpServerUtilityBase server);
    }
}