using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldBuilder.Data.EqualityComparers;

namespace WorldBuilder.Data
{
    public class WorldBuilderRepository : IWorldBuilderRepository
    {
        private readonly IGraphClient _client;
        public WorldBuilderRepository(IGraphClientFactory factory)
        {
            _client = factory.Create();
        }

        public Region AddPointOfInterest(string parentRegionId, string poiId)
        {
            _client.Cypher
                .Match("(rs:RegionSet)-[:CONTAINS*]->(r:Region),(p:PointOfInterest)")
                .Where((Region r) => r.RegionId == parentRegionId)
                .AndWhere((PointOfInterest p) => p.PointOfInterestId == poiId)
                .Set("rs.LastEdit = {date}")
                .WithParams(new { date = DateTime.UtcNow })
                .CreateUnique("r-[:CONTAINS]->p")
                .ExecuteWithoutResults();
            return GetFullRegion(parentRegionId);
        }

        public Region[] AddRegionRelationship(string parentRegionId, string childRegionId)
        {
            _client.Cypher
                .Match("(rs:RegionSet)-[:CONTAINS*]->(r1:Region),(r2:Region)")
                .Where((Region r1) => r1.RegionId == parentRegionId)
                .AndWhere((Region r2) => r2.RegionId == childRegionId)
                .Set("rs.LastEdit = {date}")
                .WithParams(new { date = DateTime.UtcNow })
                .CreateUnique("r1-[:CONTAINS]->r2")
                .ExecuteWithoutResults();
            return new Region[] { GetFullRegion(parentRegionId), GetFullRegion(childRegionId) };
        }

        public RegionSet AddTopLevelRegion(string regionSetId, string regionId)
        {
            _client.Cypher
                .Match("(rs:RegionSet),(r:Region)")
                .Where((RegionSet rs) => rs.RegionSetId == regionSetId)
                .AndWhere((Region r) => r.RegionId == regionId)
                .Set("rs.LastEdit = {date}")
                .WithParams(new { date = DateTime.Now })
                .CreateUnique("rs-[:CONTAINS]->r")
                .ExecuteWithoutResults();
            return GetFullRegionSet(regionSetId);
        }

        public Region Create(Region region)
        {
            region.RegionId = Guid.NewGuid().ToString();
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
            poi.PointOfInterestId = Guid.NewGuid().ToString();
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
            regionSet.RegionSetId = Guid.NewGuid().ToString();
            regionSet.LastEdit = DateTime.Now;
            _client.Cypher
                .Merge("(u:User { UserId: {OwnerId}})")
                .Merge("(rs:RegionSet { RegionSetId: {RegionSetId}})")
                .OnCreate()
                    .Set("rs = {regionSet}")
                .Merge("(u)-[o:OWNS]->(rs)")
                .WithParams(new { regionSet.OwnerId, regionSet.RegionSetId, regionSet })
                .ExecuteWithoutResults();
            return GetFullRegionSet(regionSet.RegionSetId);
        }

