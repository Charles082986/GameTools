using BlackFolderGames.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlackFolderGames.Application.Interfaces
{
    public interface IImageServiceFactory
    {
        IImageService Create();
    }
}
