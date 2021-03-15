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
        private readonly IMailService _mailService;
        private const string defaultPartitionKey = "1";

        public PresentController(ILogger<PresentController> logger, ITableService presentService, IMailService mailService)
        {
            _logger = logger;
            _presentService = presentService;
            _mailService = mailService;
        }

        [HttpGet]
        [Route("List")]
        public async Task<IEnumerable<WishlistPresent>> GetAllAsync()
        {
            var presents = await _presentService.GetAllPresentsPartition(defaultPartitionKey);
            presents = presents.Select(pres => CalculateValues(pres).Result).OrderByDescending(p => p.RemainingAmount).ToList();
            return presents;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<WishlistPresent> GetPresentAsync(string id)
        {
            var present = await _presentService.GetPresent(defaultPartitionKey, id);
            present = await CalculateValues(present);
            return present;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<bool> UpdatePresentAsync([FromBody] WishlistPresent updatedPresent, string id)
        {
            var purchasedPresent = updatedPresent.Purchase();
            //updatedPresent.RemainingAmount = updatedPresent.RemainingAmount - updatedPresent.NewlyBought;
            var result = await _presentService.InsertOrMergePresentAsync(purchasedPresent);
            var successEmail = await _mailService.SendEmail(purchasedPresent);
            return result != null;
        }
    
        public async Task<WishlistPresent> CalculateValues(WishlistPresent present)
        {
            present.RemainingAmount = present.RequiredAmount;
            var purchasedPresents = await _presentService.GetAllPurchasesPartition(present.PartitionKey + present.RowKey);
            if (purchasedPresents.Any())
            {
                var purchasedSum = purchasedPresents.Sum(pp => pp.NewlyBought);
                present.RemainingAmount = present.RequiredAmount - purchasedSum >= 0 ? present.RequiredAmount - purchasedSum : 0;
            }
            return present;
        }
    }
}