        public RegionSet CopyRegionSet(string regionSetId, string newOwnerId = null)
        {
            RegionSet regionSet = GetFullRegionSet(regionSetId);
            var copy = regionSet;
            copy.RegionSetId = string.Empty;
            if(!string.IsNullOrEmpty(newOwnerId))
            {
                copy.OwnerId = newOwnerId;
            }
            copy = Create(copy);

            Dictionary<string, string> regionIdMap = new Dictionary<string, string>();
            Dictionary<string, string> poiIdMap = new Dictionary<string, string>();
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

        public Region CopyRegion(string regionId, string regionSetId)
        {
            var copyRegion = GetRegion(regionId);
            copyRegion.RegionId = string.Empty;
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

        public Region ModifyRegionRelationships(Region region)
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

        public Region ModifyParentRegionRelationships(Region region, Region graphRegion = null)
        {
            if (graphRegion == null)
            {
                graphRegion = GetFullRegion(region.RegionId);
            }
            if (region.ParentRegion == null)
            {
                RemoveRegionRelationship(graphRegion.ParentRegion.RegionId, region.RegionId);
            }
            else if (graphRegion.ParentRegion != null && graphRegion.ParentRegion.RegionId != region.ParentRegion.RegionId)
            {
                RemoveRegionRelationship(graphRegion.ParentRegion.RegionId, region.RegionId);
                AddRegionRelationship(region.ParentRegion.RegionId, region.RegionId);
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


        public PointOfInterest CopyPointOfInterest(string poiId, string regionSetId)
        {
            var copyPoI = GetPointOfInterest(poiId);
            copyPoI.PointOfInterestId = string.Empty;
            copyPoI.RegionSetId = regionSetId;
            return Create(copyPoI);
        }

        public void Delete(Region region)
        {
            try
            {
                _client.Cypher
                    .OptionalMatch("(rs:RegionSet)-[:CONTAINS*]->(r:Region)-[rel]-()")
                    .Where((Region r) => r.RegionId == region.RegionId)
                    .Set("rs.LastEdit = {date}")
                    .WithParams(new { date = DateTime.Now })
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
                    .OptionalMatch("(rs:RegionSet)-[:CONTAINS*]->(p:PointOfInterest)-[rel]-()")
                    .Where((PointOfInterest p) => poi.PointOfInterestId == p.PointOfInterestId)
                    .Set("rs.LastEdit = {date}")
                    .WithParams(new { date = DateTime.Now })
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

        public List<Region> GetChildRegions(RegionSet regionSet)
        {
            var childRegions = _client.Cypher
                .Match("(rs:RegionSet)-[:CONTAINS]->(r2:Region)")
                .Where((RegionSet rs) => rs.RegionSetId == regionSet.RegionSetId)
                .Return((r2) => r2.CollectAs<Region>())
                .Results.FirstOrDefault();
            if (childRegions != null && childRegions.Any())
            {
                return childRegions.ToList();
            }
            else
            {
                return new List<Region>();
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

        public PointOfInterest GetFullPointOfInterest(string id)
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

        public Region GetFullRegion(string id)
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
                        firstResult.Region.ParentRegion = firstResult.ParentRegions.FirstOrDefault();
                    }
                    else
                    {
                        firstResult.Region.ParentRegion = null;
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

        public User GetUser(string userId)
        {
            try
            {
                var user = _client.Cypher
                    .Match("(u:User)")
                    .Where((User u) => u.UserId == userId)
                    .Return(u => u.As<User>()).Results.FirstOrDefault();
                return user;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public RegionSet GetFullRegionSet(string regionSetId)
        {
            try
            {
                var regionSet = _client.Cypher
                    .Match("(rs:RegionSet { RegionSetId: {regionSetId} })")
                    .OptionalMatch("(rs)-[:CONTAINS*]->(r: Region)")
                    .OptionalMatch("(rs)-[:CONTAINS]->(tr: Region)")
                    .OptionalMatch("(rs)-[:CONTAINS*]->(p: PointOfInterest)")
                    .OptionalMatch("(rs)-[:CONTAINS]->(tp: PointOfInterest)")
                    .WithParams(new { regionSetId })
                    .Return((rs, r, tr, p, tp, u) => new GetFullRegionSetResultSet
                    {
                        RegionSet = rs.As<RegionSet>(),
                        AllRegions = r.CollectAs<Region>(),
                        TopLevelRegions = tr.CollectAs<Region>(),
                        AllPointsOfInterest = p.CollectAs<PointOfInterest>(),
                        TopLevelPointsOfInterest = tp.CollectAs<PointOfInterest>()
                    }).Results.FirstOrDefault();

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

        public RegionSet GetFullRegionSet(string regionSetName, string userId)
        {
            try
            {
                var regionSet = _client.Cypher
                    .Match("(rs:RegionSet)")
                    .Where((RegionSet rs) => rs.Name == regionSetName && rs.OwnerId == userId)
                    .Return(rs => rs.As<RegionSet>()).Results.FirstOrDefault();
                if(regionSet != null)
                {
                    return GetFullRegionSet(regionSet.RegionSetId);
                }
                return null;
            }
            catch
            {
                throw;
            }
        }

        public Region GetParentRegion(Region region)
        {
            var regions = _client.Cypher
                .Match("(r:Region)<-[:CONTAINS]-(r2:Region)")
                .Where((Region r) => r.RegionId == region.RegionId)
                .Return(r2 => r2.CollectAs<Region>()).Results.FirstOrDefault();
            if(regions != null && regions.Any())
            {
                return regions.FirstOrDefault();
            }
            return null;
        }

        public List<Region> GetParentRegions(PointOfInterest poi)
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

        public PointOfInterest GetPointOfInterest(string id)
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

        public Region GetRegion(string id)
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

        public RegionSet GetRegionSet(string regionSetId)
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

        public Region RemovePointOfInterest(string parentRegionId, string poiId)
        {
            _client.Cypher
                .Match("(rs:RegionSet)-[:CONTAINS*]->(r:Region)-[c:CONTAINS]->(p:PointOfInterest)")
                .Where((Region r) => r.RegionId == parentRegionId)
                .AndWhere((PointOfInterest p) => p.PointOfInterestId == poiId)
                .Set("rs.LastEdit = {date}")
                .WithParams(new { date = DateTime.Now })
                .Delete("c")
                .ExecuteWithoutResults();
            return GetFullRegion(parentRegionId);
        }

        public Region[] RemoveRegionRelationship(string parentRegionId, string childRegionId)
        {
            _client.Cypher
                .Match("(rs:RegionSet)-[:CONTAINS*]->(r:Region)-[c:CONTAINS]->(r2:Region)")
                .Where((Region r) => r.RegionId == parentRegionId)
                .AndWhere((Region r2) => r2.RegionId == childRegionId)
                .Set("rs.LastEdit = {date}")
                .WithParams(new { date = DateTime.Now })
                .Delete("c")
                .ExecuteWithoutResults();
            return new Region[] { GetFullRegion(parentRegionId), GetFullRegion(childRegionId) };
        }

        public Region Update(Region region)
        {
            _client.Cypher
                .Match("(rs:RegionSet)-[:CONTAINS*]->(r:Region)")
                .Where((Region r) => r.RegionId == region.RegionId)
                .Set("r = {region}, rs.LastEdit = {date}")
                .WithParams(new { region, date = DateTime.Now })
                .ExecuteWithoutResults();
            return GetFullRegion(region.RegionId);
        }

        public PointOfInterest Update(PointOfInterest poi)
        {
            _client.Cypher
                .Match("(rs:RegionSet)-[:CONTAINS*]->(p:PointOfInterest)")
                .Where((PointOfInterest p) => p.PointOfInterestId == poi.PointOfInterestId)
                .Set("p = {poi}, rs.LastEdit = {date}")
                .WithParams(new { poi, date = DateTime.Now })
                .ExecuteWithoutResults();
            return GetFullPointOfInterest(poi.PointOfInterestId);
        }

        public RegionSet Update(RegionSet regionSet)
        {
            regionSet.LastEdit = DateTime.Now;
            _client.Cypher
                .Match("(rs:RegionSet)")
                .Where((RegionSet rs) => rs.RegionSetId == regionSet.RegionSetId)
                .Set("rs = {regionSet}")
                .WithParams(new { regionSet })
                .ExecuteWithoutResults();
            return GetFullRegionSet(regionSet.RegionSetId);
        }

        public User Create(User user)
        {
            return _client.Cypher
                .Merge("(u:User { UserId: {UserId}})")
                .OnCreate()
                .Set("u = {user}")
                .WithParams(new { user.UserId, user })
                .Return(u => u.As<User>()).Results.FirstOrDefault();
        }

        public void Delete(string userId)
        {
            try
            {
                _client.Cypher
                    .OptionalMatch("(u:User)-[rel]->()")
                    .Where((User u) => u.UserId == userId)
                    .Delete("rel, u")
                    .ExecuteWithoutResults();
            }
            catch
            {
                throw;
            }
        }

        public void AddViewPermission(string userId, string regionSetId)
        {
            _client.Cypher
                .Match("(rs:RegionSet),(u:User)")
                .Where((RegionSet rs) => rs.RegionSetId == regionSetId)
                .AndWhere((User u) => u.UserId == userId)
                .CreateUnique("u-[:CAN_VIEW]->rs")
                .ExecuteWithoutResults();
        }

        public void AddEditPermission(string userId, string regionSetId)
        {
            _client.Cypher
                .Match("(rs:RegionSet),(u:User)")
                .Where((RegionSet rs) => rs.RegionSetId == regionSetId)
                .AndWhere((User u) => u.UserId == userId)
                .CreateUnique("u-[:CAN_EDIT]->rs")
                .CreateUnique("u-[:CAN_VIEW]->rs")
                .ExecuteWithoutResults();
        }

        public void ChangeOwnership(string userId, string regionSetId)
        {
            _client.Cypher
                .Match("(rs:RegionSet)<-[o:OWNS]-(),(u:User)")
                .Where((RegionSet rs) => rs.RegionSetId == regionSetId)
                .AndWhere((User u) => u.UserId == userId)
                .Set("rs.OwnerId = {userId}")
                .Delete("o")
                .CreateUnique("u-[:OWNS]->rs")
                .WithParams(new { userId })
                .ExecuteWithoutResults();
        }

        public List<RegionSet> GetOwnedRegionSets(string userId)
        {
            var results = _client.Cypher
                .Match("(u:User)-[:OWNS]->(rs:RegionSet)")
                .Where((User u) => u.UserId == userId)
                .Return(rs => rs.CollectAs<RegionSet>()).Results.FirstOrDefault();
            if(results != null && results.Any()) { return results.ToList(); }
            return null;
        }

        public List<RegionSet> GetViewableRegionSets(string userId)
        {
            var publicRegionSets = GetPublicRegionSets() ?? new List<RegionSet>();
            var results = _client.Cypher
                .Match("(u:User)-[:CAN_EDIT]->(rs:RegionSet)")
                .Where((User u) => u.UserId == userId)
                .Return(rs => rs.CollectAs<RegionSet>()).Results.FirstOrDefault();
            if (results != null && results.Any()) { return results.Union(publicRegionSets).Distinct(new RegionSetEqualityComparer()).ToList(); }
            return publicRegionSets.Any() ? publicRegionSets : null;
        }

        public List<RegionSet> GetEditableRegionSets(string userId)
        {
            var results = _client.Cypher
                .Match("(u:User)-[:CAN_EDIT]->(rs:RegionSet)")
                .Where((User u) => u.UserId == userId)
                .Return(rs => rs.CollectAs<RegionSet>()).Results.FirstOrDefault();
            if (results != null && results.Any()) { return results.ToList(); }
            return null;
        }

        public List<RegionSet> GetPublicRegionSets()
        {
            var results = _client.Cypher
                .Match("(rs:RegionSet)")
                .Where((RegionSet rs) => rs.IsPublic == true)
                .Return(rs => rs.As<RegionSet>()).Results;
            if(results != null && results.Any()) { return results.ToList(); }
            return null;
        }

        public List<Region> GetOverlappingRegions(Region region)
        {
            throw new NotImplementedException();
        }
    }
}
