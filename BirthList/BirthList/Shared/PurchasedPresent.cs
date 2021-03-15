using Microsoft.Azure.Cosmos.Table;
using System;

namespace BirthList.Shared
{
    public class PurchasedPresent : TableEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int NewlyBought { get; set; }
        public string PurchaseInfo { get; set; }
    }
}
