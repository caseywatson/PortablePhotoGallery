using System;

namespace PortablePhotoGallery.Shared.Models
{
    public class PhotoMetadata
    {
        public string PhotoId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ContentType { get; set; }
        public string Source { get; set; }

        public DateTime PhotoDateTimeUtc { get; set; }
    }
}