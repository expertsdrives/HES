using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using HESMDMS.Models;

namespace HESMDMS.Controllers
{
    public class tbl_RawDataAPIController : ApiController
    {
        private SmartMeterEntities db = new SmartMeterEntities();

        // GET: api/tbl_RawDataAPI
        public HttpResponseMessage Gettbl_RawDataAPI(string date)
        {
            List<tbl_RawDataAPI> employeeList = new List<tbl_RawDataAPI>();
           
            employeeList = db.tbl_RawDataAPI.Where(x=>x.ReceptionDate==date).OrderBy(a => a.VayudutName).ToList();
            HttpResponseMessage response;
            //response = Request.CreateResponse(HttpStatusCode.OK, employeeList);
            return Request.CreateResponse(HttpStatusCode.OK, employeeList, Configuration.Formatters.JsonFormatter);
        }
       
        //// GET: api/tbl_RawDataAPI/5
        //[ResponseType(typeof(tbl_RawDataAPI))]
        //public IHttpActionResult Gettbl_RawDataAPI(string date)
        //{
        //    tbl_RawDataAPI tbl_RawDataAPI = new tbl_RawDataAPI();
        //    var model = db.tbl_RawDataAPI.Where(x => x.ReceptionDate == date);
        //    //.tbl_RawDataAPI.Find(id);
        //    if (model == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(model);
        //}

        // PUT: api/tbl_RawDataAPI/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Puttbl_RawDataAPI(int id, tbl_RawDataAPI tbl_RawDataAPI)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tbl_RawDataAPI.ID)
            {
                return BadRequest();
            }

            db.Entry(tbl_RawDataAPI).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tbl_RawDataAPIExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/tbl_RawDataAPI
        [ResponseType(typeof(tbl_RawDataAPI))]
        public IHttpActionResult Posttbl_RawDataAPI(tbl_RawDataAPI tbl_RawDataAPI)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.tbl_RawDataAPI.Add(tbl_RawDataAPI);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = tbl_RawDataAPI.ID }, tbl_RawDataAPI);
        }

        // DELETE: api/tbl_RawDataAPI/5
        [ResponseType(typeof(tbl_RawDataAPI))]
        public IHttpActionResult Deletetbl_RawDataAPI(int id)
        {
            tbl_RawDataAPI tbl_RawDataAPI = db.tbl_RawDataAPI.Find(id);
            if (tbl_RawDataAPI == null)
            {
                return NotFound();
            }

            db.tbl_RawDataAPI.Remove(tbl_RawDataAPI);
            db.SaveChanges();

            return Ok(tbl_RawDataAPI);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tbl_RawDataAPIExists(int id)
        {
            return db.tbl_RawDataAPI.Count(e => e.ID == id) > 0;
        }
    }
}