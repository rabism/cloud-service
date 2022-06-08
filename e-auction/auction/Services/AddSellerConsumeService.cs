
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Logging;
using System;
using auction.Models;
using MediatR;
using auction.Entity;
using auction.Commands;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
namespace auction.Services
{
    
    
    public class AddSellerConsumeService : BackgroundService
    {
       
        private readonly IServiceScopeFactory scopefactory;
        readonly ILogger<AddSellerConsumeService> logger;
         readonly ConsumerConfig consumerConfig;
        readonly IConfiguration configuration;
        public AddSellerConsumeService(IServiceScopeFactory _scopefactory,
        ILogger<AddSellerConsumeService> _logger,
        ConsumerConfig _consumerConfig,
        IConfiguration _configuration
        )
        {
            scopefactory = _scopefactory;
            logger=_logger;
            consumerConfig=_consumerConfig;
            configuration=_configuration;
            
        }


        string Topic
        {
            get
            {
                string topic = configuration["Consumer:UserTopic"];
                if (string.IsNullOrEmpty(topic))
                    throw new Exception("Topic can't be null or empty! Service fail to start");
                return topic;
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
             logger.LogInformation("hosted service invoke");
             Task.Run(() => consumeUser(stoppingToken));
            return Task.CompletedTask;
        }


        async Task consumeUser(CancellationToken stoppingToken)
        {
            logger.LogInformation("consume user method invoke");
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var consumer = new ConsumerBuilder<string, byte[]>(consumerConfig).Build())
                {
                    consumer.Subscribe(Topic);
                    try
                    {
                        var consumeResult = consumer.Consume(stoppingToken);
                        if (consumeResult != null)
                        {
                            await Task.Run(() => AddSeller(consumeResult.Message.Key, consumeResult.Message.Value));
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e.Message);
                    }
                    finally
                    {
                        consumer.Close();
                    }

                }
            }

        }

        void AddSeller(string email, byte[] sellerDetails)
        {
            string serializedOrderData = Encoding.UTF8.GetString(sellerDetails);
            UserDetail seller = JsonSerializer.Deserialize<UserDetail>(serializedOrderData);
            if (seller == null)
                throw new Exception("no seller data found in message");
            using (var scope = scopefactory.CreateScope())
            {
                IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    AddUserDetailCommand command = new AddUserDetailCommand
                    {
                        Seller = seller
                    };
                    mediator.Send(command);
                logger.LogInformation("Seller Added successfully");
            }
       
        }

    }
    
}
