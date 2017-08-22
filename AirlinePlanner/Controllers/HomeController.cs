using Microsoft.AspNetCore.Mvc;
using AirlinePlanner.Models;
using System.Collections.Generic;
using System;

namespace AirlinePlanner.Controllers
{
  public class HomeController : Controller
  {
    [HttpGet("/")]
    public ActionResult Index()
    {
      return View();
    }

    [HttpGet("/cities")]
    public ActionResult Cities()
    {
      List<City> allCities = City.GetAll();
      return View(allCities);
    }

    [HttpGet("/cities/new")]
    public ActionResult CityNew()
    {
      return View();
    }

    [HttpPost("/cities/add")]
    public ActionResult CityCreate()
    {
      City newCity = new City (Request.Form["city-name"]);
      newCity.Save();
      List<City> allCities = City.GetAll();
      return View("Cities", allCities);
    }

    [HttpGet("/flights")]
    public ActionResult Flights()
    {
      List<Flight> allFlights = Flight.GetAll();
      return View(allFlights);
    }

    [HttpGet("/flights/new")]
    public ActionResult FlightNew()
    {
      return View();
    }

    [HttpPost("/flights/add")]
    public ActionResult FlightCreate()
    {
      Flight newFlight = new Flight (DateTime.Parse(Request.Form["departure-time"]), Request.Form["departure-city"], Request.Form["arrival-city"], Request.Form["flight-status"]);
      newFlight.Save();
      List<Flight> allFlights = Flight.GetAll();
      return View("Flights", allFlights);
    }
  }
}
