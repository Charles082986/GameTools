using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorldBuilder.Data
{
    public class RegionSet
    {
        public string RegionSetId { get; set; }
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public string ImageURL { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public DateTime LastEdit { get; set; }
        [JsonIgnore]
        public List<Region> AllRegions { get; set; }
        [JsonIgnore]
        public List<Region> TopLevelRegions { get; set; }
        [JsonIgnore]
        public List<PointOfInterest> AllPointsOfInterest { get; set; }
        [JsonIgnore]
        public List<PointOfInterest> TopLevelPointsOfInterest { get; set; }
        [JsonIgnore]
        public List<PointOfInterest> OrphanedPointsOfInterest { get; set; }
        [JsonIgnore]
        public List<Region> OprhanedRegions { get; set; }
    }
}
