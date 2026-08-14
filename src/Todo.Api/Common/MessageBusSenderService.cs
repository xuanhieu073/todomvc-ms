using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace Todo.Api.Common;

public class ServiceBusOptions
{
  public const string SectionName = "ServiceBus";
  public string ConnectionString { get; set; } = string.Empty;
  public string TopicName { get; set; } = string.Empty;
}

public interface IMessageBusSenderService
{
  Task SendMessageAsync<T>(T messageBody, CancellationToken cancellationToken, IDictionary<string, object>?
    properties = null);
}

public class MessageBusSenderService(ServiceBusClient client, IOptions<ServiceBusOptions> options)
  : IMessageBusSenderService
{
  private readonly ServiceBusSender _sender = client.CreateSender(options.Value.TopicName);

  public async Task SendMessageAsync<T>(T messageBody, CancellationToken cancellationToken = default,
    IDictionary<string, object>? properties = null)
  {
    var json = JsonSerializer.Serialize(messageBody);
    var serviceBusMessage = new ServiceBusMessage(json)
    {
      ContentType = "application/json"
    };

    if (properties != null)
    {
      foreach (var property in properties)
      {
        serviceBusMessage.ApplicationProperties.Add(property.Key, property.Value);
      }
    }

    await _sender.SendMessageAsync(serviceBusMessage, cancellationToken);
  }
}