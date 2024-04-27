# The channel has been closed

In this project I have a bare implementation showing my "problem".
What is done:
- Used dotnet test-containers to start ActiveMQ Artemis;
- Added a setting in appsettings.json to set Autostart to true of false

When running this project with Autostart set to true upon stopping the application the following line is being logged:

```
fail: MassTransit[0]
      Failure removing auto-delete queues/topics
      System.Threading.Channels.ChannelClosedException: The channel has been closed.
         at MassTransit.Util.ChannelExecutor.Run[T](Func`1 method, CancellationToken cancellationToken) in /_/src/MassTransit/Util/ChannelExecutor.cs:line 147
         at MassTransit.ActiveMqTransport.Middleware.RemoveAutoDeleteAgent.DeleteAutoDelete(SessionContext context) in /_/src/Transports/MassTransit.ActiveMqTransport/ActiveMqTransport/Middleware/RemoveAutoDeleteAgent.cs:line 48
```