using System;
using System.Collections.Generic;
using WorldBuilder.Data;

namespace BlackFolderGames.Application
{
    public interface IWorldService
    {
        PointOfInterest CreatePointOfInterest(PointOfInterest poi);
        Region CreateRegion(Region region);
        List<RegionSet> GetEditableRegionSets(string userId, string searchTerm = null);
        PointOfInterest GetFullPointOfInterest(string id);
        Region GetFullRegion(string id);
        List<RegionSet> GetOwnedRegionSets(string userId, string searchTerm = null);
        List<RegionSet> GetViewableRegionSets(string userId, string searchTerm = null);
        Region ModifyChildRegionRelationships(Region region);
        Region ModifyPointOfInterestRelationships(Region region);
        RegionSet CreateRegionSet(RegionSet regionSet);
        RegionSet GetRegionSet(string regionSetId);
        RegionSet UpdateRegionSet(RegionSet regionSet);
        bool CanEditRegionSet(string userId, string regionSetId);
        bool CanViewRegionSet(string userId, string regionSetId);
    }
}