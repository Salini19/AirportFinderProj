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
    public class cityinfoesController : ApiController
    {
        private AirportDBEntities db = new AirportDBEntities();

        // GET: api/cityinfoes
        public IQueryable<cityinfo> Getcityinfoes()
        {
            return db.cityinfoes;
        }

        // GET: api/cityinfoes/5
        [ResponseType(typeof(cityinfo))]
        public IHttpActionResult Getcityinfo(string id)
        {
            cityinfo cityinfo = db.cityinfoes.Find(id);
            if (cityinfo == null)
            {
                return NotFound();
            }

            return Ok(cityinfo);
        }

        // PUT: api/cityinfoes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putcityinfo(string id, cityinfo cityinfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cityinfo.CITY)
            {
                return BadRequest();
            }

            db.Entry(cityinfo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!cityinfoExists(id))
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

        // POST: api/cityinfoes
        [ResponseType(typeof(cityinfo))]
        public IHttpActionResult Postcityinfo(cityinfo cityinfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.cityinfoes.Add(cityinfo);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (cityinfoExists(cityinfo.CITY))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = cityinfo.CITY }, cityinfo);
        }

        // DELETE: api/cityinfoes/5
        [ResponseType(typeof(cityinfo))]
        public IHttpActionResult Deletecityinfo(string id)
        {
            cityinfo cityinfo = db.cityinfoes.Find(id);
            if (cityinfo == null)
            {
                return NotFound();
            }

            db.cityinfoes.Remove(cityinfo);
            db.SaveChanges();

            return Ok(cityinfo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool cityinfoExists(string id)
        {
            return db.cityinfoes.Count(e => e.CITY == id) > 0;
        }
    }
}