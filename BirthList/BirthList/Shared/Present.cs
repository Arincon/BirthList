using Microsoft.Azure.Cosmos.Table;
using System;

namespace BirthList.Shared
{
    public class Present : TableEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int RequiredAmount { get; set; }
        public int RemainingAmount { get; set; }
        public string ImageLink { get; set; }
        public string SampleLink { get; set; }
        public double EstimatedPrice { get; set; }

        [IgnoreProperty]
        public int NewlyBought { get; set; }
    }
}
