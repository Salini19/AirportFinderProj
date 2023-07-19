using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Services.Description;

using LeoAirlines.Models;
using Newtonsoft.Json;
using System.Text;

namespace LeoAirlines.Controllers
{
    public class airportinfoesController : Controller
    {
        public AirportDBEntities db = new AirportDBEntities();

        Uri baseAddress = new Uri("https://localhost:44358//api/");

        HttpClient client;

        public airportinfoesController()
        {
            client = new HttpClient();
            client.BaseAddress = baseAddress;
        }
       

        // GET: airportinfoes
        public ActionResult Index()
        {

            return View();
        }


        //AirportFinder

        public ActionResult Create()
        {
            var cityList = db.cityinfoes.ToList();
            ViewBag.source = new SelectList(cityList, "CITY", "CITY");

            ViewBag.destination = new SelectList(cityList, "CITY", "CITY");
            return View();
        }


        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            List<cityinfo> cityList = new List<cityinfo>();
      
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/cityinfoes").Result;
            if (response.IsSuccessStatusCode)
            {
                String Data = response.Content.ReadAsStringAsync().Result;
                cityList = JsonConvert.DeserializeObject<List<cityinfo>>(Data);
            }


            string From = form["source"].ToString();
            cityinfo city1 = cityList.Find(m => m.CITY == From);
            var startlocation = new Location(city1.LAT, city1.LONG);
            string To = form["destination"].ToString();
            cityinfo city2 = cityList.Find(m => m.CITY == To);



            var destinationlocation = new Location(city2.LAT, city2.LONG);
            if (From == To)
            {
                TempData["Error"] = "Source and destination cannot be same";
                return RedirectToAction("Create");
            }
            else
            {
                var airportsInRange = new List<airportinfo>(); /// airports between cities
                var airinrange = new List<airinfo>();
                List<airportinfo> airports = new List<airportinfo>();
                //var airports = db.airportinfoes.ToList();
                HttpResponseMessage response1 = client.GetAsync(client.BaseAddress + "/airportinfoes").Result;
                if (response.IsSuccessStatusCode)
                {
                    String Data = response1.Content.ReadAsStringAsync().Result;
                    airports = JsonConvert.DeserializeObject<List<airportinfo>>(Data);
                }



                //this is comment




                var maxDistance = HaversineDistance(startlocation, destinationlocation) + 50;
                foreach (var airport in airports)
                {



                    var airportLocation = new Location(airport.LAT, airport.LONG);
                    var distance = CalculateDistance(startlocation, destinationlocation, airportLocation);

                    var dist = HaversineDistance(startlocation, airportLocation);

                    if (distance <= maxDistance)
                    {
                        airportsInRange.Add(airport);
                        airinfo a = new airinfo();
                        a.IATACODE = airport.IATACODE;
                        a.CITY = airport.CITY;
                        a.AIRPORTNAME = airport.AIRPORTNAME;
                        a.distance = dist;
                        airinrange.Add(a);
                    }
                }
                airinrange = airinrange.OrderBy(a => a.distance).ToList();
                return View("AirportDisplay", airinrange);
            }

        }


        
       
