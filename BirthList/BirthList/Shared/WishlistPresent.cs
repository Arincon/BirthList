using Microsoft.Azure.Cosmos.Table;
using System;

namespace BirthList.Shared
{
    public class WishlistPresent : TableEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int RequiredAmount { get; set; }
        public string ImageLink { get; set; }
        public string SampleLink { get; set; }
        public double EstimatedPrice { get; set; }

        [IgnoreProperty]
        public int NewlyBought { get; set; }
        [IgnoreProperty]
        public int RemainingAmount { get; set; }
        [IgnoreProperty]
        public string PurchaseInfo { get; set; }

        public PurchasedPresent Purchase()
        {
            var pPresent = new PurchasedPresent();
            pPresent.Title = this.Title;
            pPresent.PartitionKey = this.PartitionKey + this.RowKey;
            pPresent.RowKey = DateTime.Now.Ticks.ToString();
            pPresent.Description = this.Description;
            pPresent.NewlyBought = this.NewlyBought;
            pPresent.PurchaseInfo = this.PurchaseInfo;
            return pPresent;
        }
    }
}
