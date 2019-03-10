using System;
using System.Collections.Generic;
using System.Text;

namespace BlackFolderGames.Application.Interfaces
{
    public interface IImageService
    {
        byte[] GetById(string userId, string id);
        byte[] GetByFriendlyName(string userId, string name);
        bool IsFriendlyNameSanitary(string friendlyName);
        bool UploadImage(byte[] image, string friendlyName, string userId);
    }
}
