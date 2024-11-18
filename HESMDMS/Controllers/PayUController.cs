using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Controllers
{
    public class PayUController : Controller
    {
        private string upiId = "krutishruparelia@okhdfcbank";
        // GET: PayU
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PaymentSuccess()
        {
            var form= Request.Form.ToString();
            return View();
        }
        [HttpPost]
        public ActionResult GenerateQRCode(string amount)
        {
            if (decimal.TryParse(amount, out decimal amountValue) && amountValue > 0)
            {
                // Create UPI payment URL
                string paymentUrl = $"upi://pay?pa={upiId}&pn=Krutish Ruparelia&am={amount}&cu=INR";

                // Generate QR code
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(paymentUrl, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                // Convert QR code Bitmap to Base64 string
                using (MemoryStream ms = new MemoryStream())
                {
                    qrCodeImage.Save(ms, ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    string base64String = Convert.ToBase64String(byteImage);

                    // Pass the Base64 string to the View
                    ViewBag.QRCodeImage = "data:image/png;base64," + base64String;
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Please enter a valid amount.";
            }

            return View("Index");
        }
    }
}