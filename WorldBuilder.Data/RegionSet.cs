using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorldBuilder.Data
{
    public class RegionSet
    {
        public Guid RegionSetId { get; set; }
        public string Name { get; set; }
        public Guid OwnerId { get; set; }
        [JsonIgnore]
        public List<Region> AllRegions { get; set; }
        [JsonIgnore]
        public List<Region> TopLevelRegions { get; set; }
        [JsonIgnore]
        public List<PointOfInterest> AllPointsOfInterest { get; set; }
        [JsonIgnore]
        public List<PointOfInterest> TopLevelPointsOfInterest { get; set; }
    }
}
