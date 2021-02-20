using System;

namespace BirthList.Shared
{
    public class Present
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int RequiredAmount { get; set; }
        public int RemainingAmount { get; set; }
        public Uri ImageLink { get; set; }
        public Uri SampleLink { get; set; }
    }
}
