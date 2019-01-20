using Neo4j.Driver.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WorldBuilder.Data
{
    public class Region
    {
        public Guid RegionId { get; set; }
        public Guid RegionSetId { get; set; }
        [JsonIgnore]
        public List<Region> InnerRegions { get; set; }
        [JsonIgnore]
        public List<Region> ParentRegions { get; set; }
        [JsonIgnore]
        public List<PointOfInterest> PointsOfInterest { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }

    public class RegionEqualityComparer : IEqualityComparer<Region>
    {
        public bool Equals(Region x, Region y)
        {
            if(x.RegionId == null && y.RegionId == null)
            {
                return x.Name == y.Name && x.RegionSetId == y.RegionSetId;
            }
            else
            {
                return x.RegionId == y.RegionId;
            }
        }

        public int GetHashCode(Region obj)
        {
            return obj.GetHashCode();
        }
    }
}
