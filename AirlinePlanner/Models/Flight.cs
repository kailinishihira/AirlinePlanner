using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace AirlinePlanner.Models
{
  public class Flight
  {
    private int _id;
    private DateTime _departureTime;
    private string _departureCity;
    private string _arrivalCity;
    private string _status;

    public Flight(DateTime departureTime, string departureCity, string arrivalCity, string status, int Id = 0)
    {
      _id = Id;
      _departureTime = departureTime;
      _departureCity = departureCity;
      _arrivalCity = arrivalCity;
      _status = status;
    }

    public int GetId()
    {
      return _id;
    }

    public DateTime GetDepartureTime()
    {
      return _departureTime;
    }

    public string GetDepartureCity()
    {
      return _departureCity;
    }

    public string GetArrivalCity()
    {
      return _arrivalCity;
    }

    public string GetStatus()
    {
      return _status;
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM flights;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static List<Flight> GetAll()
    {
      List<Flight> allFlights = new List<Flight> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM flights;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int flightId = rdr.GetInt32(0);
        DateTime departureTime = rdr.GetDateTime(1);
        string departureCity = rdr.GetString(2);
        string arrivalCity = rdr.GetString(3);
        string status = rdr.GetString(4);
        Flight newFlight = new Flight(departureTime, departureCity, arrivalCity, status, flightId);
        allFlights.Add(newFlight);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allFlights;
    }

    public override bool Equals(System.Object otherFlight)
    {
      if (!(otherFlight is Flight))
      {
        return false;
      }
      else
      {
        Flight newFlight = (Flight) otherFlight;
        bool idEquality = (this.GetId() == newFlight.GetId());
        bool departureTimeEquality = (this.GetDepartureTime() == newFlight.GetDepartureTime());
        bool departureCityEquality = (this.GetDepartureCity() == newFlight.GetDepartureCity());
        bool arrivalCityEquality = (this.GetArrivalCity() == newFlight.GetArrivalCity());
        bool statusEquality = (this.GetStatus() == newFlight.GetStatus());
        return (idEquality && departureTimeEquality && departureCityEquality && arrivalCityEquality && statusEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetId().GetHashCode();
    }

    public void Save()
     {
       MySqlConnection conn = DB.Connection();
       conn.Open();

       var cmd = conn.CreateCommand() as MySqlCommand;
       cmd.CommandText = @"INSERT INTO flights (departure_time, departure_city, arrival_city, status) VALUES (@departureTime, @departureCity, @arrivalCity, @status);";

       MySqlParameter departureTime = new MySqlParameter();
       departureTime.ParameterName = "@departureTime";
       departureTime.Value = this._departureTime;
       cmd.Parameters.Add(departureTime);

       MySqlParameter departureCity = new MySqlParameter();
       departureCity.ParameterName = "@departureCity";
       departureCity.Value = this._departureCity;
       cmd.Parameters.Add(departureCity);

       MySqlParameter arrivalCity = new MySqlParameter();
       arrivalCity.ParameterName = "@arrivalCity";
       arrivalCity.Value = this._arrivalCity;
       cmd.Parameters.Add(arrivalCity);

       MySqlParameter status = new MySqlParameter();
       status.ParameterName = "@status";
       status.Value = this._status;
       cmd.Parameters.Add(status);

       cmd.ExecuteNonQuery();
       _id = (int) cmd.LastInsertedId;
       conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
     }

    public static Flight Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM flights WHERE id = @thisId;";

      MySqlParameter thisId = new MySqlParameter();
      thisId.ParameterName = "@thisId";
      thisId.Value = id;
      cmd.Parameters.Add(thisId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      int flightId = 0;
      DateTime departureTime = DateTime.MinValue;
      string departureCity = "";
      string arrivalCity = "";
      string status = "";

      while (rdr.Read())
      {
        flightId = rdr.GetInt32(0);
        departureTime = rdr.GetDateTime(1);
        departureCity = rdr.GetString(2);
        arrivalCity = rdr.GetString(3);
        status = rdr.GetString(4);
      }
      Flight foundFlight= new Flight(departureTime, departureCity, arrivalCity, status, flightId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return foundFlight;
    }

    public void AddCity(City newCity)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO cities_flights (city_id, flight_id) VALUES (@CityId, @FlightId);";

      MySqlParameter city_id = new MySqlParameter();
      city_id.ParameterName = "@CityId";
      city_id.Value = newCity.GetId();
      cmd.Parameters.Add(city_id);

      MySqlParameter flight_id = new MySqlParameter();
      flight_id.ParameterName = "@FlightId";
      flight_id.Value = _id;
      cmd.Parameters.Add(flight_id);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
    }

    public List<City> GetCities()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT city_id FROM cities_flights WHERE flight_id = @flightId;";

      MySqlParameter flightIdParameter = new MySqlParameter();
      flightIdParameter.ParameterName = "@flightId";
      flightIdParameter.Value = _id;
      cmd.Parameters.Add(flightIdParameter);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      List<int> cityIds = new List<int> {};
      while(rdr.Read())
      {
        int cityId = rdr.GetInt32(0);
        cityIds.Add(cityId);
      }
      rdr.Dispose();

      List<City> allCities = new List<City> {};
      foreach (int cityId in cityIds)
      {
        var cityQuery = conn.CreateCommand() as MySqlCommand;
        cityQuery.CommandText = @"SELECT * FROM cities WHERE id = @CityId;";

        MySqlParameter cityIdParameter = new MySqlParameter();
        cityIdParameter.ParameterName = "@CityId";
        cityIdParameter.Value = cityId;
        cityQuery.Parameters.Add(cityIdParameter);

        var cityQueryRdr = cityQuery.ExecuteReader() as MySqlDataReader;
        while(cityQueryRdr.Read())
        {
          int thisCityId = cityQueryRdr.GetInt32(0);
          string cityName = cityQueryRdr.GetString(1);
          City foundCity = new City(cityName, thisCityId);
          allCities.Add(foundCity);
        }
        cityQueryRdr.Dispose();
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allCities;
    }

    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = new MySqlCommand("DELETE FROM flights WHERE id = @FlightId; DELETE FROM cities_flights WHERE flight_id = @FlightId;", conn);
      MySqlParameter flightIdParameter = new MySqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = this.GetId();

      cmd.Parameters.Add(flightIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

  }
}
