using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalWebApiSSE.Reposirory
{
  public class NotificationArgs : EventArgs
  {
    public Notification Notification { get; }

    public NotificationArgs(Notification notification)
    {
      Notification = notification;
    }
  }

  public class Notification
  {
    public int Id { get; set; }
    public string Message { get; set; }
  }
  public interface IMessageRepository
  {
    event EventHandler<NotificationArgs> NotificationEvent;
    void Broadcast(Notification notification);
    public string Kuku();
  }

  public class MessageRepository : IMessageRepository
  {
    public string Kuku()
    {

      return "kuku";
    }

    public MessageRepository()
    {
    }

    public event EventHandler<NotificationArgs> NotificationEvent;

    public void Broadcast(Notification notification)
    {
      NotificationEvent?.Invoke(this, new NotificationArgs(notification));
    }
  }
}