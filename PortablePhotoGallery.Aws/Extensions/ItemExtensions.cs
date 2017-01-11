using System;
using Amazon.SimpleDB.Model;
using PortablePhotoGallery.Shared.Models;

namespace PortablePhotoGallery.Aws.Extensions
{
    public static class ItemExtensions
    {
        public static PhotoMetadata ToPhotoMetadata(this Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var photoMetadata = new PhotoMetadata { PhotoId = item.Name };

            foreach (var attribute in item.Attributes)
            {
                switch (attribute.Name)
                {
                    case (nameof(PhotoMetadata.UserId)):
                        photoMetadata.UserId = attribute.Value;
                        break;
                    case (nameof(PhotoMetadata.ContentType)):
                        photoMetadata.ContentType = attribute.Value;
                        break;
                    case (nameof(PhotoMetadata.Description)):
                        photoMetadata.Description = attribute.Value;
                        break;
                    case (nameof(PhotoMetadata.PhotoDateTimeUtc)):
                        photoMetadata.PhotoDateTimeUtc = DateTime.Parse(attribute.Value);
                        break;
                    case (nameof(PhotoMetadata.Source)):
                        photoMetadata.Source = attribute.Value;
                        break;
                    case (nameof(PhotoMetadata.Title)):
                        photoMetadata.Title = attribute.Value;
                        break;
                    case (nameof(PhotoMetadata.UserName)):
                        photoMetadata.UserName = attribute.Value;
                        break;
                }
            }

            return photoMetadata;
        }
    }
}