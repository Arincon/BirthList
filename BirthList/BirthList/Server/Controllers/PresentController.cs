using BirthList.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BirthList.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PresentController : ControllerBase
    {
        private readonly ILogger<PresentController> _logger;

        public PresentController(ILogger<PresentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Present> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 10).Select(index => new Present
            {
                Id = index,
                Title = $"Elemento {index}",
                Description = $"Descripción elemento {index}"
            })
            .ToArray();
        }
    }
}
