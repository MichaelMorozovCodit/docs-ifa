# PubSub Component

The PubSub component enables publish-subscribe messaging patterns in Invictus Framework.

## Overview

PubSub provides:
- Asynchronous message delivery
- Topic-based routing
- Multiple subscriber support
- Dead-letter queue handling

## Configuration

### Basic Setup

```json
{
  "name": "pubsub-endpoint",
  "type": "pubsub",
  "topic": "orders",
  "configuration": {
    "serviceBusNamespace": "invictus-sb",
    "topicName": "order-events",
    "subscription": "order-processor",
    "maxConcurrentCalls": 10
  }
}
```

### Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `serviceBusNamespace` | string | Yes | Azure Service Bus namespace |
| `topicName` | string | Yes | Topic name |
| `subscription` | string | Yes | Subscription name |
| `maxConcurrentCalls` | integer | No | Max concurrent messages (default: 1) |
| `prefetchCount` | integer | No | Messages to prefetch (default: 0) |

## Publishing Messages

### Example: Publish Order Event

```csharp
var message = new OrderCreatedEvent
{
    OrderId = "ORD-123",
    CustomerId = "CUST-456",
    Amount = 99.99m,
    Timestamp = DateTime.UtcNow
};

await pubSubClient.PublishAsync("orders", message);
```

## Subscribing to Messages

### Create Subscription

```csharp
pubSubClient.Subscribe("orders", "order-processor", async (message) =>
{
    var order = message.As<OrderCreatedEvent>();
    await ProcessOrder(order);
});
```

## Message Filters

Apply filters to subscriptions:

```json
{
  "filters": [
    {
      "property": "messageType",
      "operator": "equals",
      "value": "OrderCreated"
    }
  ]
}
```

## Dead-Letter Queue

Handle failed messages:

```csharp
var deadLetterMessages = await pubSubClient
    .GetDeadLetterMessagesAsync("orders", "order-processor");

foreach (var message in deadLetterMessages)
{
    // Inspect and reprocess or discard
    await HandleDeadLetter(message);
}
```

## Best Practices

1. **Idempotent Handlers**: Design subscribers to handle duplicate messages
2. **Message TTL**: Set appropriate time-to-live values
3. **Error Handling**: Implement retry logic with exponential backoff
4. **Monitoring**: Track message processing metrics

## Troubleshooting

### Messages Not Being Delivered

Check:
- Subscription is active
- Filters are correctly configured
- Dead-letter queue for failed messages

### High Latency

Solutions:
- Increase `maxConcurrentCalls`
- Optimize message handler performance
- Consider scaling out subscribers

## See Also

- [Event-Driven Architecture](./event-driven.md)
- [Service Bus Integration](./service-bus.md)
- [Error Handling](./error-handling.md)
