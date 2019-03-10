using System;
using System.Collections.Generic;
using System.Text;

namespace BlackFolderGames.Data.Interfaces
{
    public interface IEntityBase
    {
        string Id { get; set; }
        DateTime Created { get; set; }
    }
}
