using System;
using System.Collections.Generic;
using System.Text;

namespace BlackFolderGames.Data.Entities
{
    public class CampaignSetting : EntityBase
    {
        public Guid WorldId { get; set; }
        public int StartingYear { get; set; }
    }
}
