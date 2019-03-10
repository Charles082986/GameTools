using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackFolderGames.Application.Utilities
{
    public static class ImageHelper
    {
        public static bool TryGetImageContentType(byte[] imageBytes, out string contentType)
        {
            foreach (var imageType in imageContentTypes)
            {
                if (imageType.Key.SequenceEqual(imageBytes.Take(imageType.Key.Length)))
                {
                    contentType = imageType.Value.ContentType;
                    return true;
                }
            }
            contentType = string.Empty;
            return false;
        }

        public static bool TryGetImageExtension(byte[] imageBytes, out string contentType)
        {
            foreach (var imageType in imageContentTypes)
            {
                if (imageType.Key.SequenceEqual(imageBytes.Take(imageType.Key.Length)))
                {
                    contentType = imageType.Value.Extension;
                    return true;
                }
            }
            contentType = string.Empty;
            return false;
        }

        private static Dictionary<byte[], ImageType> imageContentTypes = new Dictionary<byte[], ImageType>()
        {
               { new byte[]{ 0x42, 0x4D },  new ImageType { ContentType = "image/bmp", Extension = ".bmp" } },
               { new byte[]{ 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, new ImageType { ContentType = "image/gif", Extension = ".gif" } },
               { new byte[]{ 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }, new ImageType { ContentType = "image/gif", Extension = ".gif" } },
               { new byte[]{ 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, new ImageType { ContentType = "image/png", Extension = ".png" } },
               { new byte[]{ 0xFF, 0xD8 }, new ImageType { ContentType = "image/jpg", Extension = ".jpg" } },
               { new byte[]{ 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 }, new ImageType { ContentType = "image/pipeg", Extension = ".jfif" } },
               { new byte[]{ 0x4D, 0x4D, 0x00, 0x2A }, new ImageType { ContentType = "image/tiff", Extension = ".tiff" } },
               { new byte[]{ 0x49, 0x49, 0x2A, 0x00 }, new ImageType { ContentType = "image/tiff", Extension = ".tiff" } },
               { new byte[]{ 0x00, 0x00, 0x01, 0x00 }, new ImageType { ContentType = "image/x-cion", Extension = ".ico" } },
        };

        private class ImageType
        {
            public string ContentType { get; set; }
            public string Extension { get; set; }
        }
    }
}
