using BlackFolderGames.Data.Entities;
using BlackFolderGames.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using BlackFolderGames.Application.Interfaces;
using BlackFolderGames.Application.Utilities;
using System.Text.RegularExpressions;

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

        public byte[] GetByFriendlyName(string userId, string name)
        {
            ImageLog imageLog = _data.GetImageLogByFriendlyName(userId, name);
            if(imageLog != null)
            {
                return GetImageFile(userId, imageLog.Id);
            }
            return null;
        }

        public byte[] GetById(string userId, string id)
        {
            return GetImageFile(userId, id);
        }

        public bool IsFriendlyNameAvailable(string userId, string friendlyName)
        {
            return _data.GetImageLogByFriendlyName(userId, friendlyName) == null;
        }

        public bool IsFriendlyNameSanitary(string friendlyName)
        {
            return new Regex("^[a-zA-Z0-9-_]+$").IsMatch(friendlyName);
        }
        public bool CheckImageExistsById(string id)
        {
            return _data.Get<ImageLog>(id) != null;
        }

        public bool UploadImage(byte[] image, string friendlyName, string userId)
        {
            if (!IsFriendlyNameSanitary(friendlyName))
            {
                throw new ArgumentException("Friendly name is not sanitary.", "friendlyName");
            }
            var imageLog = _data.GetImageLogByFriendlyName(userId, friendlyName);
            if (imageLog != null)
            {
                if(SaveFile(image, userId, imageLog.Id, true))
                {
                    return true;
                }
                return false;
            }
            else
            {
                imageLog = _data.Create(new ImageLog { FriendlyName = friendlyName, OwnerId = userId });
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
                    DeleteExistingFiles(userId, imageId);
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

        private void DeleteExistingFiles(string userId, string imageId)
        {
            string path = Path.Combine(_rootPath, userId);
            var files = Directory.GetFiles(path, imageId + ".*");
            foreach(var file in files)
            {
                File.Delete(file);
            }
        }

        private byte[] GetImageFile(string userId, string imageId)
        {
            string path = Path.Combine(_rootPath, userId);
            var files = Directory.GetFiles(path, imageId + ".*");
            if(files != null && files.Length > 0)
            {
                return File.ReadAllBytes(files[0]);
            }
            return null;
        }
    }
}
