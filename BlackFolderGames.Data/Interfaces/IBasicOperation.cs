using System;
using System.Collections.Generic;
using System.Text;

namespace BlackFolderGames.Data.Interfaces
{
    public interface IBasicOperation<T> where T : IEntityBase
    {
        T Create(T entity);
        T Update(T entity);
        bool Delete(string entityId);
    }
}
