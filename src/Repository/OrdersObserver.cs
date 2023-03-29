using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalWebApiSSE.Repository
{
  public class Order
  {
    public int Id { get; set; }
    public string Message { get; set; }
  }

  public interface IOrderObservable : IObservable<Order>
  {
    public new IDisposable Subscribe(IObserver<Order> observer);
    public void TrackOrder(Order order);
  }

  public class OrdersObservable : IOrderObservable
  {
    private List<IObserver<Order>> observers;

    public OrdersObservable()
    {
      observers = new List<IObserver<Order>>();
    }
    public IDisposable Subscribe(IObserver<Order> observer)
    {
      if (!observers.Contains(observer))
        observers.Add(observer);
      return new Unsubscriber(observers, observer);
    }

    private class Unsubscriber : IDisposable
    {
      private List<IObserver<Order>> _observers;
      private IObserver<Order> _observer;

      public Unsubscriber(List<IObserver<Order>> observers, IObserver<Order> observer)
      {
        this._observers = observers;
        this._observer = observer;
      }

      public void Dispose()
      {
        if (_observer != null && _observers.Contains(_observer))
          _observers.Remove(_observer);
      }
    }
    public void TrackOrder(Order order)
    {
      foreach (var observer in observers)
      {
        if (order.Message == null)
          observer.OnError(new Exception());
        else
          observer.OnNext(order);
      }
    }
  }

  public class OrderReporter : IObserver<Order>
  {
    void IObserver<Order>.OnCompleted()
    {
      throw new NotImplementedException();
    }

    void IObserver<Order>.OnError(Exception error)
    {
      Console.WriteLine(error);
    }

    void IObserver<Order>.OnNext(Order value)
    {
      Console.WriteLine("next");
      Console.WriteLine(value);
    }
  }
}