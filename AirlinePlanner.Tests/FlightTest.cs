using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using AirlinePlanner.Models;

namespace AirlinePlanner.Tests
{

  [TestClass]
  public class FlightTests : IDisposable
  {
    public FlightTests()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=3306;database=airline_planner_test;";
    }

    public void Dispose()
    {
      Flight.DeleteAll();
      City.DeleteAll();
    }

    [TestMethod]
    public void GetAll_DatabaseEmptyAtFirst_0()
    {
      //Arrange, Act
      int result = Flight.GetAll().Count;

      //Assert
      Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Equals_ReturnsTrueIfDescriptionsAreTheSame_Flight()
    {
      //Arrange, Act
      Flight firstFlight = new Flight(new DateTime (2017, 9, 22, 10, 45, 0), "Seattle", "Portland", "Delayed");
      Flight secondFlight = new Flight(new DateTime (2017, 9, 22, 10, 45, 0), "Seattle", "Portland", "Delayed");

      //Assert
      Assert.AreEqual(firstFlight, secondFlight);
    }

    [TestMethod]
    public void Save_SavesToDatabase_FlightList()
    {
      //Arrange
      Flight testFlight = new Flight(new DateTime (2017, 9, 22, 10, 45, 0), "Seattle", "Portland", "Delayed");

      //Act
      testFlight.Save();
      List<Flight> result = Flight.GetAll();
      List<Flight> testList = new List<Flight>{testFlight};

      //Assert
      CollectionAssert.AreEqual(testList, result);
    }

    [TestMethod]
    public void Save_AssignsIdToObject_Id()
    {
      //Arrange
      Flight testFlight = new Flight(new DateTime (2017, 9, 22, 10, 45, 0), "Seattle", "Portland", "Delayed");

      //Act
      testFlight.Save();
      Flight savedFlight = Flight.GetAll()[0];

      int result = savedFlight.GetId();
      int testId = testFlight.GetId();

      //Assert
      Assert.AreEqual(testId, result);
    }

    [TestMethod]
    public void Find_FindsFlightInDatabase_Flight()
    {
      //Arrange
      Flight testFlight = new Flight(new DateTime (2017, 9, 22, 10, 45, 0), "Seattle", "Portland", "Delayed");
      testFlight.Save();

      //Act
      Flight foundFlight = Flight.Find(testFlight.GetId());

      //Assert
      Assert.AreEqual(testFlight, foundFlight);
    }

    [TestMethod]
    public void AddCity_AddsFlighttoCity_CityList()
    {
      //Arrange
      Flight testFlight = new Flight(new DateTime (2017, 9, 22, 10, 45, 0), "Seattle", "Portland", "Delayed");
      testFlight.Save();

      City testCity = new City("Seattle");
      testCity.Save();

      //Act
      testFlight.AddCity(testCity);

      List<City> result = testFlight.GetCities();
      List<City> testList = new List<City>{testCity};

      //Assert
      CollectionAssert.AreEqual(testList, result);
    }

    [TestMethod]
    public void GetCities_ReturnsAllFlightCities_CityList()
    {
      //Arrange
      Flight testFlight = new Flight(new DateTime (2017, 9, 22, 10, 45, 0), "Seattle", "Portland", "Delayed");
      testFlight.Save();

      City testCity1 = new City("Seattle");
      testCity1.Save();

      City testCity2 = new City("Portland");
      testCity2.Save();

      //Act
      testFlight.AddCity(testCity1);
      testFlight.AddCity(testCity2);
      List<City> result = testFlight.GetCities();
      List<City> testList = new List<City> {testCity1, testCity2};

      //Assert
      CollectionAssert.AreEqual(testList, result);
    }

    [TestMethod]
    public void Delete_DeletesFlightAssociationsFromDatabase_FlightList()
    {
      //Arrange
      City testCity = new City("Seattle");
      testCity.Save();

      Flight testFlight = new Flight(new DateTime (2017, 9, 22, 10, 45, 0), "Seattle", "Portland", "Delayed");
      testFlight.Save();

      //Act
      testFlight.AddCity(testCity);
      testFlight.Delete();

      List<Flight> resultCityFlights = testCity.GetFlights();
      List<Flight> testCityFlights = new List<Flight> {};

      //Assert
      CollectionAssert.AreEqual(testCityFlights, resultCityFlights);
    }

  }
}
