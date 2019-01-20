using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldBuilder.Data;

namespace BlackFolderGames.Application
{
    public class WorldService
    {
        private IWorldBuilderRepository _repo;
        public WorldService(IWorldBuilderRepository repository)
        {
            _repo = repository;
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
            Region savedRegion;
            if (region.RegionId == null)
            {
                savedRegion = CreateRegion(region);
            }
            else
            {
                savedRegion = GetFullRegion(region.RegionId);
            }

            var poisToAdd = region.PointsOfInterest.Except(savedRegion.PointsOfInterest, new PointOfInterestComparer());
            var poisToRemove = savedRegion.PointsOfInterest.Except(region.PointsOfInterest, new PointOfInterestComparer());

            foreach(PointOfInterest p in poisToAdd)
            {
                PointOfInterest workingPoI = p;
                if(workingPoI.PointOfInterestId == null)
                {
                    workingPoI = CreatePointOfInterest(workingPoI);
                }
                _repo.AddPointOfInterest(savedRegion.RegionId, workingPoI.PointOfInterestId);
            }

            foreach(PointOfInterest p in poisToRemove)
            {
                PointOfInterest workingPoI = p;
                if(workingPoI.PointOfInterestId == null)
                {
                    workingPoI = CreatePointOfInterest(workingPoI);
                }
                _repo.RemovePointOfInterest(savedRegion.RegionId, workingPoI.PointOfInterestId);
            }

            return GetFullRegion(savedRegion.RegionId);
        }

        public Region GetFullRegion(Guid id)
        {
            var region = _repo.GetRegion(id);
            region.InnerRegions = _repo.GetChildRegions(region);
            region.ParentRegions = _repo.GetParentRegion(region);
            region.PointsOfInterest = _repo.GetPointsOfInterest(region);
            return region;
        }

        public PointOfInterest GetFullPointOfInterest(Guid id)
        {
            var poi = _repo.GetPointOfInterest(id);
            poi.Regions = _repo.GetParentRegion(poi);
            return poi;
        }
    }
}
