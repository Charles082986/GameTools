using System;
using System.Collections.Generic;
using System.Text;

namespace WorldBuilder.Data
{
    public class GetFullRegionSetResultSet
    {
        public GetFullRegionSetResultSet() { }
        public RegionSet RegionSet { get; set; }
        public IEnumerable<Region> AllRegions { get; set; }
        public IEnumerable<Region> TopLevelRegions { get; set; }
        public IEnumerable<PointOfInterest> AllPointsOfInterest { get; set; }
        public IEnumerable<PointOfInterest> TopLevelPointsOfInterest { get; set; }
    }
}
