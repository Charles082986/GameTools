using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldBuilder.Data
{
    public class WorldBuilderRepository : IWorldBuilderRepository
    {
        private readonly IGraphClient _client;
        public WorldBuilderRepository(IGraphClientFactory factory)
        {
            _client = factory.Create();
        }

        public Region AddPointOfInterest(Guid parentRegionId, Guid poiId)
        {
            _client.Cypher
                .Match("(r:Region),(p:PointOfInterest)")
                .Where((Region r) => r.RegionId == parentRegionId)
                .AndWhere((PointOfInterest p) => p.PointOfInterestId == poiId)
                .CreateUnique("r-[:CONTAINS]->p")
                .ExecuteWithoutResults();
            return GetFullRegion(parentRegionId);
        }

        public Region[] AddRegionRelationship(Guid parentRegionId, Guid childRegionId)
        {
            _client.Cypher
                .Match("(r1:Region),(r2:Region)")
                .Where((Region r1) => r1.RegionId == parentRegionId)
                .AndWhere((Region r2) => r2.RegionId == childRegionId)
                .CreateUnique("r1-[:CONTAINS]->r2")
                .ExecuteWithoutResults();
            return new Region[] { GetFullRegion(parentRegionId), GetFullRegion(childRegionId) };
        }

        public Region Create(Region region)
        {
            region.RegionId = Guid.NewGuid();
            _client.Cypher
                .Merge("(r:Region { RegionId: {RegionId}})")
                .OnCreate()
                    .Set("r = {region}")
                    .WithParams(new { region.RegionId, region })
                .ExecuteWithoutResults();
            return GetFullRegion(region.RegionId);
        }

        public PointOfInterest Create(PointOfInterest poi)
        {
            poi.PointOfInterestId = Guid.NewGuid();
            _client.Cypher
                .Merge("(p:PointOfInterest { PointOfInterestId: {PointOfInterestId}})")
                .OnCreate()
                    .Set("p = {poi}")
                    .WithParams(new { poi.PointOfInterestId, poi })
                .ExecuteWithoutResults();
            return GetFullPointOfInterest(poi.PointOfInterestId);
        }

        public RegionSet Create(RegionSet regionSet)
        {
            regionSet.RegionSetId = Guid.NewGuid();
            _client.Cypher
                .Merge("(rs:RegionSet { RegionSetId: {RegionSetId}})")
                .OnCreate()
                    .Set("rs = {regionSet}")
                    .WithParams(new { regionSet.RegionSetId, regionSet })
                .ExecuteWithoutResults();
            return GetFullRegionSet(regionSet.RegionSetId);
        }

        public RegionSet CopyRegionSet(Guid regionSetId, Guid? newOwnerId = null)
        {
            RegionSet regionSet = GetFullRegionSet(regionSetId);
            var copy = regionSet;
            copy.RegionSetId = Guid.Empty;
            if(newOwnerId.HasValue && newOwnerId != Guid.Empty)
            {
                copy.OwnerId = newOwnerId.Value;
            }
            copy = Create(copy);

            Dictionary<Guid, Guid> regionIdMap = new Dictionary<Guid, Guid>();
            Dictionary<Guid, Guid> poiIdMap = new Dictionary<Guid, Guid>();
            foreach(Region r in regionSet.AllRegions)
            {
                regionIdMap.Add(r.RegionId, CopyRegion(r.RegionId, copy.RegionSetId).RegionId);
            }

            foreach(PointOfInterest p in regionSet.AllPointsOfInterest)
            {
                poiIdMap.Add(p.PointOfInterestId, CopyPointOfInterest(p.PointOfInterestId, copy.RegionSetId).PointOfInterestId);
            }

            foreach(Region region in regionSet.AllRegions)
            {
                var fullRegion = GetFullRegion(region.RegionId);
                var mappedRegionId = regionIdMap[region.RegionId];
                var mappedRegion = GetFullRegion(mappedRegionId);

                foreach(Region innerRegion in fullRegion.InnerRegions)
                {
                    var mappedInnerRegionId = regionIdMap[innerRegion.RegionId];
                    AddRegionRelationship(mappedRegionId, mappedInnerRegionId);
                    
                }
                foreach(PointOfInterest poi in fullRegion.PointsOfInterest)
                {
                    var mappedPointOfInterestId = poiIdMap[poi.PointOfInterestId];
                    AddPointOfInterest(mappedRegionId, mappedPointOfInterestId);
                }
            }

            return GetFullRegionSet(copy.RegionSetId);
        }

        public Region CopyRegion(Guid regionId, Guid regionSetId)
        {
            var copyRegion = GetRegion(regionId);
            copyRegion.RegionId = Guid.Empty;
            copyRegion.RegionSetId = regionSetId;
            return Create(copyRegion);
        }

        public PointOfInterest ModifyPointOfInterestRelationships(PointOfInterest poi)
        {
            var graphPoI = GetFullPointOfInterest(poi.PointOfInterestId);

            var regionsToAdd = poi.Regions.Except(graphPoI.Regions, new RegionEqualityComparer());
            var regionsToRemove = graphPoI.Regions.Except(poi.Regions, new RegionEqualityComparer());

            foreach(var r in regionsToAdd)
            {
                AddPointOfInterest(r.RegionId, poi.PointOfInterestId);
            }

            foreach(var r in regionsToRemove)
            {
                RemovePointOfInterest(r.RegionId, poi.PointOfInterestId);
            }

            return GetFullPointOfInterest(poi.PointOfInterestId);
        }

        public Region ModifiyRegionRelationships(Region region)
        {
            var graphRegion = GetFullRegion(region.RegionId);

            ModifyPointOfInterestRelationships(region, graphRegion);
            ModifyChildRegionRelationships(region, graphRegion);
            return ModifyParentRegionRelationships(region, graphRegion);
        }

        public Region ModifyPointOfInterestRelationships(Region region, Region graphRegion = null)
        {
            if (graphRegion == null)
            {
                graphRegion = GetFullRegion(region.RegionId);
            }

            var poisToAdd = region.PointsOfInterest.Except(graphRegion.PointsOfInterest, new PointOfInterestComparer());
            var poisToRemove = graphRegion.PointsOfInterest.Except(region.PointsOfInterest, new PointOfInterestComparer());

            foreach (var p in poisToAdd)
            {
                AddPointOfInterest(region.RegionId, p.PointOfInterestId);
            }
            foreach (var p in poisToRemove)
            {
                RemovePointOfInterest(region.RegionId, p.PointOfInterestId);
            }

            return GetFullRegion(region.RegionId);
        }

        public Region ModifyChildRegionRelationships(Region region, Region graphRegion = null)
        {
            if (graphRegion == null)
            {
                graphRegion = GetFullRegion(region.RegionId);
            }

            var childRegionsToAdd = region.InnerRegions.Except(graphRegion.InnerRegions, new RegionEqualityComparer());
            var childRegionsToRemove = graphRegion.InnerRegions.Except(region.InnerRegions, new RegionEqualityComparer());

            foreach (var r in childRegionsToAdd)
            {
                AddRegionRelationship(region.RegionId, r.RegionId);
            }
            foreach (var r in childRegionsToRemove)
            {
                RemoveRegionRelationship(region.RegionId, r.RegionId);
            }

            return GetFullRegion(region.RegionId);
        }

        public Region ModifyParentRegionRelationships(Region region, Region graphRegion = null)
        {
            if(graphRegion == null)
            {
                graphRegion = GetFullRegion(region.RegionId);
            }

            var parentRegionsToAdd = region.ParentRegions.Except(graphRegion.ParentRegions, new RegionEqualityComparer());
            var parentRegionsToRemove = graphRegion.ParentRegions.Except(region.ParentRegions, new RegionEqualityComparer());
            foreach (var r in parentRegionsToAdd)
            {
                AddRegionRelationship(r.RegionId, region.RegionId);
            }
            foreach (var r in parentRegionsToRemove)
            {
                RemoveRegionRelationship(r.RegionId, region.RegionId);
            }

            return GetFullRegion(region.RegionId);
        }


        public PointOfInterest CopyPointOfInterest(Guid poiId, Guid regionSetId)
        {
            var copyPoI = GetPointOfInterest(poiId);
            copyPoI.PointOfInterestId = Guid.Empty;
            copyPoI.RegionSetId = regionSetId;
            return Create(copyPoI);
        }

        public void Delete(Region region)
        {
            try
            {
                _client.Cypher
                    .OptionalMatch("(r:Region)-[rel]-()")
                    .Where((Region r) => r.RegionId == region.RegionId)
                    .Delete("rel, r")
                    .ExecuteWithoutResults();
            }
            catch
            {
                throw;
            }
        }

        public void Delete(PointOfInterest poi)
        {
            try
            {
                _client.Cypher
                    .OptionalMatch("(p:PointOfInterest)-[rel]-()")
                    .Where((PointOfInterest p) => poi.PointOfInterestId == p.PointOfInterestId)
                    .Delete("rel, p")
                    .ExecuteWithoutResults();
            }
            catch
            {
                throw;
            }
        }

        public void Delete(RegionSet regionSet)
        {
            try
            {
                _client.Cypher
                    .OptionalMatch("(rs:RegionSet)-[rel:CONTAINS*]->(n)")
                    .Where((RegionSet rs) => rs.RegionSetId == regionSet.RegionSetId)
                    .Delete("rel, rs, n")
                    .ExecuteWithoutResults();
            }
            catch
            {
                throw;
            }
        }

        public List<Region> GetChildRegions(Region region)
        {
            var childRegions = _client.Cypher
                .Match("(r:Region)-[:CONTAINS]->(r2:Region)")
                .Where((Region r) => r.RegionId == region.RegionId)
                .Return((r2) => r2.CollectAs<Region>())
                .Results.FirstOrDefault();
            if(childRegions != null && childRegions.Any())
            {
                return childRegions.ToList();
            }
            else
            {
                return new List<Region>();
            }
        }

        public PointOfInterest GetFullPointOfInterest(Guid id)
        {
            try
            {
                var fullPoIResultSets = _client.Cypher
                    .Match("(p:PointOfInterest)<-[:CONTAINS]-(r:Region)")
                    .Where((PointOfInterest p) => p.PointOfInterestId == id)
                    .Return((poi, regs) => new GetFullPointOfInterestResultSet(poi.As<PointOfInterest>(), regs.CollectAs<Region>())).Results;
                var firstResult = fullPoIResultSets.FirstOrDefault();
                if(firstResult != null && firstResult.PointOfInterest != null)
                {
                    if (firstResult.ParentRegions != null && firstResult.ParentRegions.Any())
                    {
                        firstResult.PointOfInterest.Regions = firstResult.ParentRegions.ToList();
                    }
                    else
                    {
                        firstResult.PointOfInterest.Regions = new List<Region>();
                    }
                    return firstResult.PointOfInterest;
                }
                return null;
            }
            catch
            {
                throw;
            }
            
        }

        public Region GetFullRegion(Guid id)
        {
            try
            {
                var fullRegionResultSets = _client.Cypher
                    .Match("(rP:Region)-[:CONTAINS]->(r:Region)-[:CONTAINS]->(rC:Region),(r)-[:CONTAINS]->(p:PointOfInterest)")
                    .Where((Region r) => r.RegionId == id)
                    .Return((rP, r, rC, p) => new GetFullRegionResultSet(r.As<Region>(), rP.CollectAs<Region>(), rC.CollectAs<Region>(), p.CollectAs<PointOfInterest>()))
                    .Results;
                var firstResult = fullRegionResultSets.FirstOrDefault();
                if (firstResult != null && firstResult.Region != null)
                {
                    if (firstResult.ParentRegions != null && firstResult.ParentRegions.Any())
                    {
                        firstResult.Region.ParentRegions = firstResult.ParentRegions.ToList();
                    }
                    else
                    {
                        firstResult.Region.ParentRegions = new List<Region>();
                    }

                    if (firstResult.InnerRegions != null && firstResult.InnerRegions.Any())
                    {
                        firstResult.Region.InnerRegions = firstResult.InnerRegions.ToList();
                    }
                    else
                    {
                        firstResult.Region.InnerRegions = new List<Region>();
                    }

                    if (firstResult.PointsOfInterest != null && firstResult.PointsOfInterest.Any())
                    {
                        firstResult.Region.PointsOfInterest = firstResult.PointsOfInterest.ToList();
                    }
                    else
                    {
                        firstResult.Region.PointsOfInterest = new List<PointOfInterest>();
                    }
                    return firstResult.Region;
                }
                return null;
            }
            catch
            {
                throw;
            }
        }

        public RegionSet GetFullRegionSet(Guid regionSetId)
        {
            try
            {
                var regionSet = _client.Cypher
                    .Match("(rs:RegionSet)-[:CONTAINS*]->(r:Region),(rs)-[:CONTAINS]->(tr:Region),(rs)-[:CONTAINS*]->(p:PointOfInterest),(rs)-[:CONTAINS]->(tp:PointOfInterest)")
                    .Where((RegionSet rs) => rs.RegionSetId == regionSetId)
                    .Return((rs, r, tr, p, tp) => new GetFullRegionSetResultSet(rs.As<RegionSet>(), r.CollectAs<Region>(), tr.CollectAs<Region>(), p.CollectAs<PointOfInterest>(), tp.CollectAs<PointOfInterest>()))
                    .Results.FirstOrDefault();

                if (regionSet != null)
                {
                    if (regionSet.AllRegions != null && regionSet.AllRegions.Any())
                    {
                        regionSet.RegionSet.AllRegions = regionSet.AllRegions.ToList();
                    }
                    else
                    {
                        regionSet.RegionSet.AllRegions = new List<Region>();
                    }

                    if (regionSet.TopLevelRegions != null && regionSet.TopLevelRegions.Any())
                    {
                        regionSet.RegionSet.TopLevelRegions = regionSet.TopLevelRegions.ToList();
                    }
                    else
                    {
                        regionSet.RegionSet.TopLevelRegions = new List<Region>();
                    }

                    if (regionSet.AllPointsOfInterest != null && regionSet.AllPointsOfInterest.Any())
                    {
                        regionSet.RegionSet.AllPointsOfInterest = regionSet.AllPointsOfInterest.ToList();
                    }
                    else
                    {
                        regionSet.RegionSet.AllPointsOfInterest = new List<PointOfInterest>();
                    }

                    if (regionSet.TopLevelPointsOfInterest != null && regionSet.TopLevelPointsOfInterest.Any())
                    {
                        regionSet.RegionSet.TopLevelPointsOfInterest = regionSet.TopLevelPointsOfInterest.ToList();
                    }
                    else
                    {
                        regionSet.RegionSet.TopLevelPointsOfInterest = new List<PointOfInterest>();
                    }
                    return regionSet.RegionSet;
                }
                return null;
            }
            catch
            {
                throw;
            }
        }

        public RegionSet GetFullRegionSet(string regionSetName, Guid userId)
        {
            try
            {
                var regionSet = _client.Cypher
                    .Match("(rs:RegionSet)-[:CONTAINS*]->(r:Region),(rs)-[:CONTAINS]->(tr:Region),(rs)-[:CONTAINS*]->(p:PointOfInterest),(rs)-[:CONTAINS]->(tp:PointOfInterest)")
                    .Where((RegionSet rs) => rs.OwnerId == userId && rs.Name == regionSetName)
                    .Return((rs, r, tr, p, tp) => new GetFullRegionSetResultSet(rs.As<RegionSet>(), r.CollectAs<Region>(), tr.CollectAs<Region>(), p.CollectAs<PointOfInterest>(), tp.CollectAs<PointOfInterest>()))
                    .Results.FirstOrDefault();

                if (regionSet != null)
                {
                    if (regionSet.AllRegions != null && regionSet.AllRegions.Any())
                    {
                        regionSet.RegionSet.AllRegions = regionSet.AllRegions.ToList();
                    }
                    else
                    {
                        regionSet.RegionSet.AllRegions = new List<Region>();
                    }

                    if (regionSet.TopLevelRegions != null && regionSet.TopLevelRegions.Any())
                    {
                        regionSet.RegionSet.TopLevelRegions = regionSet.TopLevelRegions.ToList();
                    }
                    else
                    {
                        regionSet.RegionSet.TopLevelRegions = new List<Region>();
                    }

                    if (regionSet.AllPointsOfInterest != null && regionSet.AllPointsOfInterest.Any())
                    {
                        regionSet.RegionSet.AllPointsOfInterest = regionSet.AllPointsOfInterest.ToList();
                    }
                    else
                    {
                        regionSet.RegionSet.AllPointsOfInterest = new List<PointOfInterest>();
                    }

                    if (regionSet.TopLevelPointsOfInterest != null && regionSet.TopLevelPointsOfInterest.Any())
                    {
                        regionSet.RegionSet.TopLevelPointsOfInterest = regionSet.TopLevelPointsOfInterest.ToList();
                    }
                    else
                    {
                        regionSet.RegionSet.TopLevelPointsOfInterest = new List<PointOfInterest>();
                    }
                    return regionSet.RegionSet;
                }
                return null;
            }
            catch
            {
                throw;
            }
        }

        public List<Region> GetParentRegion(Region region)
        {
            var regions = _client.Cypher
                .Match("(r:Region)<-[:CONTAINS]-(r2:Region)")
                .Where((Region r) => r.RegionId == region.RegionId)
                .Return(r2 => r2.CollectAs<Region>()).Results.FirstOrDefault();
            if(regions != null && regions.Any())
            {
                return regions.ToList();
            }
            return new List<Region>();
        }

        public List<Region> GetParentRegion(PointOfInterest poi)
        {
            var regions = _client.Cypher
                .Match("(p:PointOfInterest)<-[:CONTAINS]-(r2:Region)")
                .Where((PointOfInterest p) => p.PointOfInterestId == poi.PointOfInterestId)
                .Return(r2 => r2.CollectAs<Region>()).Results.FirstOrDefault();
            if (regions != null && regions.Any())
            {
                return regions.ToList();
            }
            return new List<Region>();
        }

        public PointOfInterest GetPointOfInterest(Guid id)
        {
            return _client.Cypher
                .Match("(p:PointOfInterest)")
                .Where((PointOfInterest p) => p.PointOfInterestId == id)
                .Return(p => p.As<PointOfInterest>()).Results.FirstOrDefault();
        }

        public List<PointOfInterest> GetPointsOfInterest()
        {
            var pois = _client.Cypher
                .Match("(p:PointOfInterest)")
                .Return(p => p.As<PointOfInterest>()).Results;
            if(pois != null && pois.Any())
            {
                return pois.ToList();
            }
            return new List<PointOfInterest>();
        }

        public List<PointOfInterest> GetPointsOfInterest(Region region)
        {
            var childPoIs = _client.Cypher
                    .Match("(r:Region)-[:CONTAINS]->(p:PointOfInterest)")
                    .Where((Region r) => r.RegionId == region.RegionId)
                    .Return((p) => p.CollectAs<PointOfInterest>())
                    .Results.FirstOrDefault();
            if (childPoIs != null && childPoIs.Any())
            {
                return childPoIs.ToList();
            }
            else
            {
                return new List<PointOfInterest>();
            }
        }

        public Region GetRegion(Guid id)
        {
            return _client.Cypher
                .Match("(r:Region)")
                .Where((Region r) => r.RegionId == id)
                .Return(p => p.As<Region>()).Results.FirstOrDefault();
        }

        public List<Region> GetRegions()
        {
            var regions = _client.Cypher
                .Match("(r:Region)")
                .Return(p => p.As<Region>()).Results;
            if (regions != null && regions.Any())
            {
                return regions.ToList();
            }
            return new List<Region>();
        }

        public RegionSet GetRegionSet(Guid regionSetId)
        {
            return _client.Cypher
                .Match("(rs:RegionSet)")
                .Where((RegionSet rs) => rs.RegionSetId == regionSetId)
                .Return((rs) => rs.As<RegionSet>()).Results.FirstOrDefault();
        }

        public List<Region> GetRootRegions(Region region)
        {
            var regions = _client.Cypher
                .Match("(r:Region)<-[:CONTAINS]-(rh:Region)")
                .Where((Region r) => r.RegionId == region.RegionId)
                .OptionalMatch("(rh)<-[d:CONTAINS]-(:Region)")
                .Where("d is null")
                .Return(rh => rh.CollectAs<Region>()).Results.FirstOrDefault();
            if (regions != null && regions.Any())
            {
                return regions.ToList();
            }
            return new List<Region>();
        }

        public Region RemovePointOfInterest(Guid parentRegionId, Guid poiId)
        {
            _client.Cypher
                .Match("(r:Region)-[c:CONTAINS]->(p:PointOfInterest)")
                .Where((Region r) => r.RegionId == parentRegionId)
                .AndWhere((PointOfInterest p) => p.PointOfInterestId == poiId)
                .Delete("c")
                .ExecuteWithoutResults();
            return GetFullRegion(parentRegionId);
        }

        public Region[] RemoveRegionRelationship(Guid parentRegionId, Guid childRegionId)
        {
            _client.Cypher
                .Match("(r:Region)-[c:CONTAINS]->(r2:Region)")
                .Where((Region r) => r.RegionId == parentRegionId)
                .AndWhere((Region r2) => r2.RegionId == childRegionId)
                .Delete("c")
                .ExecuteWithoutResults();
            return new Region[] { GetFullRegion(parentRegionId), GetFullRegion(childRegionId) };
        }

        public Region Update(Region region)
        {
            _client.Cypher
                .Match("(r:Region)")
                .Where((Region r) => r.RegionId == region.RegionId)
                .Set("r = {region}")
                .WithParams(new { region })
                .ExecuteWithoutResults();
            return GetFullRegion(region.RegionId);
        }

        public PointOfInterest Update(PointOfInterest poi)
        {
            _client.Cypher
                .Match("(p:PointOfInterest)")
                .Where((PointOfInterest p) => p.PointOfInterestId == poi.PointOfInterestId)
                .Set("p = {poi}")
                .WithParams(new { poi })
                .ExecuteWithoutResults();
            return GetFullPointOfInterest(poi.PointOfInterestId);
        }

        public RegionSet Update(RegionSet regionSet)
        {
            _client.Cypher
                .Match("(rs:RegionSet)")
                .Where((RegionSet rs) => rs.RegionSetId == regionSet.RegionSetId)
                .Set("rs = {regionSet}")
                .WithParams(new { regionSet })
                .ExecuteWithoutResults();
            return GetFullRegionSet(regionSet.RegionSetId);
        }
    }
}
