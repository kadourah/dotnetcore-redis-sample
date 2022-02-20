using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using  Microsoft.Extensions.Caching.Distributed;

namespace dotnetcore_redis_sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
       // private IDistributedCache _cache;
        private IConnectionMultiplexer _conn;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConnectionMultiplexer conn)
        {
            _conn = conn;
            //_cache = cache;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }


        [HttpGet("FillCache")]
        public bool FillCache()
        {
            _logger.LogDebug("Log add fill cache");
            IDatabase db = _conn.GetDatabase();
            var rand = new Random();
           
            return db.StringSet("random-key", rand.Next(100).ToString());
        }

        [HttpGet("GetCache")]
        public string GetCache()
        {
            _logger.LogDebug("Log add get cache");
            IDatabase db = _conn.GetDatabase();
            string redisValue = db.StringGet("random-key");
            return redisValue;

        }

        [HttpGet("CleakKey")]
        public bool CleakKey()
        {
            _logger.LogDebug("clear cache");
            IDatabase db = _conn.GetDatabase();
           return db.StringSet("random-key", "");
            

        }
    }
}
