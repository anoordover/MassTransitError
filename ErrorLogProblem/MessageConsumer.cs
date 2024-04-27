using MassTransit;

namespace ErrorLogProblem;

public class MessageConsumer : IConsumer<Message>
{
    private readonly ILogger<MessageConsumer> _logger;

    public MessageConsumer(ILogger<MessageConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<Message> context)
    {
        _logger.LogDebug("Hello!");
        Console.WriteLine(context.Message.Greeting);
        return Task.CompletedTask;
    }
}