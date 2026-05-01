using AdventureWorks.Server.Core;
using AdventureWorks.Server.Data;
using Microsoft.AspNetCore.Mvc;

namespace AdventureWorks.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WeatherForecastController(ApplicationDbContext context)
        {
            _context = context;
        }
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("businessentity")]
        public IActionResult GetBusinessEntities()
        {
            try
            {
                var entities = _context.Person.ToList();
                return Ok(entities);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("businessentity")]
        public IActionResult AddBusinessEntity(BusinessEntity entity, IConfiguration config)
        {
            try
            {
                _context.Person.Add(entity);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("businessentity/{id}")]
        public IActionResult DeleteBusinessEntity(int id)
        {
            try
            {
                var entity = _context.Person.FirstOrDefault(e => e.BusinessEntityID == id);
                if (entity == null)
                {
                    return NotFound();
                }

                _context.Person.Remove(entity);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
