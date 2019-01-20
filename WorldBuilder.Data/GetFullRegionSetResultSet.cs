using System;
using System.Collections.Generic;
using System.Text;

namespace WorldBuilder.Data
{
    public class GetFullRegionSetResultSet
    {
        public GetFullRegionSetResultSet(RegionSet regionSet, IEnumerable<Region> allRegions, IEnumerable<Region> topRegions, IEnumerable<PointOfInterest> allPoIs, IEnumerable<PointOfInterest> topPoIs)
        {
            RegionSet = regionSet;
            AllRegions = allRegions;
            TopLevelRegions = topRegions;
            AllPointsOfInterest = allPoIs;
            TopLevelPointsOfInterest = topPoIs;
        }
        public RegionSet RegionSet { get; set; }
        public IEnumerable<Region> AllRegions { get; set; }
        public IEnumerable<Region> TopLevelRegions { get; set; }
        public IEnumerable<PointOfInterest> AllPointsOfInterest { get; set; }
        public IEnumerable<PointOfInterest> TopLevelPointsOfInterest { get; set; }
    }
}
