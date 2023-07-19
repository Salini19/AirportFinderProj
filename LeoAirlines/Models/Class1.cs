using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeoAirlines.Models
{
    public class Class1
    {
    }
    public class Location
    {

        public double Latitude { get; set; }
        public double Longitude { get; set; }



        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }



    public class State
    {
        public string Sname { get; set; }
    }
    public class airinfo
    {
        public string IATACODE { get; set; }
        public string AIRPORTNAME { get; set; }
        public string CITY { get; set; }
        public double distance { get; set; }
    }






}