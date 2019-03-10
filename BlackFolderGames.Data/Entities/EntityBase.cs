using BlackFolderGames.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlackFolderGames.Data.Entities
{
    public class EntityBase : IEntityBase
    {
        public string Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime Created { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
