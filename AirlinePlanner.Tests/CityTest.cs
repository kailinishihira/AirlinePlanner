using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using AirlinePlanner.Models;

namespace AirlinePlanner.Tests
{
  [TestClass]
  public class CityTests : IDisposable
  {
    public CityTests()
    {
        DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=3306;database=airline_planner_test;";
    }

    public void Dispose()
    {
      Flight.DeleteAll();
      City.DeleteAll();
    }

    [TestMethod]
    public void GetAll_CitiesEmptyAtFirst_0()
    {
      //Arrange, Act
      int result = City.GetAll().Count;

      //Assert
      Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Equals_ReturnsTrueForSameName_City()
    {
      //Arrange, Act
      City firstCity = new City("Seattle");
      City secondCity = new City("Seattle");

      //Assert
      Assert.AreEqual(firstCity, secondCity);
    }

    [TestMethod]
    public void Save_SavesCityToDatabase_CityList()
    {
      //Arrange
      City testCity = new City("Seattle");
      testCity.Save();

      //Act
      List<City> result = City.GetAll();
      List<City> testList = new List<City>{testCity};

      //Assert
      CollectionAssert.AreEqual(testList, result);
    }

    [TestMethod]
    public void Save_DatabaseAssignsIdToCity_Id()
    {
      //Arrange
      City testCity = new City("Seattle");
      testCity.Save();

      //Act
      City savedCity = City.GetAll()[0];

      int result = savedCity.GetId();
      int testId = testCity.GetId();

      //Assert
      Assert.AreEqual(testId, result);
    }

    [TestMethod]
    public void Find_FindsCityInDatabase_City()
    {
      //Arrange
      City testCity = new City("Seattle");
      testCity.Save();

      //Act
      City foundCity = City.Find(testCity.GetId());

      //Assert
      Assert.AreEqual(testCity, foundCity);
    }

    [TestMethod]
    public void Delete_DeletesCityFromDatabase_CityList()
    {
      //Arrange
      string city1 = "Seattle";
      City testCity1 = new City(city1);
      testCity1.Save();

      string city2 = "Honolulu";
      City testCity2 = new City(city2);
      testCity2.Save();

      //Act
      testCity1.Delete();
      List<City> resultCities = City.GetAll();
      List<City> testCityList = new List<City> {testCity2};

      //Assert
      CollectionAssert.AreEqual(testCityList, resultCities);
    }

    [TestMethod]
    public void Test_AddFlight_AddsFlightToCity()
    {
      //Arrange
      City testCity = new City("Seattle");
      testCity.Save();

      Flight testFlight1 = new Flight(new DateTime (2017, 9, 22, 10, 45, 0), "Seattle", "Portland", "Delayed");
      testFlight1.Save();

      Flight testFlight2 = new Flight(new DateTime (2017, 9, 20, 9, 30, 0), "Seattle", "Honolulu", "On-time");
      testFlight2.Save();

      //Act
      testCity.AddFlight(testFlight1);
      testCity.AddFlight(testFlight2);

      List<Flight> result = testCity.GetFlights();
      List<Flight> testList = new List<Flight>{testFlight1, testFlight2};

      //Assert
      CollectionAssert.AreEqual(testList, result);
    }

    [TestMethod]
    public void GetFlights_ReturnsAllCityFlights_FlightList()
    {
      //Arrange
      City testCity = new City("Seattle");
      testCity.Save();

      Flight testFlight1 = new Flight(new DateTime (2017, 9, 22, 10, 45, 0), "Seattle", "Portland", "Delayed");
      testFlight1.Save();

      Flight testFlight2 = new Flight(new DateTime (2017, 9, 20, 9, 30, 0), "Seattle", "Honolulu", "On-time");
      testFlight2.Save();

      //Act
      testCity.AddFlight(testFlight1);
      List<Flight> savedFlights = testCity.GetFlights();
      List<Flight> testList = new List<Flight> {testFlight1};

      //Assert
      CollectionAssert.AreEqual(testList, savedFlights);
    }

    [TestMethod]
    public void Delete_DeletesCityAssociationsFromDatabase_CityList()
    {
      //Arrange
      Flight testFlight = new Flight(new DateTime (2017, 9, 22, 10, 45, 0), "Seattle", "Portland", "Delayed");
      testFlight.Save();

      string newCity = "Seattle";
      City testCity = new City(newCity);
      testCity.Save();

      //Act
      testCity.AddFlight(testFlight);
      testCity.Delete();

      List<City> resultFlightCategories = testFlight.GetCities();
      List<City> testFlightCategories = new List<City> {};

      //Assert
      CollectionAssert.AreEqual(testFlightCategories, resultFlightCategories);
    }

  }
}
