using System;
using System.Collections.Generic;
using System.Text;

namespace BlackFolderGames.Data.Entities
{
    public class ImageLog : EntityBase
    {
        public string OwnerId { get; set; }
        public string FriendlyName { get; set; }
        public string ImagePath { get; set; }
    }
}
