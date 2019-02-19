using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorldBuilder.Data
{
    public class PointOfInterest
    {
        public string PointOfInterestId { get; set; }
        public string RegionSetId { get; set; }
        [JsonIgnore]
        public List<Region> Regions { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
    }

    public class PointOfInterestComparer : IEqualityComparer<PointOfInterest>
    {
        public bool Equals(PointOfInterest x, PointOfInterest y)
        {
            if(x.PointOfInterestId == null && y.PointOfInterestId == null)
            {
                return x.Name == y.Name && x.RegionSetId == y.RegionSetId;
            }
            else
            {
                return x.PointOfInterestId == y.PointOfInterestId;
            }
        }

        public int GetHashCode(PointOfInterest obj)
        {
            return obj.GetHashCode();
        }
    }
}
