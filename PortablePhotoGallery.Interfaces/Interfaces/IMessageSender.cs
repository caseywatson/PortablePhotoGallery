using System.Threading.Tasks;

namespace PortablePhotoGallery.Shared.Interfaces
{
    public interface IMessageSender<T>
    {
        Task SendMessageAsync(T message);
    }
}