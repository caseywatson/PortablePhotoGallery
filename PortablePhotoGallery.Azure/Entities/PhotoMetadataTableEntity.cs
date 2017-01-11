using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace PortablePhotoGallery.Azure.Entities
{
    public class PhotoMetadataTableEntity : TableEntity
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ContentType { get; set; }
        public string Source { get; set; }

        public DateTime PhotoDateTimeUtc { get; set; }
    }
}