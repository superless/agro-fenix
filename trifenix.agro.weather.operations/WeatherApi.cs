﻿using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using trifenix.agro.weather.interfaces;
using trifenix.agro.weather.model;

namespace trifenix.agro.weather.operations {
    public class WeatherApi : IWeatherApi {

        private readonly string _appId;

        public WeatherApi(string appId) {
            _appId = appId;
        }

        public async Task<Weather> GetWeather(float lat, float lon) {
            HttpClient client = new HttpClient();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.openweathermap.org/data/2.5/weather?lat=" + lat + "&lon=" + lon + "&appid=" + _appId);
            var response = await client.SendAsync(requestMessage);
            client.Dispose();
            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(responseBody);
            string cityName = (string)json.name;
            string main = (string)json.weather[0].main;
            string desc = (string)json.weather[0].description;
            float temp = (float)json.main.temp;
            float speed = (float)json.wind.speed;
            int degree = (int)json.wind.degree;
            int cloud = (int)json.clouds;
            int hum = (int)json.main.humidity;
            int pressure = (int)json.main.pressure;
            string iconCode = (string)json.weather[0].icon;
            return new Weather(cityName, lat, lon, main, desc, temp, speed, degree, cloud, hum, pressure, iconCode);
        }

    }
}