using BlackFolderGames.Data.Context;
using BlackFolderGames.Data.Entities;
using BlackFolderGames.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackFolderGames.Data.Repositories
{
    class BFGRepository : IBFGRepository
    {
        private BlackFolderGamesDbContext _ctx;
        public BFGRepository(IBlackFolderGamesContextFactory factory)
        {
            _ctx = factory.Create();
        }

        Campaign IBasicOperation<Campaign>.Create(Campaign entity)
        {
            throw new NotImplementedException();
        }

        CampaignSetting IBasicOperation<CampaignSetting>.Create(CampaignSetting entity)
        {
            throw new NotImplementedException();
        }

        ImageLog IBasicOperation<ImageLog>.Create(ImageLog entity)
        {
            if (!string.IsNullOrEmpty(entity.Id))
            {
                throw new ArgumentException("Entity already has a populated Id value.  Use the Update method to update an existing entity.","entity");
            }
            using (var t = _ctx.Database.BeginTransaction())
            {
                try
                {
                    entity.Id = Guid.NewGuid().ToString();
                    _ctx.ImageLogs.Add(entity);
                    _ctx.SaveChanges();
                    t.Commit();
                    return _ctx.ImageLogs.Find(entity.Id);
                }
                catch
                {
                    t.Rollback();
                    throw;
                }
            }
        }

        bool IBasicOperation<Campaign>.Delete(string entityId)
        {
            throw new NotImplementedException();
        }

        bool IBasicOperation<CampaignSetting>.Delete(string entityId)
        {
            throw new NotImplementedException();
        }

        bool IBasicOperation<ImageLog>.Delete(string entityId)
        {
            if (string.IsNullOrEmpty(entityId)) { throw new ArgumentNullException("entityId"); }
            using (var t = _ctx.Database.BeginTransaction())
            {
                try
                {
                    var entity = _ctx.ImageLogs.Find(entityId);
                    if (entity != null)
                    {
                        _ctx.ImageLogs.Remove(entity);
                        _ctx.SaveChanges();
                        t.Commit();
                        return true;
                    }
                    return false;
                }
                catch
                {
                    t.Rollback();
                    throw;
                }
            }
        }

        Campaign IBasicOperation<Campaign>.Update(Campaign entity)
        {
            throw new NotImplementedException();
        }

        CampaignSetting IBasicOperation<CampaignSetting>.Update(CampaignSetting entity)
        {
            throw new NotImplementedException();
        }

        ImageLog IBasicOperation<ImageLog>.Update(ImageLog entity)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                throw new ArgumentException("Entity does not have a populated Id value.  Use the Create function to save a new entity to the database.", "entity");
            }
            using (var t = _ctx.Database.BeginTransaction())
            {
                try
                {
                    _ctx.ImageLogs.Update(entity);
                    _ctx.SaveChanges();
                    t.Commit();
                    return _ctx.ImageLogs.Find(entity.Id);
                }
                catch
                {
                    t.Rollback();
                    throw;
                }
            }
        }

        public ImageLog GetImageLogByFriendlyName(string ownerId, string friendlyName)
        {
            return _ctx.ImageLogs.FirstOrDefault(il => il.FriendlyName == friendlyName && il.OwnerId == ownerId);
        }

        public T Get<T>(string id) where T : EntityBase
        {
            return _ctx.Set<T>().FirstOrDefault(e => e.Id == id);
        }
    }
}
