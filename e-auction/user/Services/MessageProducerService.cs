using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using user.Models;

namespace user.Services
{
    public class MessageProducerService: IMessageProducerService
    {
        readonly ProducerConfig producerConfig;
        public MessageProducerService(ProducerConfig _producerConfig)
        {
            producerConfig = _producerConfig;
        }

         public async Task WriteMessage(string topicName, UserBasicDetails userBasicDetails)
        {
            var serializedOrderData = JsonSerializer.Serialize<UserBasicDetails>(userBasicDetails);
            var objByteOrder = Encoding.UTF8.GetBytes(serializedOrderData);
            using (var producer = new ProducerBuilder<string, byte[]>(producerConfig).Build())
            {
                await producer.ProduceAsync(topicName, new Message<string, byte[]> { Key = userBasicDetails.Email, Value = objByteOrder });
            }
        }
    }
}