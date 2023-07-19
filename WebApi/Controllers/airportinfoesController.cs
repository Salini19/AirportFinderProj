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
using WebApi.Models;

namespace WebApi.Controllers
{
    public class airportinfoesController : ApiController
    {
        private AirportDBEntities db = new AirportDBEntities();

        // GET: api/airportinfoes
        public IQueryable<airportinfo> Getcityinfoes()
        {
            return db.airportinfoes;
        }

        // GET: api/airportinfoes/5
        [ResponseType(typeof(airportinfo))]
        public IHttpActionResult Getcityinfo(string id)
        {
            airportinfo airportinfo = db.airportinfoes.Find(id);
            if (airportinfo == null)
            {
                return NotFound();
            }

            return Ok(airportinfo);
        }


        // PUT: api/airportinfoes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putairportinfo(string id, airportinfo airportinfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != airportinfo.IATACODE)
            {
                return BadRequest();
            }

            db.Entry(airportinfo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!airportinfoExists(id))
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

        // POST: api/airportinfoes
        [ResponseType(typeof(airportinfo))]
        public IHttpActionResult Postairportinfo(airportinfo airportinfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.airportinfoes.Add(airportinfo);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (airportinfoExists(airportinfo.IATACODE))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = airportinfo.IATACODE }, airportinfo);
        }

        // DELETE: api/airportinfoes/5
        [ResponseType(typeof(airportinfo))]
        public IHttpActionResult Deleteairportinfo(string id)
        {
            airportinfo airportinfo = db.airportinfoes.Find(id);
            if (airportinfo == null)
            {
                return NotFound();
            }

            db.airportinfoes.Remove(airportinfo);
            db.SaveChanges();

            return Ok(airportinfo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool airportinfoExists(string id)
        {
            return db.airportinfoes.Count(e => e.IATACODE == id) > 0;
        }
    }
}