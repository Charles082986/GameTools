using System;
using System.Collections.Generic;
using System.Text;

namespace BlackFolderGames.Application.Interfaces
{
    public interface IImageService
    {
        byte[] GetById(string id);
        byte[] GetByFriendlyName(string name);
        bool IsFriendlyNameAvailable(string friendlyName);
        bool IsFriendlyNameSanitary(string friendlyName);
        bool UploadImage(byte[] image, string friendlyName, string userId);
        bool IsOwner(string friendlyName, string userId);
    }
}
