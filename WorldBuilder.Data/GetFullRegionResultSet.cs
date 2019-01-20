using System;
using System.Collections.Generic;
using System.Text;

namespace WorldBuilder.Data
{
    public class GetFullRegionResultSet
    {
        public GetFullRegionResultSet(Region region, IEnumerable<Region> parents, IEnumerable<Region> children, IEnumerable<PointOfInterest> pois)
        {
            Region = region;
            ParentRegions = parents;
            InnerRegions = children;
            PointsOfInterest = pois;
        }
        public Region Region { get; set; }
        public IEnumerable<Region> ParentRegions { get; set; }
        public IEnumerable<Region> InnerRegions { get; set; }
        public IEnumerable<PointOfInterest> PointsOfInterest { get; set; }
    }
}
