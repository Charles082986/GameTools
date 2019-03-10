using BlackFolderGames.Data.Context;

namespace BlackFolderGames.Data.Interfaces
{
    public interface IBlackFolderGamesContextFactory
    {
        BlackFolderGamesDbContext Create();
    }
}