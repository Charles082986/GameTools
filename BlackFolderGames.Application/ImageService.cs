using BlackFolderGames.Data.Entities;
using BlackFolderGames.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using BlackFolderGames.Application.Interfaces;
using BlackFolderGames.Application.Utilities;

namespace BlackFolderGames.Application
{
    public class ImageService : IImageService
    {
        private IBFGRepository _data;
        private IBasicOperation<ImageLog> _basic;
        private string _rootPath;
        public ImageService(IBFGRepository data, string rootPath)
        {
            _data = data;
            _basic = data;
            _rootPath = rootPath;
        }

        public byte[] GetByFriendlyName(string name)
        {
            throw new NotImplementedException();
        }

        public byte[] GetById(string id)
        {
            throw new NotImplementedException();
        }

        public bool IsFriendlyNameAvailable(string friendlyName)
        {
            throw new NotImplementedException();
        }

        public bool IsFriendlyNameSanitary(string friendlyName)
        {
            throw new NotImplementedException();
        }

        public bool IsOwner(string friendlyName, string userId)
        {
            if (string.IsNullOrEmpty(friendlyName)) { throw new ArgumentNullException("friendlyName"); }
            if (string.IsNullOrEmpty(userId)) { throw new ArgumentNullException("userId"); }
            ImageLog imageLog = _data.GetImageLogByFriendlyName(friendlyName);
            if(imageLog != null)
            {
                return imageLog.OwnerId == userId;
            }
            throw new ArgumentOutOfRangeException("frinedlyName", "No image found for friendly name.");
        }

        public bool CheckImageExistsByFriendlyName(string friendlyName)
        {
            throw new NotImplementedException();
        }

        public bool CheckImageExistsById(string id)
        {
            throw new NotImplementedException();
        }

        public bool UploadImage(byte[] image, string friendlyName, string userId)
        {
            if (!IsFriendlyNameSanitary(friendlyName))
            {
                throw new ArgumentException("Friendly name is not sanitary.", "friendlyName");
            }
            if (CheckImageExistsByFriendlyName(friendlyName))
            {
                var imageLog = _data.GetImageLogByFriendlyName(friendlyName);
                if(imageLog != null && imageLog.OwnerId != userId) { return false; }
                if(SaveFile(image, userId, imageLog.Id, true))
                {
                    return true;
                }
                return false;
            }
            else
            {
                var imageLog = _data.Create(new ImageLog { FriendlyName = friendlyName, OwnerId = userId });
                if(SaveFile(image, userId, imageLog.Id, false))
                {
                    return true;
                }
                _basic.Delete(imageLog.Id);
                return false;
            }
        }

        private bool SaveFile(byte[] file, string userId, string imageId, bool overwriteIfExists = false)
        {
            string imageExtension = string.Empty;
            if(ImageHelper.TryGetImageExtension(file, out imageExtension))
            {
                try
                {
                    string path = Path.Combine(_rootPath, userId, imageId + "." + imageExtension);
                    if (File.Exists(path) && overwriteIfExists) { File.Delete(path); } else { return false; }
                    var stream = File.Create(path, file.Length);
                    stream.Write(file, 0, file.Length);
                    stream.Close();
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            return false;
        }
    }
}
