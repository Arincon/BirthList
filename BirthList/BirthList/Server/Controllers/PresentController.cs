using BirthList.Shared;
using BirthList.Shared.Services;
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
        private readonly ITableService _presentService;
        private const string defaultPartitionKey = "1";

        public PresentController(ILogger<PresentController> logger, ITableService presentService)
        {
            _logger = logger;
            _presentService = presentService;
        }

        [HttpGet]
        [Route("List")]
        public async Task<IEnumerable<Present>> GetAllAsync()
        {
            var presents = await _presentService.GetAllPresentsPartition(defaultPartitionKey);
            return presents;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<Present> GetPresentAsync(string id)
        {
            var present = await _presentService.GetPresent(defaultPartitionKey, id);
            return present;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<bool> UpdatePresentAsync([FromBody] Present updatedPresent, string id)
        {
            updatedPresent.RemainingAmount = updatedPresent.RemainingAmount - updatedPresent.NewlyBought;
            var result = await _presentService.InsertOrMergePresentAsync(updatedPresent);
            return result != null;
        }
    }
}
