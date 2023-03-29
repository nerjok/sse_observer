using System;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalWebApiSSE.Reposirory;
using MinimalWebApiSSE.Repository;

namespace MinimalWebApiSSE.Controllers
{
  [ApiController]
  [Route("api")]
  public class ServerSentEventController : Controller
  {
    private readonly IMessageRepository _messageRepository;
    private readonly IOrderObservable _orderObservable;
    // private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions();


    public ServerSentEventController(IMessageRepository messageRepository, IOrderObservable orderObservable)
    {
      _messageRepository = messageRepository;
      _orderObservable = orderObservable;
    }

    [Produces("text/event-stream")]
    [HttpGet("sse2")]
    public async Task Get(CancellationToken cancellationToken)
    {
      Response.StatusCode = 200;
      Response.Headers.Add("Content-Type", "text/event-stream");
      Response.Headers.Add("Cache-Control", "no-cache");
      Response.Headers.Add("Connection", "keep-alive");

      async void OnNotification(object? sender, NotificationArgs eventArgs)
      {
        try
        {
          // idea: https://stackoverflow.com/a/58565850/80527

          await Response.WriteAsync($"id:{eventArgs.Notification.Id}\n", cancellationToken);
          await Response.WriteAsync("retry: 10000\n", cancellationToken);
          await Response.WriteAsync($"event:snackbar\n", cancellationToken);
          await Response.WriteAsync($"data: {eventArgs.Notification.Message}\n\n", cancellationToken);
          await Response.Body.FlushAsync(cancellationToken);
        }
        catch (Exception)
        {
        }
      }

      Console.WriteLine(_messageRepository.Kuku());
      _messageRepository.NotificationEvent += OnNotification;
      // while (true)
      // {
      //   await Response.WriteAsync($"data: ${DateTimeOffset.Now}\n\n");
      //   await Response.Body.FlushAsync();
      //   await Task.Delay(5000);
      // }
      while (!cancellationToken.IsCancellationRequested)
      {
        await Task.Delay(1000);
      }
    }

    [HttpGet("broadcast")]
    public Task Broadcast(string msg = "notFound")
    {
      var notification = new Notification();
      notification.Message = msg;

      _messageRepository.Broadcast(notification);

    var order = new Order();
    order.Message = msg;
      _orderObservable.TrackOrder(order);
      _orderObservable.Subscribe(new OrderReporter());

      return Task.CompletedTask;
    }
  }
}