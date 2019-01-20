using System;
using System.Collections.Generic;
using System.Text;

namespace WorldBuilder.Data
{
    public interface IWorldBuilderRepository
    {
        Region Create(Region region);
        Region Update(Region region);
        void Delete(Region region);
        Region GetRegion(Guid id);
        Region GetFullRegion(Guid id);
        Region[] AddRegionRelationship(Guid parentRegionId, Guid childRegionId);
        Region[] RemoveRegionRelationship(Guid parentRegionId, Guid childRegionId);
        Region AddPointOfInterest(Guid parentRegionId, Guid poiId);
        Region RemovePointOfInterest(Guid parentRegionId, Guid poiId);
        List<Region> GetRegions();
        List<Region> GetChildRegions(Region region);
        List<Region> GetParentRegion(Region region);
        List<Region> GetParentRegion(PointOfInterest poi);

        PointOfInterest Create(PointOfInterest poi);
        PointOfInterest Update(PointOfInterest poi);
        void Delete(PointOfInterest poi);
        PointOfInterest GetPointOfInterest(Guid id);
        PointOfInterest GetFullPointOfInterest(Guid id);
        List<PointOfInterest> GetPointsOfInterest();
        List<PointOfInterest> GetPointsOfInterest(Region region);

        RegionSet GetFullRegionSet(string regionSetName, Guid ownerId);
        RegionSet Create(RegionSet regionSet);
        RegionSet Update(RegionSet regionSet);
        void Delete(RegionSet regionSet);
        RegionSet GetRegionSet(Guid regionSetId);
        
    }
}
