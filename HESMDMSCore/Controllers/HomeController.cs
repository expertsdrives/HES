using HESMDMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HESMDMS.Controllers
{
    public class HomeController : Controller
    {
        SmartMeterEntities clsMeter = new SmartMeterEntities();
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult CustomerRegistration()
        {
            if (Convert.ToString(Session["FullName"]) != "")
            {
                tbl_CustomerRegistration product = new tbl_CustomerRegistration();
                string num = Convert.ToString(Session["Bpnumber"]);
                var customerDetails = clsMeter.tbl_CustomerDetails.Where(x => x.SerialNumber == num).FirstOrDefault();
                product.BusinessPartnerNo = Convert.ToInt32(customerDetails.BusinessPartnerNo);
                product.FullName = customerDetails.FullName;
                product.Address = customerDetails.HouseNumber + ", " + customerDetails.Street + ", " + customerDetails.Street2 + ", " + customerDetails.Street3 + ", " + customerDetails.Street4 + ", " + customerDetails.Street5;
                //product.MobileNumber = customerDetails.MobileNumber;
                product.MeterNumber = Convert.ToInt32(customerDetails.SerialNumber);
                var serialNumber = clsMeter.tbl_SerialNumberMaster.Where(x => x.IsAssigned == false);
                ViewBag.serialNumbers = JsonConvert.SerializeObject(serialNumber);
                return View(product);
            }
            else
            {
                return RedirectToAction("../Login");
            }

        }
        public ActionResult SelectBusiness()
        {
            if (Convert.ToString(Session["FullName"]) != "")
            {
                var BPNo = from post in clsMeter.tbl_MeterMaster
                           join meta in clsMeter.tbl_CustomerDetails on post.Number equals meta.SerialNumber
                           where meta.Street == "Water Lily" || meta.Street== "Dreamland Appartment" || meta.Street == "Richmond Grand"
                           select new { ID = post.ID, Number = post.Number };

                //List<SelectListItem> items = new List<SelectListItem>();

                //foreach (var a in BPNo)
                //{
                //    items.Add(new SelectListItem
                //    { Text = a, Value = a });
                //}

                ViewBag.BP = JsonConvert.SerializeObject(BPNo);

                return View();
            }
            else
            {
                return RedirectToAction("../Login");
            }
        }
        public ActionResult SelectPOCLocation()
        {
            if (Convert.ToString(Session["FullName"]) != "")
            {
                //var BPNo = from post in clsMeter.tbl_MeterMaster
                //           join meta in clsMeter.tbl_CustomerDetails on post.Number equals meta.SerialNumber
                //           where meta.Street == "Water Lily" || meta.Street == "Dreamland Appartment" || meta.Street == "Richmond Grand"
                //           select new { ID = post.ID, Number = post.Number };

                List<SelectListItem> items = new List<SelectListItem>();

               
                    items.Add(new SelectListItem
                    { Text = "Water Lily", Value = "Water Lily" });
                items.Add(new SelectListItem
                { Text = "Richmond Grand", Value = "Richmond Grand" });items.Add(new SelectListItem
                { Text = "Dreamland Appartment", Value = "Dreamland Appartment" });


                ViewBag.BP = JsonConvert.SerializeObject(items);

                return View();
            }
            else
            {
                return RedirectToAction("../Login");
            }
        }
        public ActionResult SelectedBusiness()
        {
            string strDDLValue = Request.Form["BP"].ToString();
            Session.Add("Bpnumber", strDDLValue);
            return Redirect("CustomerRegistration");
        }
        public ActionResult Insert(tbl_CustomerRegistration collection, HttpPostedFileBase file)
        {
            if (collection.InstallationDate != null)
            {
                string ImageName = "";
                string newpat = "";
                if (file != null)
                {
                    ImageName = System.IO.Path.GetFileName(file.FileName);
                    string physicalPath = Server.MapPath("~/Images/" + ImageName);
                    newpat = Server.MapPath("~/Images/" + collection.BusinessPartnerNo + "_" + DateTime.Now.ToString("ddMMyyyyHHMMss") + "_" + ImageName);
                    file.SaveAs(physicalPath);
                    System.IO.File.Copy(physicalPath, newpat);
                    System.IO.File.Delete(physicalPath);
                    collection.Image = collection.BusinessPartnerNo + "_" + DateTime.Now.ToString("ddMMyyyyHHMMss") + "_" + ImageName;
                }
                else
                {
                    ImageName = "1.png";
                    collection.Image = "NoImage.png";
                }
                string slNumber = Convert.ToString(Session["snum"]);
                collection.SerialNumber = slNumber;
                collection.Status = "Pending";
                collection.CreatedBy = Convert.ToString(Session["FullName"]);
                
                var model = clsMeter.tbl_CustomerRegistration;
                model.Add(collection);
                clsMeter.SaveChanges();

                clsMeter.Database.ExecuteSqlCommand("UPDATE tbl_SerialNumberMaster SET IsAssigned = 1 WHERE [Serial Number] = '" + slNumber + "'");

                return View("Sucess");
            }
            else
            {
                return null;
            }
        }
        public string getTxid(string serialNumber)
        {
            string txid = "";
            var id = clsMeter.tbl_SerialNumberMaster.Where(x => x.Serial_Number == serialNumber).FirstOrDefault();
            Session.Add("snum", serialNumber);
            txid = id.TXID.ToString();
            return txid;
        }
        public ActionResult Sucess()
        {
            return View();
        }
    }

}