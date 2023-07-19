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
    public class StateImgsController : ApiController
    {
        private AirportDBEntities db = new AirportDBEntities();

        // GET: api/StateImgs
        public IQueryable<StateImg> GetStateImgs()
        {
            return db.StateImgs;
        }

        // GET: api/StateImgs/5
        [ResponseType(typeof(StateImg))]
        public IHttpActionResult GetStateImg(string id)
        {
            StateImg stateImg = db.StateImgs.Find(id);
            if (stateImg == null)
            {
                return NotFound();
            }

            return Ok(stateImg);
        }

        // PUT: api/StateImgs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutStateImg(string id, StateImg stateImg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != stateImg.State)
            {
                return BadRequest();
            }

            db.Entry(stateImg).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StateImgExists(id))
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

        // POST: api/StateImgs
        [ResponseType(typeof(StateImg))]
        public IHttpActionResult PostStateImg(StateImg stateImg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.StateImgs.Add(stateImg);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (StateImgExists(stateImg.State))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = stateImg.State }, stateImg);
        }

        // DELETE: api/StateImgs/5
        [ResponseType(typeof(StateImg))]
        public IHttpActionResult DeleteStateImg(string id)
        {
            StateImg stateImg = db.StateImgs.Find(id);
            if (stateImg == null)
            {
                return NotFound();
            }

            db.StateImgs.Remove(stateImg);
            db.SaveChanges();

            return Ok(stateImg);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StateImgExists(string id)
        {
            return db.StateImgs.Count(e => e.State == id) > 0;
        }
    }
}