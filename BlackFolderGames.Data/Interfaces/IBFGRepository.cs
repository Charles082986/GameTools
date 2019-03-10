using BlackFolderGames.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlackFolderGames.Data.Interfaces
{
    public interface IBFGRepository :
        IBasicOperation<Campaign>,
        IBasicOperation<CampaignSetting>,
        IBasicOperation<ImageLog>
    {
        T Get<T>(string id) where T : EntityBase;
        ImageLog GetImageLogByFriendlyName(string friendlyName);
    }
}
