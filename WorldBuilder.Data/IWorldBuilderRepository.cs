using System;
using System.Collections.Generic;

namespace WorldBuilder.Data
{
    public interface IWorldBuilderRepository
    {
        Region AddPointOfInterest(string parentRegionId, string poiId);
        Region[] AddRegionRelationship(string parentRegionId, string childRegionId);
        PointOfInterest CopyPointOfInterest(string poiId, string regionSetId);
        Region CopyRegion(string regionId, string regionSetId);
        RegionSet CopyRegionSet(string regionSetId, string newOwnerId = null);
        PointOfInterest Create(PointOfInterest poi);
        Region Create(Region region);
        RegionSet Create(RegionSet regionSet);
        void Delete(PointOfInterest poi);
        void Delete(Region region);
        void Delete(RegionSet regionSet);
        List<Region> GetChildRegions(Region region);
        PointOfInterest GetFullPointOfInterest(string id);
        Region GetFullRegion(string id);
        RegionSet GetFullRegionSet(string regionSetId);
        RegionSet GetFullRegionSet(string regionSetName, string userId);
        List<Region> GetParentRegion(PointOfInterest poi);
        List<Region> GetParentRegion(Region region);
        PointOfInterest GetPointOfInterest(string id);
        List<PointOfInterest> GetPointsOfInterest();
        List<PointOfInterest> GetPointsOfInterest(Region region);
        Region GetRegion(string id);
        List<Region> GetRegions();
        RegionSet GetRegionSet(string regionSetId);
        
        List<Region> GetRootRegions(Region region);
        Region ModifyRegionRelationships(Region region);
        Region ModifyChildRegionRelationships(Region region, Region graphRegion = null);
        Region ModifyParentRegionRelationships(Region region, Region graphRegion = null);
        PointOfInterest ModifyPointOfInterestRelationships(PointOfInterest poi);
        Region ModifyPointOfInterestRelationships(Region region, Region graphRegion = null);
        Region RemovePointOfInterest(string parentRegionId, string poiId);
        Region[] RemoveRegionRelationship(string parentRegionId, string childRegionId);
        PointOfInterest Update(PointOfInterest poi);
        Region Update(Region region);
        RegionSet Update(RegionSet regionSet);
        User Create(User user);
        void Delete(string userId);
        void AddViewPermission(string userId, string regionSetId);
        void AddEditPermission(string userId, string regionSetId);
        void ChangeOwnership(string userId, string regionSetId);
        List<RegionSet> GetOwnedRegionSets(string userId);
        List<RegionSet> GetViewableRegionSets(string userId);
        List<RegionSet> GetEditableRegionSets(string userId);

    }
}