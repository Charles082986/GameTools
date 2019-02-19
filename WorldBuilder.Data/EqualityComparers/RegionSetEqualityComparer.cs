using System;
using System.Collections.Generic;
using System.Text;

namespace WorldBuilder.Data.EqualityComparers
{
    public class RegionSetEqualityComparer : IEqualityComparer<RegionSet>
    {
        public bool Equals(RegionSet x, RegionSet y)
        {
            if (!string.IsNullOrEmpty(x.RegionSetId) && !string.IsNullOrEmpty(y.RegionSetId))
            {
                return x.RegionSetId.Equals(y.RegionSetId);
            }
            else
            {
                return x.OwnerId == y.OwnerId && x.Name == y.Name;
            }
        }

        public int GetHashCode(RegionSet obj)
        {
            return obj.GetHashCode();
        }
    }
}
