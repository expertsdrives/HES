using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HESMDMS.Controllers
{
    public class PaymentDemoController : Controller
    {
        // GET: PaymentDemo
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Charge(string cardNumber, string expMonth, string expYear, string cvc)
        {
            // Retrieve Stripe API key from configuration
            var stripeApiKey = System.Configuration.ConfigurationManager.AppSettings["StripeApiKey"];

            // Set your secret key
            StripeConfiguration.ApiKey = stripeApiKey;

            // Create a token
            var tokenOptions = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = cardNumber,
                    ExpMonth = expMonth,
                    ExpYear = expYear,
                    Cvc = cvc
                }
            };

            var tokenService = new TokenService();
            Token stripeToken = tokenService.Create(tokenOptions);

            // Create a charge
            var chargeOptions = new ChargeCreateOptions
            {
                Amount = 1000, // amount in cents, change to your desired amount
                Currency = "usd",
                Description = "Example Charge",
                Source = stripeToken.Id
            };

            var chargeService = new ChargeService();
            Charge charge = chargeService.Create(chargeOptions);

            // Process the charge
            if (charge.Status == "succeeded")
            {
                // Payment success logic
                return Content("Payment successful!");
            }
            else
            {
                // Payment failure logic
                return Content("Payment failed.");
            }
        }
    }
}