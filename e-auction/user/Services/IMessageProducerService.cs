using System.Threading.Tasks;
using user.Models;

namespace user.Services
{
    public interface IMessageProducerService
    {
         Task WriteMessage(string topicName, UserBasicDetails userBasicDetails);
    }
}