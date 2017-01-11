using System;
using PortablePhotoGallery.Azure.Entities;
using PortablePhotoGallery.Shared.Models;

namespace PortablePhotoGallery.Azure.Extensions
{
    public static class PhotoMetadataTableEntityExtensions
    {
        public static PhotoMetadata ToPhotoMetadata(this PhotoMetadataTableEntity tableEntity)
        {
            if (tableEntity == null)
                throw new ArgumentNullException(nameof(tableEntity));

            return new PhotoMetadata
            {
                PhotoId = tableEntity.RowKey,
                UserId = tableEntity.UserId,
                UserName = tableEntity.UserName,
                Title = tableEntity.Title,
                Description = tableEntity.Description,
                ContentType = tableEntity.ContentType,
                Source = tableEntity.Source,
                PhotoDateTimeUtc = tableEntity.PhotoDateTimeUtc
            };
        }
    }
}