using System;
using System.Collections.Generic;
using System.Text;

namespace WorldBuilder.Data
{
    public class GetFullPointOfInterestResultSet
    {
        public GetFullPointOfInterestResultSet(PointOfInterest poi, IEnumerable<Region> parents)
        {
            PointOfInterest = poi;
            ParentRegions = parents;
        }
        public PointOfInterest PointOfInterest { get; set; }
        public IEnumerable<Region> ParentRegions { get; set; }
    }
}