        public ActionResult Contact()
        {
            

            return View();
        }
        [HttpPost]
        public ActionResult Contact(FormCollection form)
        {
            FeedBack feed = new FeedBack();
            feed.Name = form["Name"];
            feed.Email = form["Email"];
            feed.Subject = form["subject"];

            if (string.IsNullOrEmpty(feed.Name) || string.IsNullOrEmpty(feed.Email) || string.IsNullOrEmpty(feed.Subject))
            {
                TempData["Error"] = "Please fill in all the required fields.";
                return RedirectToAction("Contact");
            }


            string data = JsonConvert.SerializeObject(feed);

            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(client.BaseAddress + "/FeedBacks", content).Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Feedback submitted successfully.";
                return RedirectToAction("Contact");
            }
            else
            {
                TempData["Error"] = "An error occurred while submitting the feedback.";
                return RedirectToAction("Contact");
            }




        }
        public ActionResult cost()
        {


            return View();
        }
        [HttpPost]
        public ActionResult cost(FormCollection form)
        {

            List<airportinfo> airportlist = new List<airportinfo>();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/airportinfoes").Result;
            if (response.IsSuccessStatusCode)
            {
                String Data = response.Content.ReadAsStringAsync().Result;
                airportlist = JsonConvert.DeserializeObject<List<airportinfo>>(Data);
            }



            string From1 = form["From1"].ToString();

            airportinfo airport1 = airportlist.Find(m => m.AIRPORTNAME == From1);
            var start = new Location(airport1.LAT, airport1.LONG);
            string To1 = form["To1"].ToString();
            airportinfo airport2 = airportlist.Find(m => m.AIRPORTNAME == To1);



            var destination = new Location(airport2.LAT, airport2.LONG);



            var maxDistance = HaversineDistance(start, destination);
            var rph = 14.54;
            double price = rph * maxDistance;
            price = Math.Round(price, 4);
            var dist = Math.Round(maxDistance, 4);



            TempData["dist"] = $"The distance between {From1} and {To1} is {dist} Kms, Cost incurred is   ";
            TempData["Cost"] = $"INR(₹):{price}";




            return View("cost", new { From1 = From1, To1 = To1 });



        }



        public ActionResult StateWiseAirports()
        {
            List<StateImg> state = new List<StateImg>();
           
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/stateImgs").Result;
            if (response.IsSuccessStatusCode)
            {
                String Data = response.Content.ReadAsStringAsync().Result;
                state = JsonConvert.DeserializeObject<List<StateImg>>(Data);
            }
            return View(state);
        }
        [HttpPost]
        public ActionResult StateWiseAirports(string state)
        {


            List<StateImg> slist = new List<StateImg>();

            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/stateImgs").Result;
            if (response.IsSuccessStatusCode)
            {
                String Data = response.Content.ReadAsStringAsync().Result;
                slist = JsonConvert.DeserializeObject<List<StateImg>>(Data);
            }
            var StateList = slist.Where(x => x.State.ToLower().Contains(state.ToLower())).ToList();
            if (StateList.Count > 0)
            {
                return View(StateList);
            }
            return View();


        }



        public ActionResult AirportList(string id)
        {
       
            List<airportinfo> airports = new List<airportinfo>();
            //var airports = db.airportinfoes.ToList();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "/airportinfoes").Result;
            if (response.IsSuccessStatusCode)
            {
                String Data = response.Content.ReadAsStringAsync().Result;
                airports = JsonConvert.DeserializeObject<List<airportinfo>>(Data);
            }

            List<airportinfo> list = airports.FindAll(m => m.STATE == id);



            return View(list);



        }
        public double CalculateDistance(Location startLocation, Location destinationLocation, Location airportLocation)
        {
            var startToAirportDistance = HaversineDistance(startLocation, airportLocation);
            var airportToDestinationDistance = HaversineDistance(airportLocation, destinationLocation);
            var totalDistance = startToAirportDistance + airportToDestinationDistance;



            return totalDistance;
        }



        public double HaversineDistance(Location location1, Location location2)
        {


            var earthRadius = 6371; // Radius of the Earth in kilometers
            var latitudeDifference = DegreesToRadians(location2.Latitude - location1.Latitude);
            var longitudeDifference = DegreesToRadians(location2.Longitude - location1.Longitude);



            var a = Math.Sin(latitudeDifference / 2) * Math.Sin(latitudeDifference / 2) +
            Math.Cos(DegreesToRadians(location1.Latitude)) * Math.Cos(DegreesToRadians(location2.Latitude)) *
            Math.Sin(longitudeDifference / 2) * Math.Sin(longitudeDifference / 2);



            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));



            var distance = earthRadius * c;



            return distance;
        }



        public double DegreesToRadians(double degrees)
        {



            return degrees * (Math.PI / 180);
        }

        public ActionResult Services()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        //last
    }



}