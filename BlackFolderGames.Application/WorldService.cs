using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldBuilder.Data;

namespace BlackFolderGames.Application
{
    public class WorldService : IWorldService
    {
        private IWorldBuilderRepository _repo;
        public WorldService(IWorldBuilderRepository repository)
        {
            _repo = repository;
        }

        public RegionSet CreateRegionSet(RegionSet regionSet)
        {
            return _repo.Create(regionSet);
        }

        public RegionSet GetRegionSet(string regionSetId)
        {
            return _repo.GetFullRegionSet(regionSetId);
        }

        public RegionSet UpdateRegionSet(RegionSet regionSet)
        {
            return _repo.Update(regionSet);
        }

        public bool CanEditRegionSet(string userId, string regionSetId)
        {
            var regionSets = _repo.GetEditableRegionSets(userId);
            var regionSet = _repo.GetRegionSet(regionSetId);
            return (regionSets != null && regionSets.FirstOrDefault(rs => rs.RegionSetId == regionSetId) != null) || (regionSet != null && regionSet.OwnerId == userId);
        }

        public bool CanViewRegionSet(string userId, string regionSetId)
        {
            var viewableRegionSets = _repo.GetViewableRegionSets(userId);
            return (viewableRegionSets != null && viewableRegionSets.FirstOrDefault(rs => rs.RegionSetId == regionSetId) != null) || CanEditRegionSet(userId, regionSetId);
        }

        public List<RegionSet> GetOwnedRegionSets(string userId, string searchTerm = null)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return _repo.GetOwnedRegionSets(userId);
            }
            else
            {
                var results = (_repo.GetOwnedRegionSets(userId) ?? new List<RegionSet>()).Where(rs => rs.Name.Contains(searchTerm));
                return results.ToList();
            }
        }

        public List<RegionSet> GetEditableRegionSets(string userId, string searchTerm = null)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return _repo.GetEditableRegionSets(userId);
            }
            else
            {
                var results = (_repo.GetEditableRegionSets(userId) ?? new List<RegionSet>()).Where(rs => rs.Name.Contains(searchTerm));
                return results.ToList();
            }
        }

        public List<RegionSet> GetViewableRegionSets(string userId, string searchTerm = null)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return _repo.GetViewableRegionSets(userId);
            }
            else
            {
                var results = (_repo.GetViewableRegionSets(userId) ?? new List<RegionSet>()).Where(rs => rs.Name.Contains(searchTerm));
                return results.ToList();
            }
        }

        public Region CreateRegion(Region region)
        {
            return _repo.Create(region);
        }

        public PointOfInterest CreatePointOfInterest(PointOfInterest poi)
        {
            return _repo.Create(poi);
        }

        public Region ModifyChildRegionRelationships(Region region)
        {
            Region savedRegion;
            if (region.RegionId == null)
            {
                savedRegion = CreateRegion(region);
            }
            else
            {
                savedRegion = GetFullRegion(region.RegionId);
            }

            var regionsToAdd = region.InnerRegions.Except(savedRegion.InnerRegions, new RegionEqualityComparer());
            var regionsToRemove = savedRegion.InnerRegions.Except(region.InnerRegions, new RegionEqualityComparer());

            foreach(Region r in regionsToAdd)
            {
                Region workingRegion = r;
                if(workingRegion.RegionId == null)
                {
                    workingRegion = CreateRegion(workingRegion);
                }
                _repo.AddRegionRelationship(savedRegion.RegionId, workingRegion.RegionId);
            }

            foreach(Region r in regionsToRemove)
            {
                Region workingRegion = r;
                if(workingRegion.RegionId != null)
                {
                    _repo.RemoveRegionRelationship(savedRegion.RegionId, workingRegion.RegionId);
                }
            }
            return GetFullRegion(savedRegion.RegionId);
        }

        public Region ModifyPointOfInterestRelationships(Region region)
        {
            return _repo.ModifyRegionRelationships(region);
        }

        public Region GetFullRegion(string id)
        {
            var region = _repo.GetRegion(id);
            region.InnerRegions = _repo.GetChildRegions(region);
            region.ParentRegions = _repo.GetParentRegion(region);
            region.PointsOfInterest = _repo.GetPointsOfInterest(region);
            return region;
        }

        public PointOfInterest GetFullPointOfInterest(string id)
        {
            var poi = _repo.GetPointOfInterest(id);
            poi.Regions = _repo.GetParentRegion(poi);
            return poi;
        }
    }
}
