using BlackFolderGames.Application.Interfaces;
using BlackFolderGames.Data.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlackFolderGames.Application.Factories
{
    public class ImageServiceFactory : IImageServiceFactory
    {
        private string _rootPath = string.Empty;
        private IBFGRepository _repo = null;
        public ImageServiceFactory(IBFGRepository repository, IConfiguration config)
        {
            _rootPath = config["BlackFolderGames:Image:RootPath"];
            _repo = repository;
        }

        public IImageService Create()
        {
            return new ImageService(_repo, _rootPath);
        }
    }
}
